﻿using NLog;

/// \file CDevicesManager.cs
/// \brief Fichier principal de la dll.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;

namespace DeviceLibrary
{
    /// <summary>
    /// Enumération des causes des événements
    /// </summary>
    public enum Reason
    {
        /// <summary>
        /// Une pièce a été reconnue par le monnayeur.
        /// </summary>
        MONEYINTRODUCTED,

        /// <summary>
        /// Une erreur a été dectée sur le monnayeur.
        /// </summary>
        COINVALIDATORERROR,

        /// <summary>
        /// Les moyens de paiement ont été fermés.
        /// </summary>
        CASHCLOSED,

        /// <summary>
        /// Les moyens de paiement ont été ouverts.
        /// </summary>
        CASHOPENED,

        /// <summary>
        /// Une erreur a été detectée sur un hopper.
        /// </summary>
        HOPPERERROR,

        /// <summary>
        /// Le hopper a terminé la distribution.
        /// </summary>
        HOPPERDISPENSED,

        /// <summary>
        /// Une des sondes hardwares d'un hopper a changé d'état.
        /// </summary>
        HOPPERHWLEVELCHANGED,

        /// <summary>
        /// Un seuil de niveau a été atteint
        /// </summary>
        HOPPERSWLEVELCHANGED,

        /// <summary>
        /// Le hopper est vidé.
        /// </summary>
        HOPPEREMPTIED,

        /// <summary>
        /// Une erreur sur le BNR est survenue.
        /// </summary>
        BNRERREUR,

        /// <summary>
        /// La dll est prête.
        /// </summary>
        DLLLREADY,
    }

    /// <summary>
    /// Class d'évenement
    /// </summary>
    public class CalertEventArgs : EventArgs
    {
        /// <summary>
        /// Cause de l'événement.
        /// </summary>
        public Reason reason;

        /// <summary>
        /// Objet contenant les infomations concernant l'événement.
        /// </summary>
        public object donnee;
    }

    /// <summary>
    /// Class principale
    /// </summary>
    public class CDevicesManager : IDisposable
    {
        /// <summary>
        /// Instance le la class Logger utilisée pour les logs.
        /// </summary>
        internal static Logger Log;

        /// <summary>
        /// Nom du fichier des paramètres.
        /// </summary>
        internal string ParamFileName;

        /// <summary>
        /// Instance de la classe XmlDocument permettant la lecture des fichiers XML.
        /// </summary>
        private static XmlDocument parametersFile = new XmlDocument();

        /// <summary>
        /// Thread de la classe principale.
        /// </summary>
        private readonly Thread msgTask;

        ///// <summary>
        ///// Evenement de la classe principale.
        ///// </summary>
        //public CDevice.CEvent evenement;

        /// <summary>
        /// Variable contenant le montant à payer.
        /// </summary>
        public static int ToPay
        {
            get;
            set;
        }

        /// <summary>
        /// Instance du monnayeur
        /// </summary>
        public CCoinValidator monnayeur;

        /// <summary>
        /// Liste des hoppers
        /// </summary>
        public List<CHopper> Hoppers;

        /// <summary>
        /// Instance du BNR
        /// </summary>
        public CBNR_CPI bnX;

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void AlertEventHandler(object sender, CalertEventArgs e);

        /// <summary>
        /// Evenement levant un appel à la fonction déléguée.
        /// </summary>
        public event AlertEventHandler CallAlert;

        /// <summary>
        /// Masque des canaux activés.
        /// </summary>
        public static byte[] EnableChannels
        {
            get
            {
                byte[] result = { 0, 0 };
                try
                {
                    XmlNodeList canauxList = parametersFile.GetElementsByTagName("Canal");
                    foreach(XmlNode channel in canauxList)
                    {
                        byte posChannel = 0;
                        foreach(XmlElement e in channel)
                        {
                            switch(e.Name)
                            {
                                case "Position":
                                {
                                    posChannel = byte.Parse(e.InnerText);
                                    break;
                                }
                                case "Enable":
                                {
                                    result[(posChannel < 9) ? 0 : 1] += (byte)(Convert.ToByte(e.InnerText == "true") << ((posChannel < 9) ? posChannel - 1 : posChannel - 9));
                                    break;
                                }
                            }
                        }
                    }
                }
                catch(Exception E)
                {
                    Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /*-------------------------------------------------------*/

        /// <summary>
        /// Cette fonction lit, pour chaque canal, les chemins de tris et les hoppers devant être rechargés lors de l'insertion d'une pièce dans le monnayeur
        /// </summary>
        private void SetSortersAndHoppersToLoad()
        {
            try
            {
                XmlNodeList canauxList = parametersFile.GetElementsByTagName("Canal");
                foreach(XmlNode channel in canauxList)
                {
                    byte posChannel = 0;
                    byte pathSorter = 0;
                    byte hopperToLoad = 0;
                    foreach(XmlElement e in channel)
                    {
                        switch(e.Name)
                        {
                            case "Position":
                            {
                                posChannel = byte.Parse(e.InnerText);
                                break;
                            }
                            case "PathSorter":
                            {
                                pathSorter = byte.Parse(e.InnerText);
                                break;
                            }
                            case "HopperToLoad":
                            {
                                hopperToLoad = byte.Parse(e.InnerText);
                                break;
                            }
                        }
                    }
                    monnayeur.canaux[posChannel - 1].sorter.PathSorter = pathSorter;
                    monnayeur.canaux[posChannel - 1].HopperToLoad = hopperToLoad;
                }
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Effectue le rendu monnaie.
        /// </summary>
        private void ChangeBack()
        {
            try
            {
                int rest = CDevice.denominationInserted.TotalAmount - ToPay;
                if(rest > 0)
                {
                    foreach(CHopper hopper in Hoppers)
                    {
                        if(hopper.IsPresent &&
                            (hopper.deviceLevel.softLevel != CDevice.CLevel.SoftLevel.VIDE) &&
                            (hopper.deviceLevel.hardLevel != CDevice.CLevel.HardLevel.VIDE) &&
                            (hopper.CoinValue > 0) &&
                            (hopper.CoinValue <= rest))
                        {
                            hopper.CoinsToDistribute = (byte)(rest / hopper.CoinValue);
                            if((rest -= (int)(hopper.CoinsToDistribute * hopper.CoinValue)) == 0)
                            {
                                break;
                            }
                        }
                    }
                    foreach(CHopper hopper in Hoppers)
                    {
                        if(hopper.IsPresent && (hopper.CoinsToDistribute > 0))
                        {
                            hopper.State = CHopper.Etat.STATE_DISPENSE;
                        }
                    }
                    foreach(CHopper hopper in Hoppers)
                    {
                        if(hopper.IsPresent)
                        {
                            while(hopper.State != CHopper.Etat.STATE_IDLE)
                                ;
                            ;
                        }
                    }
                }
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Modifie le le chemin de tri du canal.
        /// </summary>
        /// <param name="canal">Canal du monnayeur</param>
        /// <param name="sorter">chemin du trieur</param>
        public void SetSorterPath(byte canal, byte sorter)
        {
            try
            {
                if(canal < 1 || canal > 16)
                {
                    throw new Exception("Le numéro de canal doit être compris entre 1 et 16");
                }
                if(sorter < 1 || sorter > 8)
                {
                    throw new Exception("Le numéro de chemin doit être compris entre 1 et 8");
                }
                monnayeur.canaux[canal].sorter.SetSorterPath(sorter);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Tâche principale de la dll
        /// </summary>
        private void Task()
        {
            Reason reason;
            while(true)
            {
                if(CDevice.eventsList.Count > 0)
                {
                    lock(CDevice.eventListLock)
                    {
                        reason = CDevice.eventsList[0].reason;
                    }
                    switch(reason)
                    {
                        case Reason.MONEYINTRODUCTED:
                        {
                            OnMoneyReceived(CDevice.eventsList[0]);

                            if((ToPay - CDevice.denominationInserted.TotalAmount) < 1)
                            {
                                monnayeur.IsCVToBeDeactivated = true;
                                bnX.IsBNRToBeDeactivated = true;
                                ChangeBack();
                                EndTransaction();
                                CDevice.denominationInserted.TotalAmount = 0;
                            }
                            CDevice.denominationInserted.BackTotalAmount = CDevice.denominationInserted.TotalAmount;
                            break;
                        }
                        case Reason.BNRERREUR:
                        {
                            OnBNRErreur(CDevice.eventsList[0]);
                            break;
                        }
                        case Reason.DLLLREADY:
                        {
                            OnDllReady();
                            break;
                        }
                        case Reason.COINVALIDATORERROR:
                        {
                            OnCVError(CDevice.eventsList[0]);
                            break;
                        }
                        case Reason.CASHCLOSED:
                        {
                            monnayeur.IsCVToBeDeactivated = true;
                            bnX.IsBNRToBeDeactivated = true;
                            CDevice.denominationInserted.TotalAmount = ToPay = 0;
                            OnCashClose();
                            break;
                        }
                        case Reason.CASHOPENED:
                        {
                            OnCashOpen();
                            break;
                        }
                        case Reason.HOPPERERROR:
                        {
                            OnHopperError(CDevice.eventsList[0]);
                            break;
                        }
                        case Reason.HOPPERDISPENSED:
                        {
                            OnHopperDispensed(CDevice.eventsList[0]);
                            break;
                        }
                        case Reason.HOPPERHWLEVELCHANGED:
                        {
                            OnHopperHardLevelChanged(CDevice.eventsList[0]);
                            break;
                        }
                        case Reason.HOPPERSWLEVELCHANGED:
                        {
                            OnHopperSoftLevelChanged(CDevice.eventsList[0]);
                            break;
                        }
                        case Reason.HOPPEREMPTIED:
                        {
                            OnHopperEmptied(CDevice.eventsList[0]);
                            break;
                        }
                        default:
                            break;
                    }
                    lock(CDevice.eventListLock)
                    {
                        CDevice.eventsList.RemoveAt(0);
                    }
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Cette fonction lit les paramètres des hoppers.
        /// </summary>
        private void ReadParamHopper()
        {
            try
            {
                XmlNodeList parametersHopperList = parametersFile.GetElementsByTagName("Hopper");
                byte byIndex = 0;
                foreach(XmlNode n in parametersHopperList)
                {
                    foreach(XmlElement e in n)
                    {
                        switch(e.Name)
                        {
                            case "ID":
                            {
                                Hoppers[byIndex].name = e.InnerText;
                                break;
                            }
                            case "Recharge":
                            {
                                Hoppers[byIndex].DefaultFilling = Convert.ToUInt32(e.InnerText);
                                break;
                            }
                            case "Critique":
                            {
                                Hoppers[byIndex].IsCritical = Convert.ToBoolean(e.InnerText);
                                break;
                            }
                            case "Valeur":
                            {
                                Hoppers[byIndex].CoinValue = Hoppers[byIndex].IsPresent ?
                                    Hoppers[byIndex].CoinValue = Convert.ToUInt32(e.InnerText) : 0;
                                break;
                            }
                            case "Niveaux":
                            {
                                foreach(XmlElement e2 in e.ChildNodes)
                                {
                                    switch(e2.Name)
                                    {
                                        case "Plein":
                                        {
                                            Hoppers[byIndex].LevelFullSoft = Convert.ToUInt32(e2.InnerText);
                                            break;
                                        }
                                        case "Haut":
                                        {
                                            Hoppers[byIndex].LevelHISoft = Convert.ToUInt32(e2.InnerText);
                                            break;
                                        }
                                        case "Bas":
                                        {
                                            Hoppers[byIndex].LevelLOSoft = Convert.ToUInt32(e2.InnerText);
                                            break;
                                        }
                                        case "Vide":
                                        {
                                            Hoppers[byIndex].LevelEmptySoft = Convert.ToUInt32(e2.InnerText);
                                            break;
                                        }
                                        default:
                                        {
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                    }
                    byIndex++;
                }
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsque le hopper est vidé.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperEmptied(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPEREMPTIED,
                    donnee = evenement,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsqu'un changement d'état d'une sonde de niveau d'un hopper est detecté.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperHardLevelChanged(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERHWLEVELCHANGED,
                    donnee = evenement
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsqu'un niveau de pièces atteint une des limites fixées pour le hopper
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperSoftLevelChanged(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERSWLEVELCHANGED,
                    donnee = evenement,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evéenemnt levé lorsqu'une error est detectée sur un hopper.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperError(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERERROR,
                    donnee = evenement,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsque la distribution est terminée.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperDispensed(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERDISPENSED,
                    donnee = evenement,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lors de la cloture de la transaction.
        /// </summary>
        private void OnCashClose()
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.CASHCLOSED,
                    donnee = null,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lors de l'ouverture des moyens de paiement.
        /// </summary>
        private void OnCashOpen()
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.CASHOPENED,
                    donnee = null,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evémenment levé lors de l'introduction d'une pièce.
        /// </summary>
        private void OnMoneyReceived(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.MONEYINTRODUCTED,
                    donnee = evenement,
                };
                Console.Beep();
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lors de la détection d'une erreur sur le monnayeur.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnCVError(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.COINVALIDATORERROR,
                    donnee = evenement,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsque la dll est prête.
        /// </summary>
        private void OnDllReady()
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.DLLLREADY,
                    donnee = null,
                };
                CallAlert(new object(), alertEventArgs);
                EndTransaction();
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void OnBNRErreur(CDevice.CEvent evenement)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.BNRERREUR,
                    donnee = evenement,
                };
                CallAlert(new object(), alertEventArgs);
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Ouvre une transaction le montant à payer.
        /// </summary>
        /// <param name="value">montant en centimes à payer</param>
        /// <remarks>Provoque l'ouverture des moyens de paiement.</remarks>
        public void BeginTransaction(int value)
        {
            try
            {
                if((ToPay = value) > CDevice.denominationInserted.TotalAmount)
                {
                    monnayeur.IsCVToBeActivated = monnayeur.IsPresent;
                    bnX.IsBNRToBeActivated = bnX.IsPresent;
                    lock(CDevice.eventListLock)
                    {
                        CDevice.eventsList.Add(new CDevice.CEvent
                        {
                            reason = Reason.CASHOPENED,
                            nameOfDevice = "",
                            data = null
                        });
                    }
                }
                else
                {
                    ToPay = CDevice.denominationInserted.TotalAmount = CDevice.denominationInserted.BackTotalAmount =  0;
                    monnayeur.IsCVToBeDeactivated = monnayeur.IsPresent;
                    bnX.IsBNRToBeDeactivated = monnayeur.IsPresent;
                    EndTransaction();
                }
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Cloture la transaction en cours
        /// </summary>
        public void EndTransaction()
        {
            lock(CDevice.eventListLock)
            {
                CDevice.CEvent evenement = new CDevice.CEvent
                {
                    reason = Reason.CASHCLOSED,
                    nameOfDevice = "",
                    data = null
                };
                CDevice.eventsList.Add(evenement);
            }
        }

        /// <summary>
        /// Renvoi le nom du port série utilisé par le bus ccTalk
        /// </summary>
        public string GetSerialPort()
        {
            return CccTalk.PortSerie.PortName;
        }

        /// <summary>
        /// Remise à zéro des compteurs.
        /// </summary>
        public void ResetCounters()
        {
            CccTalk.ResetCounters();
            foreach(CHopper hopper in Hoppers)
            {
                hopper.State = CHopper.Etat.STATE_CHECKLEVEL;
            }
        }

        /// <summary>
        /// Envoi une demande de distribution au BNR
        /// </summary>
        /// <param name="Amount"></param>
        public void BNRDispense(decimal Amount)
        {
            CBNR_CPI.bnr.Dispense((int)Amount, "EUR");
        }

        /// <summary>
        /// Constructeur de la classe principale
        /// </summary>
        public CDevicesManager()
        {
            try
            {
                Log = LogManager.GetCurrentClassLogger();
                Log.Info("\r\n\r\n\r\n{0}\r\n", messagesText.callDll);
                ParamFileName = "parametres.xml";
                ParamFileName = Directory.GetCurrentDirectory() + "\\" + ParamFileName;
                parametersFile.Load(ParamFileName);

                CccTalk.counters = new CcoinsCounters();
                CccTalk.counterSerializer = new BinaryFormatter();
                if(!File.Exists(CccTalk.fileCounterName))
                {
                    CccTalk.countersFile = File.Create(CccTalk.fileCounterName);
                    CccTalk.counterSerializer.Serialize(CccTalk.countersFile, CccTalk.counters);
                    CccTalk.countersFile.Close();
                }
                CccTalk.countersFile = File.Open(CccTalk.fileCounterName, FileMode.Open, FileAccess.ReadWrite);
                CccTalk.countersFile.Seek(0, SeekOrigin.Begin);
                CccTalk.counters = (CcoinsCounters)CccTalk.counterSerializer.Deserialize(CccTalk.countersFile);

                bnX = new CBNR_CPI();
                monnayeur = new CCoinValidator();
                if(monnayeur.ProductCode == "BV")
                {
                    monnayeur.CVTask.Abort();
                    Thread.Sleep(100);
                    monnayeur = new CPelicano();
                    ((CPelicano)monnayeur).SpeedMotor = Convert.ToByte(parametersFile.SelectSingleNode("/CashParameters/CoinValidator/SpeedMTR").InnerText);
                }
                SetSortersAndHoppersToLoad();

                Hoppers = new List<CHopper>(8);
                for(byte i = 1; i < 9; i++)
                {
                    Hoppers.Add(new CHopper(i));
                }
                ReadParamHopper();
                foreach(CHopper hopper in Hoppers)
                {
                    hopper.State = CHopper.Etat.STATE_CHECKLEVEL;
                }
                Hoppers.Sort((x, y) => y.CoinValue.CompareTo(x.CoinValue));

                lock(CDevice.eventListLock)
                {
                    CDevice.CEvent evenement = new CDevice.CEvent
                    {
                        reason = Reason.DLLLREADY,
                        nameOfDevice = "",
                        data = null
                    };
                    CDevice.eventsList.Insert(0, evenement);
                }
                msgTask = new Thread(Task);
                msgTask.Start();
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Fonction libérant les ressources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                try
                {
                    CccTalk.countersFile.Close();
                }
                catch(Exception)
                {
                }
                try
                {
                    monnayeur.CVTask.Abort();
                }
                catch(Exception)
                {
                }
                foreach(CHopper h in Hoppers)
                {
                    try
                    {
                        h.HTask.Abort();
                    }
                    catch
                    {
                    }
                }
            }

            Log.Trace("Stop");
            Log.Trace("\r\n---------------------\r\n");
            LogManager.Shutdown();
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructeur
        /// </summary>
        ~CDevicesManager()
        {
            Dispose(true);
        }
    }
}