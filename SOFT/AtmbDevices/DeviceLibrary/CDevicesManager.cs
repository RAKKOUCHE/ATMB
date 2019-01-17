/// \file CDevicesManager.cs
/// \brief Fichier principal de la dll.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using NLog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;

namespace DeviceLibrary
{
    /// <summary>
    /// Class principale
    /// </summary>
    public partial class CDevicesManager : IDisposable
    {
        #region VARIABLES

        /// <summary>
        /// Instance de la classe XmlDocument permettant la lecture des fichiers XML.
        /// </summary>
        private static readonly XmlDocument parametersFile = new XmlDocument();

        /// <summary>
        /// Instance du BNR
        /// </summary>
        private readonly CBNR_CPI bnX;

        /// <summary>
        /// Thread de la classe principale.
        /// </summary>
        private readonly Thread msgTask;

        /// <summary>
        /// Instance le la class Logger utilisée pour les logs.
        /// </summary>
        internal static Logger Log;

        /// <summary>
        /// Variable contenant le montant à payer.
        /// </summary>
        internal static int ToPay;

        /// <summary>
        /// Instance de la classe CCashReceived.
        /// </summary>
        internal CCashReceived cashReceived;

        /// <summary>
        /// Nom du fichier des paramètres.
        /// </summary>
        internal string ParamFileName;

        /// <summary>
        /// Liste des hoppers
        /// </summary>
        public List<CHopper> Hoppers;

        /// <summary>
        /// Instance du monnayeur
        /// </summary>
        public CCoinValidator monnayeur;

        /// <summary>
        /// Prototype de la fonctio delegate recevant les messages de la dll.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void FireEventHandler(object sender, CEvent.FireEventArg e);

        /// <summary>
        /// Evenement levant un appel à la fonction déléguée.
        /// </summary>
        public event FireEventHandler FireEvent;

        #endregion VARIABLES

        #region PROPRIETEES

        /// <summary>
        /// Masque des canaux activés.
        /// </summary>
        internal static byte[] EnableChannelsCV
        {
            get
            {
                byte[] result = { 0, 0 };
                try
                {
                    XmlNodeList canauxList = parametersFile.GetElementsByTagName("Canal");
                    foreach (XmlNode channel in canauxList)
                    {
                        byte posChannel = 0;
                        foreach (XmlElement e in channel)
                        {
                            switch (e.Name)
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
                catch (Exception E)
                {
                    Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        #endregion PROPRIETEES

        #region METHODES

        #region PRIVATE

        /// <summary>
        /// Effectue le rendu monnaie.
        /// </summary>
        private void ChangeBack()
        {
            Log.Debug("Change back");
            try
            {
                //Calcul du montant à rendre.
                int rest = CDevice.denominationInserted.TotalAmount - ToPay;
                //Si il y a un rendu à effectuer.
                if (rest > 0)
                {
                    Log.Debug("Change back : A rendre {0:C2}", (decimal)rest / 100);
                    {
                        if (CBNR_CPI.bnr != null && bnX.IsPresent)
                            //Recherche u plus petit billet disponible.
                            try
                            {
                                while (CBNR_CPI.State != CBNR_CPI.Etat.STATE_IDLE)
                                    ;
                                int divider = bnX.SearchDivider;
                                //Si le reste est supérieur au plus petit billet disponible, on provoquera une distribution de billet
                                if (rest > divider)
                                {
                                    //Le montant à distribuer sera le reste modulo la valeur du plus petit billet
                                    bnX.CheckAndDispense(bnX.AmountToDispense = rest / divider * divider);
                                    rest -= bnX.AmountToDispense;
                                }
                            }
                            catch (Exception E)
                            {
                                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            }
                    }
                    //Pour chaque hopper
                    foreach (CHopper hopper in Hoppers)
                    {
                        //Verifie si le hopper est capable de distribuer.
                        //Comme les hoppers sont classés par ordre croissant de valeur, donc on distribue le minimum de pièces.
                        if (hopper.IsPresent &&
                            (hopper.deviceLevel.softLevel != CHopper.CLevel.SoftLevel.VIDE) &&
                            (hopper.deviceLevel.hardLevel != CHopper.CLevel.HardLevel.VIDE) &&
                            (hopper.CoinValue > 0) &&
                            (hopper.CoinValue <= rest))
                        {
                            hopper.CoinsToDistribute = (byte)(rest / hopper.CoinValue);
                            if ((rest -= (int)(hopper.CoinsToDistribute * hopper.CoinValue)) <= 0)
                            {
                                break;
                            }
                        }
                    }
                    foreach (CHopper hopper in Hoppers)
                    {
                        if (hopper.IsPresent && (hopper.CoinsToDistribute > 0))
                        {
                            Log.Debug("Distribution de {0} pièces par le {1}", hopper.CoinsToDistribute, hopper.ToString());
                            hopper.State = CHopper.Etat.STATE_DISPENSE;
                        }
                    }
                    foreach (CHopper hopper in Hoppers)
                    {
                        if (hopper.IsPresent)
                        {
                            while (hopper.State != CHopper.Etat.STATE_IDLE)
                                ;
                            ;
                        }
                    }
                    //S'assure de la liberation de l'évenment et de la présentation des billets si nécessaire.
                    if (CBNR_CPI.isDispensable)
                    {
                        try
                        {
                            CBNR_CPI.ev.WaitOne(CBNR_CPI.BnrDefaultOperationTimeOutInMS);
                            CBNR_CPI.ev.Reset();
                            CBNR_CPI.bnr.Present();
                            CBNR_CPI.ev.WaitOne(CBNR_CPI.BnrDefaultOperationTimeOutInMS);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Le BNR a distribué des billets.
        /// </summary>
        /// <param name="evenment"></param>
        private void OnBNRDispensed(CEvent evenment)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.BNRDISPENSE,
                    donnee = evenment,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé quand une erreur s'est produte sur le BNR.
        /// </summary>
        private void OnBNRErreur(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.BNRERREUR,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evenment levé quand un module est réinséré.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnbnrModuleReinsere(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.BNRMODULEREINSERE,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Indique que le module est remis à zéro.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnBNRResetModule(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.BNRRAZMETER,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evenement levé si un module est retiré du BNR.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnBRNModuleRemoved(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.BNRMODULEMANQUANT,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        private void OnCashBoxRemoved(CEvent evenment)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.CASHBOXREMOVED,
                    donnee = evenment,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lors de la cloture de la transaction.
        /// </summary>
        private void OnCashClose()
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.CASHCLOSED,
                    donnee = cashReceived,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lors de l'ouverture des moyens de paiement.
        /// </summary>
        private void OnCashOpen()
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.CASHOPENED,
                    donnee = null,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lors de la détection d'une erreur sur le monnayeur.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnCVError(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.COINVALIDATORERROR,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsque la dll est prête.
        /// </summary>
        private void OnDllReady()
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.DLLLREADY,
                    donnee = null,
                };
                FireEvent(new object(), fireEventArgs);
                EndTransaction();
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsque la distribution est terminée.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperDispensed(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.HOPPERDISPENSED,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsque le hopper est vidé.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperEmptied(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.HOPPEREMPTIED,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evéenemnt levé lorsqu'une error est detectée sur un hopper.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperError(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.HOPPERERROR,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsqu'un changement d'état d'une sonde de niveau d'un hopper est detecté.
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperHardLevelChanged(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.HOPPERHWLEVELCHANGED,
                    donnee = evenement
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evénement levé lorsqu'un niveau de pièces atteint une des limites fixées pour le hopper
        /// </summary>
        /// <param name="evenement"></param>
        private void OnHopperSoftLevelChanged(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.HOPPERSWLEVELCHANGED,
                    donnee = evenement,
                };
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Evémenment levé lors de l'introduction d'une pièce.
        /// </summary>
        private void OnMoneyReceived(CEvent evenement)
        {
            try
            {
                CEvent.FireEventArg fireEventArgs = new CEvent.FireEventArg
                {
                    reason = CEvent.Reason.MONEYINTRODUCTED,
                    donnee = evenement,
                };
                Console.Beep();
                FireEvent(new object(), fireEventArgs);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                foreach (XmlNode n in parametersHopperList)
                {
                    foreach (XmlElement e in n)
                    {
                        switch (e.Name)
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
                                foreach (XmlElement e2 in e.ChildNodes)
                                {
                                    switch (e2.Name)
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
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Cette fonction lit, pour chaque canal, les chemins de tris et les hoppers devant être rechargés lors de l'insertion d'une pièce dans le monnayeur
        /// </summary>
        private void SetSortersAndHoppersToLoad()
        {
            try
            {
                XmlNodeList canauxList = parametersFile.GetElementsByTagName("Canal");
                foreach (XmlNode channel in canauxList)
                {
                    byte posChannel = 0;
                    byte pathSorter = 0;
                    byte hopperToLoad = 0;
                    foreach (XmlElement e in channel)
                    {
                        switch (e.Name)
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
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Tâche principale de la dll
        /// </summary>
        private void Task()
        {
            CEvent.Reason reason;
            while (true)
            {
                try
                {
                    if (CDevice.eventsList.Count > 0)
                    {
                        lock (CDevice.eventListLock)
                        {
                            reason = CDevice.eventsList[0].reason;
                        }
                        switch (reason)
                        {
                            case CEvent.Reason.MONEYINTRODUCTED:
                            {
                                OnMoneyReceived(CDevice.eventsList[0]);
                                cashReceived.listValueIntroduced.Add(CDevice.denominationInserted.ValeurCent);
                                cashReceived.amountIntroduced += CDevice.denominationInserted.ValeurCent;
                                if ((ToPay - CDevice.denominationInserted.TotalAmount) < 1)
                                {
                                    if (bnX != null)
                                    {
                                        bnX.IsBNRToBeDeactivated = true;
                                        while (bnX.IsBNRToBeDeactivated)
                                            ;
                                        while (CBNR_CPI.State != CBNR_CPI.Etat.STATE_IDLE)
                                            ;
                                    }
                                    if (monnayeur != null)
                                    {
                                        monnayeur.IsCVToBeDeactivated = true;
                                        while (monnayeur.IsCVToBeDeactivated)
                                            ;
                                    }
                                    ChangeBack();
                                    EndTransaction();
                                    CDevice.denominationInserted.TotalAmount = 0;
                                }
                                CDevice.denominationInserted.BackTotalAmount = CDevice.denominationInserted.TotalAmount;
                                break;
                            }
                            case CEvent.Reason.BNRERREUR:
                            {
                                OnBNRErreur(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.DLLLREADY:
                            {
                                OnDllReady();
                                break;
                            }
                            case CEvent.Reason.COINVALIDATORERROR:
                            {
                                OnCVError(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.CASHCLOSED:
                            {
                                if (monnayeur != null)
                                {
                                    monnayeur.IsCVToBeDeactivated = true;
                                }
                                bnX.IsBNRToBeDeactivated = true;
                                CDevice.denominationInserted.TotalAmount = ToPay = 0;
                                OnCashClose();
                                break;
                            }
                            case CEvent.Reason.CASHOPENED:
                            {
                                OnCashOpen();
                                break;
                            }
                            case CEvent.Reason.HOPPERERROR:
                            {
                                OnHopperError(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.HOPPERDISPENSED:
                            {
                                OnHopperDispensed(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.HOPPERHWLEVELCHANGED:
                            {
                                OnHopperHardLevelChanged(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.HOPPERSWLEVELCHANGED:
                            {
                                OnHopperSoftLevelChanged(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.HOPPEREMPTIED:
                            {
                                OnHopperEmptied(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.BNRMODULEMANQUANT:
                            {
                                OnBRNModuleRemoved(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.BNRMODULEREINSERE:
                            {
                                OnbnrModuleReinsere(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.BNRRAZMETER:
                            {
                                OnBNRResetModule(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.BNRDISPENSE:
                            {
                                OnBNRDispensed(CDevice.eventsList[0]);
                                break;
                            }
                            case CEvent.Reason.CASHBOXREMOVED:
                            {
                                OnCashBoxRemoved(CDevice.eventsList[0]);
                                break;
                            }
                            default:
                                break;
                        }
                        lock (CDevice.eventListLock)
                        {
                            CDevice.eventsList.RemoveAt(0);
                        }
                    }
                    Thread.Sleep(100);
                }
                catch (Exception E)
                {
                    Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }
        }

        #endregion PRIVATE

        #region PUBLIC

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
                if (!File.Exists(CccTalk.fileCounterName))
                {
                    CccTalk.countersFile = File.Create(CccTalk.fileCounterName);
                    CccTalk.counterSerializer.Serialize(CccTalk.countersFile, CccTalk.counters);
                    CccTalk.countersFile.Close();
                }
                CccTalk.countersFile = File.Open(CccTalk.fileCounterName, FileMode.Open, FileAccess.ReadWrite);
                CccTalk.countersFile.Seek(0, SeekOrigin.Begin);
                CccTalk.counters = (CcoinsCounters)CccTalk.counterSerializer.Deserialize(CccTalk.countersFile);
                try
                {
                    bnX = new CBNR_CPI();
                }
                catch (Exception E)
                {
                    Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                monnayeur = new CCoinValidator();
                if (monnayeur.ProductCode == "BV")
                {
                    monnayeur.CVTask.Abort();
                    Thread.Sleep(100);
                    monnayeur = new CPelicano();
                    ((CPelicano)monnayeur).SpeedMotor = Convert.ToByte(parametersFile.SelectSingleNode("/CashParameters/CoinValidator/SpeedMTR").InnerText);
                }
                SetSortersAndHoppersToLoad();

                Hoppers = new List<CHopper>(8);
                for (byte i = 1; i < 9; i++)
                {
                    Hoppers.Add(new CHopper(i));
                }
                ReadParamHopper();
                foreach (CHopper hopper in Hoppers)
                {
                    hopper.State = CHopper.Etat.STATE_CHECKLEVEL;
                }
                Hoppers.Sort((x, y) => y.CoinValue.CompareTo(x.CoinValue));

                lock (CDevice.eventListLock)
                {
                    CEvent evenement = new CEvent
                    {
                        reason = CEvent.Reason.DLLLREADY,
                        nameOfDevice = "",
                        data = null
                    };
                    CDevice.eventsList.Insert(0, evenement);
                }
                msgTask = new Thread(Task);
                msgTask.Start();
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                if ((ToPay = value) > CDevice.denominationInserted.TotalAmount)
                {
                    if (monnayeur != null)
                    {
                        monnayeur.IsCVToBeActivated = monnayeur.IsPresent;
                    }
                    if (bnX != null)
                    {
                        bnX.IsBNRToBeActivated = bnX.IsPresent;
                    }
                    lock (CDevice.eventListLock)
                    {
                        CDevice.eventsList.Add(new CEvent
                        {
                            reason = CEvent.Reason.CASHOPENED,
                            nameOfDevice = "",
                            data = null
                        });
                    }
                    cashReceived = new CCashReceived();
                }
                else
                {
                    ToPay = CDevice.denominationInserted.TotalAmount = CDevice.denominationInserted.BackTotalAmount = 0;
                    monnayeur.IsCVToBeDeactivated = monnayeur.IsPresent;
                    bnX.IsBNRToBeDeactivated = monnayeur.IsPresent;
                    EndTransaction();
                }
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Reupère les billets présents dans le bezel.
        /// </summary>
        public void BNRBillRetract()
        {
            if (CBNR_CPI.bnr != null && bnX.IsPresent)
            {
                CBNR_CPI.State = CBNR_CPI.Etat.STATE_RETRACT;
                while (CBNR_CPI.State != CBNR_CPI.Etat.STATE_IDLE) ;
            }
        }

        /// <summary>
        /// Retourne les billets e escrow.
        /// </summary>
        public void BNRBillRollBack()
        {
            if (CBNR_CPI.bnr != null && bnX.IsPresent)
            {
                CBNR_CPI.State = CBNR_CPI.Etat.STATE_ROLLBACK;
                while (CBNR_CPI.State != CBNR_CPI.Etat.STATE_IDLE) ;
                BeginTransaction(ToPay);
            }
        }

        /// <summary>
        /// Envoi une demande de distribution au BNR
        /// </summary>
        /// <param name="amount">Montant à distribuer.</param>
        public void BNRDispense(int amount)
        {
            bnX.CheckAndDispense(amount);
        }

        /// <summary>
        /// Abandonne la transaction et retourne le montant introduit
        /// </summary>
        public void CancelTransaction()
        {
            ToPay = 0;
            if (CBNR_CPI.bnr != null && bnX.IsPresent)
            {
                CBNR_CPI.State = CBNR_CPI.Etat.STATE_ROLLBACK;
                while (CBNR_CPI.State != CBNR_CPI.Etat.STATE_IDLE) ;
            }
            ChangeBack();
            EndTransaction();
        }

        /// <summary>
        /// Remise à zéro des compteurs de la caisse
        /// </summary>
        public void CashBoxRemoved()
        {
            try
            {
                CcoinsCounters.CCoinInCB coinInCashBox = new CcoinsCounters.CCoinInCB
                {
                    amountTotalInCB = CccTalk.counters.totalAmountInCB
                };
                CccTalk.counters.totalAmountInCabinet -= CccTalk.counters.totalAmountInCB;
                CccTalk.counters.totalAmountInCB = 0;
                foreach (CCoinValidator.CCanal canal in monnayeur.canaux)
                {
                    coinInCashBox.coin[canal.Number - 1] = new CcoinsCounters.CCoinInCB.CCoin
                    {
                        coinValue = canal.coinId.ValeurCent,
                        coinInCB = canal.CoinInInCB,
                        amountCoinInCB = canal.AmountCoinInCB
                    };
                    canal.CoinInInCB = 0;
                    canal.AmountCoinInCB = 0;
                }
                lock (CDevice.eventListLock)
                {
                    CDevice.eventsList.Add(new CEvent
                    {
                        reason = CEvent.Reason.CASHBOXREMOVED,
                        nameOfDevice = "CAISSE",
                        data = coinInCashBox,
                    });
                }
                CccTalk.counters.SaveCounters();
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Libère les ressources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cloture la transaction en cours
        /// </summary>
        public void EndTransaction()
        {
            lock (CDevice.eventListLock)
            {
                CEvent evenement = new CEvent
                {
                    reason = CEvent.Reason.CASHCLOSED,
                    nameOfDevice = "",
                    data = null,
                };
                CDevice.eventsList.Add(evenement);
            }
        }

        /// <summary>
        /// Renvoi le nom du port série utilisé par le bus ccTalk
        /// </summary>
        public string GetSerialPort()
        {
            if (CccTalk.PortSerie != null)
            {
                return CccTalk.PortSerie.PortName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Remise à zéro des compteurs.
        /// </summary>
        public void ResetCounters()
        {
            CccTalk.ResetCounters();
            foreach (CHopper hopper in Hoppers)
            {
                hopper.State = CHopper.Etat.STATE_CHECKLEVEL;
            }
        }

        /// <summary>
        /// Fixe le compteur de billets contenus dans le loader.
        /// </summary>
        /// <param name="numberOfBill"></param>
        public void SetLoaderNumber(uint numberOfBill)
        {
            CBNR_CPI.bnr.SetLoaderCuContent("LO1", numberOfBill);
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
                if (canal < 1 || canal > 16)
                {
                    throw new Exception("Le numéro de canal doit être compris entre 1 et 16");
                }
                if (sorter < 1 || sorter > 8)
                {
                    throw new Exception("Le numéro de chemin doit être compris entre 1 et 8");
                }
                monnayeur.canaux[canal].sorter.SetSorterPath(sorter);
            }
            catch (Exception E)
            {
                Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        #endregion PUBLIC

        /// <summary>
        /// Destructeur
        /// </summary>
        ~CDevicesManager()
        {
            Dispose(true);
        }

        /// <summary>
        /// Fonction libérant les ressources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    msgTask.Abort();
                }
                catch
                {
                }

                try
                {
                    bnX.bnrTask.Abort();
                }
                catch
                {
                }
                try
                {
                    CccTalk.countersFile.Close();
                }
                catch
                {
                }

                try
                {
                    monnayeur.CVTask.Abort();
                }
                catch
                {
                }
                try
                {
                    foreach (CHopper h in Hoppers)
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
                catch
                {
                }
            }
            bnX.Dispose();
            monnayeur.Dispose();
            Log.Trace("Stop");
            Log.Trace("\r\n---------------------\r\n");
            LogManager.Shutdown();
        }

        #endregion METHODES
    }
}