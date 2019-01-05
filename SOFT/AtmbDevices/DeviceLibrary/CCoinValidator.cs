/// \file CCoinValidator.cs
/// \brief Fichier contenant la classe CCoinValidator
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;
using System.Threading;

namespace DeviceLibrary
{
    /// <summary>
    /// Class gérant le monnayeur.
    /// </summary>
    public partial class CCoinValidator : CcashReader
    {
        /// <summary>
        /// Chemin de tri par défaut.
        /// </summary>
        public byte defaultSorterPath;

        /// <summary>
        /// Classe des erreurs du monnayeur.
        /// </summary>
        public class CErroCV
        {
            /// <summary>
            ///
            /// </summary>
            public byte code;

            /// <summary>
            ///
            /// </summary>
            public CVErrorCodes errorText;
        }

        /// <summary>
        /// Instance de la classe des erreurs du monnayeurs.
        /// </summary>
        public CErroCV errorCV;

        /// <summary>
        /// Enumération des codes d'état du monnayeurs.
        /// </summary>
        protected enum CVStatus
        {
            /// <summary>
            /// Monnayeur à l'état normal
            /// </summary>
            OK = 0,

            /// <summary>
            /// Le mechanisme de rendu de pièce est activé.
            /// </summary>
            COINRETURNACTIVATED = 1,

            /// <summary>
            /// Le système anti-fishing est activé.
            /// </summary>
            COSACTIVATED = 2,
        }

        /// <summary>
        /// Etats de la porte de rejet.
        /// </summary>
        protected enum TrashDoor
        {
            /// <summary>
            /// Trappe de sortie du Pelicano fermée.
            /// </summary>
            CLOSED = 0,

            /// <summary>
            /// Trappe de sortie du Pelcano ouverte.
            /// </summary>
            OPEN = 1,
        }

        /// <summary>
        /// Etat des optos de sortie.
        /// </summary>
        protected enum LowerSensor
        {
            /// <summary>
            /// Senseurs de sortie sont libres.
            /// </summary>
            FREE = 0,

            /// <summary>
            /// Un senseur de sortie est occupée.
            /// </summary>
            BUSY = 1,
        }

        /// <summary>
        /// Nombre de canaux du monnayeur.
        /// </summary>
        public const byte numChannel = 16;

        /// <summary>
        /// Tableau contenant l'identification des pièces.
        /// </summary>
        //public CCoindID[] coinIDs;

        private byte backEventCounter;

        /// <summary>
        /// Sauvegarde du compteur d'événement.
        /// </summary>
        protected byte BackEventCounter
        {
            get => backEventCounter;
            set => backEventCounter = value;
        }

        private bool isCVToBeDeactivated;

        /// <summary>
        /// Indique si le monnayeur doit être adtivé.
        /// </summary>
        public bool IsCVToBeDeactivated
        {
            get => isCVToBeDeactivated;
            set => isCVToBeDeactivated = value;
        }

        private bool isCVToBeActivated;

        /// <summary>
        /// Indique si le monnayeur doit être adtivé.
        /// </summary>
        public bool IsCVToBeActivated
        {
            get => isCVToBeActivated;
            set => isCVToBeActivated = value;
        }

        //TODO A déplacer vers PELICANO
        private TrashDoor trashLid;

        /// <summary>
        /// Contient l'indicateur d'ouverture du containner.
        /// </summary>
        protected TrashDoor TrashLid
        {
            get => trashLid;
            set => trashLid = value;
        }

        private LowerSensor exitSensor;

        /// <summary>
        /// Indique si les optiques de sortie sont libre.
        /// </summary>
        protected LowerSensor ExitSensor
        {
            get => exitSensor;
            set => exitSensor = value;
        }

        //todo jusqu'ici

        private byte channelInProgress;

        /// <summary>
        /// Numéro du canal en cours d'utilisation.
        /// </summary>
        protected byte ChannelInProgress
        {
            get => channelInProgress;
            set => channelInProgress = value;
        }

        /// <summary>
        /// Buffer contenant les informations sur les pièces lues ou les erreurs rencontrées
        /// </summary>
        public CCVcreditBuffer creditBuffer;

        private byte overrideMask;

        /// <summary>
        /// Masque des chemins optionnels
        /// </summary>
        protected byte OverrideMask
        {
            get => overrideMask;
            set => overrideMask = value;
        }

        private Etat state;

        /// <summary>
        /// Etat de la machie d'état.
        /// </summary>
        protected Etat State
        {
            get => state;
            set => state = value;
        }

        /// <summary>
        /// Tableau des canaux
        /// </summary>
        public CCanal[] canaux;

        /// <summary>
        /// Thread de gestion de la machine d'état du monnayeur.
        /// </summary>
        public Thread CVTask;

        /// <summary>
        /// Chemin utilisé dans le trieur pour le canal
        /// </summary>
        public byte sorterPath;

        private bool isCVInitialized;

        /// <summary>
        /// Flag indiquant si le monnayeur est initialisé.
        /// </summary>
        public bool IsCVInitialized
        {
            get => isCVInitialized;
            set => isCVInitialized = value;
        }

        ///// <summary>
        ///// Contient les informations concernant un évenement du monnayeur.
        ///// </summary>
        //public CEvent cvEvent;

        /// <summary>
        /// Nom de produit du monnayeur.
        /// </summary>
        private string name;

        /********************************************************************************/

        /// <summary>
        /// Renvoi la version de la data base.
        /// </summary>
        /// <remarks>Header 243</remarks>
        protected byte DataBaseVersion
        {
            get
            {
                byte dataBaseVersion = 0;
                try
                {
                    CDevicesManager.Log.Info(messagesText.dataBaseVersion, DeviceAddress);
                    dataBaseVersion = GetByte(Header.REQUESTDATABASEVER);
                    CDevicesManager.Log.Info(messagesText.dataBaseVersion, DeviceAddress, dataBaseVersion);
                }
                catch(Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return dataBaseVersion;
            }
        }

        /// <summary>
        /// Active les activateurs du monnayeur
        /// </summary>
        /// <param name="mask">Masque correspondant aux bobines</param>
        /// <returns></returns>
        protected bool TestSolenoid(byte mask = 0X01)
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.testSolenoid, DeviceAddress);
                byte[] bufferParam = { mask };
                if(!IsCmdccTalkSended(DeviceAddress, Header.TESTSOLENOID, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManager.Log.Error(messagesText.erreurCmd, Header.TESTSOLENOID, DeviceAddress);
                    throw new Exception(messagesText.erreurCmd);
                }
            }
            catch(Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Modifie l'override status.
        /// </summary>
        /// <remarks>Header 222</remarks>
        protected void SetOverrideStatus(byte mask)
        {
            try
            {
                CDevicesManager.Log.Info("Définit le masque d'override des trieurs {0}", DeviceAddress);
                byte[] bufferParam = { mask };
                if(!IsCmdccTalkSended(DeviceAddress, Header.MODIFYOVERRIDESTATUS, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManager.Log.Error("Impossible de modifier l'override status du {0}", DeviceAddress);
                }
            }
            catch(Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Renvoie l'override status.
        /// </summary>
        /// <returns>L'octet de l'override</returns>
        /// <remarks>Header 221</remarks>
        protected byte OverrideStatus
        {
            get
            {
                byte result = 0;
                try
                {
                    CDevicesManager.Log.Info("Lecture du masque d'override des trieurs du {0}", DeviceAddress);
                    result = GetByte(Header.REQUESTOVERRIDESTATUS);
                    CDevicesManager.Log.Info("Le masque d'override des trieurs du {0} et {1}", DeviceAddress, result);
                }
                catch(Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Renvoi le chemin de tri par défaut.
        /// </summary>
        /// <returns>Le chemin de tri par défaut. 0 en cas d'échec.</returns>
        /// <remarks>Header 188</remarks>
        private byte DefaultSorterPath
        {
            get
            {
                byte result = 0;
                try
                {
                    CDevicesManager.Log.Info("Lecture du chemin de tri par défaut du {0}", DeviceAddress);
                    CDevicesManager.Log.Info("Le chemin par défaut du {0} est {1}", result = GetByte(Header.REQUESTDEFAULTSORTERPATH));
                }
                catch(Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Renvoi le status du coin validator
        /// </summary>
        /// <returns>status</returns>
        protected CVStatus Status
        {
            get
            {
                CVStatus status = 0;
                try
                {
                    status = (CVStatus)GetByte(Header.REQUESTSTATUS);
                }
                catch(Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return status;
            }
        }

        /// <summary>
        /// Les donnees variables du monnaayeur.
        /// </summary>
        /// <returns></returns>
        public byte[] VariablesSet
        {
            get
            {
                byte[] bufferIn = { 0, 0, 0, 0 };
                try
                {
                    CDevicesManager.Log.Info("Lecture de l'ensemble des variables du {0}", DeviceAddress);
                    if(IsCmdccTalkSended(DeviceAddress, CccTalk.Header.REQUESTVARIABLESET, 0, null, bufferIn))
                    {
                        CDevicesManager.Log.Info("variableSetToRead");
                    }
                }
                catch(Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return bufferIn;
            }
        }

        /// <summary>
        /// Cette fonction analyse le buffer des crédits et des codes erreurs
        /// </summary>
        protected void CheckCreditBuffer()
        {
            try
            {
                CDevicesManager.Log.Debug("Lecture du buffer des crédits et des codes erreurs {0}", DeviceAddress);
                int eventNumber = (BackEventCounter < creditBuffer.EventCounter) ? creditBuffer.EventCounter - BackEventCounter : 255 + creditBuffer.EventCounter - BackEventCounter;
                BackEventCounter = creditBuffer.EventCounter;
                if(eventNumber > 5)
                {
                    throw new Exception("Trop d'évenements dans le buffer de credit ou code erreur");
                }
                for(int i = 0; i < eventNumber; i++)
                {
                    if(creditBuffer.Result[i, 0] == 0)
                    {
                        errorCV.errorText = (CVErrorCodes)creditBuffer.Result[i, 1];
                        lock(eventListLock)
                        {
                            eventsList.Add(new CEvent
                            {
                                reason = Reason.COINVALIDATORERROR,
                                nameOfDevice = base.ProductCode,
                                data = errorCV
                            });
                        }
                        CDevicesManager.Log.Error("L'erreur {0} a été rencontré sur le {1}", errorCV.errorText, DeviceAddress);
                    }
                    else
                    {
                        denominationInserted.ValeurCent = canaux[creditBuffer.Result[i, 0] - 1].coinId.ValeurCent;
                        denominationInserted.CVChannel = creditBuffer.Result[i, 0];
                        denominationInserted.CVPath = creditBuffer.Result[i, 1];
                        denominationInserted.TotalAmount += canaux[creditBuffer.Result[i, 0] - 1].coinId.ValeurCent;

                        lock(eventListLock)
                        {
                            eventsList.Add(new CEvent
                            {
                                reason = Reason.MONEYINTRODUCTED,
                                nameOfDevice = ProductCode,
                                data = denominationInserted
                            });
                        }

                        counters.totalAmountCashInCV += denominationInserted.ValeurCent;
                        counters.amountCoinInAccepted[creditBuffer.Result[i, 0] - 1] += denominationInserted.ValeurCent;
                        ++counters.coinsInAccepted[creditBuffer.Result[i, 0] - 1];
                        counters.totalAmountInCabinet += denominationInserted.ValeurCent;
                        if(canaux[creditBuffer.Result[i, 0] - 1].HopperToLoad == 0)
                        {
                            counters.totalAmountInCB = denominationInserted.ValeurCent;
                        }
                        else
                        {
                            counters.amountInHopper[canaux[creditBuffer.Result[i, 0] - 1].HopperToLoad - 1] += denominationInserted.ValeurCent;
                            ++counters.coinsInHopper[canaux[creditBuffer.Result[i, 0] - 1].HopperToLoad - 1];
                        }
                        counters.SaveCounters();
                        CDevicesManager.Log.Debug("Une pièce de {0:C2} a été reconnue", (decimal)canaux[creditBuffer.Result[i, 0] - 1].coinId.ValeurCent / 100);
                    }
                }
            }
            catch(Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Initialisation du monnayeur.
        /// </summary>
        public override void Init()
        {
            CDevicesManager.Log.Debug("Initialisation monnayeur");
            IsCVInitialized = false;
            canaux = new CCanal[numChannel];
            for(byte i = 0; i < numChannel; i++)
            {
                canaux[i] = new CCanal((byte)(i + 1), this);
            }
            errorCV = new CErroCV();
            PollingDelay = 20;
            BackEventCounter = 0;
            creditBuffer = new CCVcreditBuffer(this);
            CDevicesManager.Log.Info("Identification du {0} \r\n//////////////////", DeviceAddress);
            OverrideMask = 0XFF;
            defaultSorterPath = 1;
            InhibitMask = CDevicesManager.EnableChannels;
            creditBuffer.GetBufferCredit();
            backEventCounter = creditBuffer.EventCounter;
        }

        /// <summary>
        /// Traitement des états du monnayers.
        /// </summary>
        public void CheckState()
        {
            try
            {
                mutexCCTalk.WaitOne();
                switch(State)
                {
                    case Etat.STATE_INIT:
                    {
                        Init();
                        State = Etat.STATE_DISABLEMASTER;
                        break;
                    }
                    case Etat.STATE_RESET:
                    {
                        if(IsDeviceReseted())
                        {
                            State = Etat.STATE_INIT;
                        }
                        else
                        {
                            Thread.Sleep(60000);
                        }
                        break;
                    }
                    case Etat.STATE_SIMPLEPOLL:
                    {
                        CDevicesManager.Log.Debug("Simple poll sur {0}", DeviceAddress);
                        if(!SimplePoll)
                        {
                            CDevicesManager.Log.Info("Echec du simple poll sur le {0}", DeviceAddress);
                        }
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_GETEQUIPEMENTCATEGORY:
                    {
                        CDevicesManager.Log.Info("La catégorie de l'equipement est : {0} ", EquipementCategory);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETMANUFACTURERID;
                        break;
                    }
                    case Etat.STATE_GETMANUFACTURERID:
                    {
                        CDevicesManager.Log.Debug("Le code fabricant du {0} est : {1}", DeviceAddress, Manufacturer);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETPRODUCTCODE;
                        break;
                    }
                    case Etat.STATE_GETPRODUCTCODE:
                    {
                        CDevicesManager.Log.Debug("Le code du produit du {0} est : {1}", DeviceAddress, name = ProductCode);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETBUILDCODE;
                        break;
                    }
                    case Etat.STATE_GETBUILDCODE:
                    {
                        CDevicesManager.Log.Debug("Le build code du {0} est : {1}", DeviceAddress, BuildCode);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETSOFTWAREREVISION;
                        break;
                    }
                    case Etat.STATE_GETSOFTWAREREVISION:
                    {
                        CDevicesManager.Log.Debug("La révision du software du {0} est : {1}", DeviceAddress, SWRev);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETDATABASEVERSION;
                        break;
                    }
                    case Etat.STATE_GETSERIALNUMBER:
                    {
                        CDevicesManager.Log.Info("Le numéro de série du {0} est : {1}", DeviceAddress, SerialNumber);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETEQUIPEMENTCATEGORY;
                        break;
                    }
                    case Etat.STATE_GETDATABASEVERSION:
                    {
                        CDevicesManager.Log.Debug("La version de la base de donnée du {0} est : {1}", DeviceAddress, DataBaseVersion);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_TESTSOLENOID;
                        break;
                    }
                    case Etat.STATE_TESTSOLENOID:
                    {
                        CDevicesManager.Log.Info("Test des bobines du {0}", DeviceAddress);
                        TestSolenoid();
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETSTATUS;
                        break;
                    }
                    case Etat.STATE_GETSTATUS:
                    {
                        CDevicesManager.Log.Info("Le status du {0} est {1}", DeviceAddress, Status);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETCOINID;
                        break;
                    }
                    case Etat.STATE_SELF_TEST:
                    {
                        CDevicesManager.Log.Info("Effectue le self-test du {0}", DeviceAddress);
                        SelfTestResult result = SelfTest;
                        string message = string.Format("Le self-test du {0} indque l'erreur {1}", DeviceAddress, result);
                        if(result == SelfTestResult.OK)
                        {
                            CDevicesManager.Log.Info("Le self-test du {0} n'indique pas d'erreur", DeviceAddress);
                        }
                        else
                        {
                            CDevicesManager.Log.Error("Le self-test du {0} indque l'erreur {1}", DeviceAddress, result);
                        }
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_GETCOINID:
                    {
                        foreach(CCanal canal in canaux)
                        {
                            canal.coinId.GetCoinId();
                            CDevicesManager.Log.Info("Le code pays du canal {0} est {1}, la valeur est {2}, la version est {3}", canal.Number, canal.coinId.CountryCode, canal.coinId.ValeurCent, canal.coinId.Issue);
                        }
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_SETINHIBITSTATUS;
                        break;
                    }
                    case Etat.STATE_GETINHIBITSTATUS:
                    {
                        CDevicesManager.Log.Info("Lecture des inhibitions des canaux du {0}", DeviceAddress);
                        GetInhibitMask(InhibitMask);
                        CDevicesManager.Log.Info("Le mask d'inhibition du {0} est {1} et {2}", DeviceAddress, InhibitMask[0], InhibitMask[1]);
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_SETINHIBITSTATUS:
                    {
                        CDevicesManager.Log.Info("Inhibition des canaux du {0}, DeviceAddress");
                        SetInhibitStatus(InhibitMask);
                        CDevicesManager.Log.Debug("Le masque des inhibitions du {0} est {1} et {2}", DeviceAddress, InhibitMask[0], InhibitMask[1]);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_SETSORTERPATH;
                        break;
                    }
                    case Etat.STATE_GETPOLLINGDELAY:
                    {
                        PollingDelay = PollingPriority * 2 / 3;
                        CDevicesManager.Log.Info("Le déali de polling pour {0} est de {1}", DeviceAddress, PollingDelay);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_SELF_TEST;
                        break;
                    }
                    case Etat.STATE_GETCREDITBUFFER:
                    {
                        creditBuffer.GetBufferCredit();
                        if(!IsCVInitialized)
                        {
                            backEventCounter = creditBuffer.EventCounter;
                            IsCVInitialized = true;
                            evReady.Set();
                        }
                        State = Etat.STATE_IDLE;
                        break;
                    }
                    case Etat.STATE_DISABLEMASTER:
                    {
                        CDevicesManager.Log.Info("desactive le {0}", DeviceAddress);
                        MasterDisable();
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETSERIALNUMBER;
                        break;
                    }
                    case Etat.STATE_ENABLEMASTER:
                    {
                        CDevicesManager.Log.Info("Active le {0}", DeviceAddress);
                        MasterEnable();
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_GETMASTERINHIBT:
                    {
                        CDevicesManager.Log.Info("Le {0} est {1}", DeviceAddress, MasterInhibitStatus);
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_GETOVERRIDE:
                    {
                        CDevicesManager.Log.Info("Le status ovveride du {0} est : {1} ", OverrideStatus);
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_SETOVERRIDE:
                    {
                        CDevicesManager.Log.Info("Enregistre le status du {0} : {1}", OverrideMask);
                        SetOverrideStatus(OverrideMask);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETDEFAULTSORTERPATH;
                        break;
                    }
                    case Etat.STATE_GETSORTERPATH:
                    {
                        //todo lire les pièces autorisees dans le fichier de paramètre et comparer les masks
                        //si les masks sont identiques passer à l'dentification des pièces.
                        CDevicesManager.Log.Info("lecture des chemins de tris du {0}", DeviceAddress);
                        foreach(CCanal canal in canaux)
                        {
                            CDevicesManager.Log.Info("La sortie du trieur pour le canal {0} du {1} est {2}", canal.sorter.PathSorter);
                        }
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_SETSORTERPATH:
                    {
                        CDevicesManager.Log.Info("Enregistrement du chemin de tri {0} pour le canal {1}", sorterPath, channelInProgress);
                        canaux[ChannelInProgress].sorter.SetSorterPath(sorterPath);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_GETPOLLINGDELAY;
                        break;
                    }
                    case Etat.STATE_GETDEFAULTSORTERPATH:
                    {
                        CDevicesManager.Log.Info("le chemin de  tri par default du {0} est : {1}", DeviceAddress, DefaultSorterPath);
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_SETDEFAULTSORTERPATH:
                    {
                        CDevicesManager.Log.Info("Chemin de sortie par défault : {0}", DefaultSorterPath);
                        State = IsCVInitialized ? Etat.STATE_GETCREDITBUFFER : Etat.STATE_SETINHIBITSTATUS;
                        break;
                    }
                    case Etat.STATE_CHECKCREDIBUFFER:
                    {
                        CheckCreditBuffer();
                        State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                    case Etat.STATE_IDLE:
                    {
                        State = Etat.STATE_GETCREDITBUFFER;
                        if(IsCVToBeActivated)
                        {
                            State = Etat.STATE_ENABLEMASTER;
                            IsCVToBeActivated = false;
                        }
                        if(IsCVToBeDeactivated)
                        {
                            State = Etat.STATE_DISABLEMASTER;
                            isCVToBeDeactivated = false;
                        }
                        if(backEventCounter != creditBuffer.EventCounter)
                        {
                            State = Etat.STATE_CHECKCREDIBUFFER;
                        }
                        break;
                    }
                    case Etat.STATE_STOP:
                    {
                        CVTask.Abort();
                        break;
                    }
                    default:
                    {
                        //State = Etat.STATE_GETCREDITBUFFER;
                        break;
                    }
                }
                //if (ProductCode != "BV")
                //{
                //    State = Etat.STATE_GETCREDITBUFFER;
                //}
            }
            catch(Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), "Exception dans le thread du monnayeur : " + E.Message, E.StackTrace);
            }
            finally
            {
                try
                {
                    mutexCCTalk.ReleaseMutex();
                }
                catch(Exception)
                {
                }
            }
        }

        /// <summary>
        /// Tâche de la machine d'état du monnayeur.
        /// </summary>
        public override void Task()
        {
            CDevicesManager.Log.Debug("Tâche de lecture des évenements concernant du {0}", DeviceAddress);
            while(true)
            {
                CheckState();
                Thread.Sleep(PollingDelay);
            }
        }

        /// <summary>
        /// Constructeur de la class CCoinValidator
        /// </summary>
        public CCoinValidator()
        {
            try
            {
                
                CDevicesManager.Log.Info("Instancation de la classe CCoinValidator.");
                DeviceAddress = DefaultDevicesAddress.CoinAcceptor;
                if(!(IsPresent = SimplePoll))
                {
                    Thread.Sleep(100);
                    IsDeviceReseted();
                    IsPresent = SimplePoll;
                }
                if(IsPresent)
                {
                    State = Etat.STATE_INIT;
                    CVTask = new Thread(Task);
                    CVTask.Start();
                }
                else
                {
                    throw new Exception("Pas de monnayeur detecté.");
                }
            }
            catch(Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                evReady.Set();
            }
            evReady.WaitOne(60000);
        }
    }
}