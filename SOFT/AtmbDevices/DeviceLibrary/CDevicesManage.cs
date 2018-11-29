/// \file CDevicesManage.cs
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
using NLog;

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
    public class CDevicesManage
    {
        internal static Logger Log = NLog.LogManager.GetCurrentClassLogger();
        internal string ParamFileName = "parametres.xml";
        private static XmlDocument parametersFile = new XmlDocument();
        private int toPay;
        private Thread MsgTask;
        private bool isDllReady;

        /// <summary>
        /// Instance du monnayeur
        /// </summary>
        public CCoinValidator monnayeur;

        /// <summary>
        /// Liste des hoppers
        /// </summary>
        public List<CHopper> Hoppers;

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
                        byte.TryParse(channel.ChildNodes[0].InnerText, out byte numChannel);
                        if(numChannel < 9)
                        {
                            if(channel.ChildNodes[1].InnerText == "true")
                            {
                                result[0] += (byte)(1 << (numChannel - 1));
                            }
                        }
                        else
                        {
                            if(channel.ChildNodes[1].InnerText == "true")
                            {
                                result[1] += (byte)(1 << (numChannel - 9));
                            }
                        }
                    }
                }
                catch(Exception E)
                {
                    Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
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
                    byte.TryParse(channel.ChildNodes[0].InnerText, out byte numChannel);
                    byte.TryParse(channel.ChildNodes[2].InnerText, out byte pathSorter);
                    byte.TryParse(channel.ChildNodes[3].InnerText, out byte hopperToLoad);
                    monnayeur.canaux[numChannel - 1].sorter.PathSorter = pathSorter;
                    monnayeur.canaux[numChannel - 1].HopperToLoad = hopperToLoad;
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
                int rest = CDevice.denominationInserted.TotalAmount - toPay;
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
                            hopper.State = CHopper.Etat.DISPENSE;
                        }
                    }
                    foreach(CHopper hopper in Hoppers)
                    {
                        if(hopper.IsPresent)
                        {
                            while(hopper.State != CHopper.Etat.IDLE)
                                ;
                            hopper.SubCounters(hopper.dispenseStatus.CoinsPaid);
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
        /// Tâche principale de la dll
        /// </summary>
        private void TaskMessage()
        {
            while(true)
            {
                if(isDllReady)
                {
                    isDllReady = false;
                    OnDllReady();
                }
                foreach(CHopper hopper in Hoppers)
                {
                    if(hopper.IsPresent)
                    {
                        if(hopper.IsDispensed)
                        {
                            hopper.IsDispensed = false;
                            if(!hopper.isEmptyingInProgress)
                            {
                                OnHopperDispensed(hopper);
                            }
                            else
                            {
                                hopper.isEmptyingInProgress = false;
                            }
                        }
                        if(hopper.deviceLevel.isSoftLevelChanged)
                        {
                            hopper.deviceLevel.isSoftLevelChanged = false;
                            OnHopperSoftLevelChanged(hopper);
                        }
                        if((hopper.deviceLevel.isHardLevelChanged))
                        {
                            hopper.deviceLevel.isHardLevelChanged = false;
                            OnHopperHardLevelChanged(hopper);
                        }
                        if(hopper.isEmptied)
                        {
                            hopper.isEmptied = false;
                            OnHopperEmptied(hopper);
                        }
                        if(hopper.isHopperError)
                        {
                            hopper.isHopperError = false;
                            hopper.ResetDevice();
                            hopper.Init();
                            hopper.errorHopper.isHopperCritical = hopper.IsCritical;
                            OnHopperError(hopper);
                        }
                    }
                }
                if(monnayeur.errorCV.errorText != CCoinValidator.CVErrorCodes.NULL)
                {
                    OnCVError();
                    monnayeur.errorCV.errorText = CCoinValidator.CVErrorCodes.NULL;
                }
                if(CDevice.denominationInserted.BackTotalAmount != CDevice.denominationInserted.TotalAmount)
                {
                    OnMoneyReceived();
                    if((toPay - CDevice.denominationInserted.TotalAmount) < 1)
                    {
                        monnayeur.IsCVToBeDeactivated = true;
                        OnCashClose();
                        ChangeBack();
                        CDevice.denominationInserted.TotalAmount = 0;
                    }
                    CDevice.denominationInserted.BackTotalAmount = CDevice.denominationInserted.TotalAmount;
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
                                byIndex = Convert.ToByte(Convert.ToInt32(e.InnerText) - 1);
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
        /// <param name="hopper"></param>
        private void OnHopperEmptied(CHopper hopper)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPEREMPTIED,
                    donnee = hopper,
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
        /// <param name="hopper"></param>
        private void OnHopperHardLevelChanged(CHopper hopper)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERHWLEVELCHANGED,
                    donnee = hopper.deviceLevel
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
        /// <param name="hopper"></param>
        private void OnHopperSoftLevelChanged(CHopper hopper)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERSWLEVELCHANGED,
                    donnee = hopper.deviceLevel,
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
        /// <param name="hopper"></param>
        private void OnHopperError(CHopper hopper)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERERROR,
                    donnee = hopper.errorHopper,
                };
                CallAlert(new object(), alertEventArgs);
                hopper.ResetDevice();
                hopper.Init();
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }

        }

        /// <summary>
        /// Evénement levé lorsque la distribution est terminée.
        /// </summary>
        /// <param name="hopper"></param>
        private void OnHopperDispensed(CHopper hopper)
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.HOPPERDISPENSED,
                    donnee = hopper.dispenseStatus.dispensedResult,
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
        private void OnMoneyReceived()
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.MONEYINTRODUCTED,
                    donnee = CDevice.denominationInserted,
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
        private void OnCVError()
        {
            try
            {
                CalertEventArgs alertEventArgs = new CalertEventArgs
                {
                    reason = Reason.COINVALIDATORERROR,
                    donnee = monnayeur.errorCV,
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
        public void OpenTransaction(int value)
        {
            try
            {
                if(monnayeur.IsCVToBeActivated = (toPay = value) > CDevice.denominationInserted.TotalAmount)
                {
                    OnCashOpen();
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
        public void CloseTransaction()
        {
            monnayeur.IsCVToBeDeactivated = true;
            CDevice.denominationInserted.TotalAmount = toPay = 0;
            OnCashClose();
        }

        /// <summary>
        /// Renvoi le nom du port série utilisé par le bus ccTalk
        /// </summary>
        public string GetSerialPort()
        {
            return CccTalk.PortSerie.PortName;
        }

        /// <summary>
        /// Constructeur de la classe principale
        /// </summary>
        public CDevicesManage()
        {
            isDllReady = false;
            try
            {
                Log.Info("\r\n\r\n\r\n{0}\r\n", messagesText.callDll);
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
                monnayeur = new CCoinValidator();
                if(!monnayeur.IsPresent)
                {
                    throw new Exception("Pas de monnayeur detecté.");
                }
                else
                {
                    if(monnayeur.ProductCode == "BV")
                    {
                        monnayeur = new CPelicano();
                        ((CPelicano)monnayeur).SpeedMotor = Convert.ToByte(parametersFile.SelectSingleNode("/CashParameters/CoinValidator/SpeedMTR").InnerText);
                    }
                    monnayeur.Init();
                    SetSortersAndHoppersToLoad();
                }
                Hoppers = new List<CHopper>();
                for(byte i = 1; i < 9; i++)
                {
                    Hoppers.Add(new CHopper(i));
                }
                ReadParamHopper();
                Hoppers.Sort((x, y) => y.CoinValue.CompareTo(x.CoinValue));
                foreach(CHopper hopper in Hoppers)
                {
                    hopper.Init();
                }
                monnayeur.CVTask.Start();
                MsgTask = new Thread(TaskMessage);
                MsgTask.Start();
                isDllReady = true;
            }
            catch(Exception E)
            {
                Log.Error("Erreur {0}, {1}, {2}", E.GetType(), E.Message, E.StackTrace);
            }
            finally
            {
            }
        }


        /// <summary>
        /// Fonction libérant les ressources
        /// </summary>
        public void Dispose()
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
            Log.Trace("Stop");
            Log.Trace("\r\n---------------------\r\n");
            NLog.LogManager.Shutdown();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        ~CDevicesManage()
        {
            Dispose();
        }
    }
}
