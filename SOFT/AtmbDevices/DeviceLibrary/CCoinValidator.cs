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
                    CDevicesManage.Log.Info(messagesText.dataBaseVersion, DeviceAddress);
                    dataBaseVersion = GetByte(Header.REQUESTDATABASEVER);
                    CDevicesManage.Log.Info(messagesText.dataBaseVersion, DeviceAddress, dataBaseVersion);
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                CDevicesManage.Log.Info(messagesText.testSolenoid, DeviceAddress);
                byte[] bufferParam = { mask };
                if(!IsCmdccTalkSended(DeviceAddress, Header.TESTSOLENOID, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManage.Log.Error(messagesText.erreurCmd, Header.TESTSOLENOID, DeviceAddress);
                    throw new Exception(messagesText.erreurCmd);
                }
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                CDevicesManage.Log.Info("Définit le masque d'override des trieurs {0}", DeviceAddress);
                byte[] bufferParam = { mask };
                if(!IsCmdccTalkSended(DeviceAddress, Header.MODIFYOVERRIDESTATUS, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManage.Log.Error("Impossible de modifier l'override status du {0}", DeviceAddress);
                }
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                    CDevicesManage.Log.Info("Lecture du masque d'override des trieurs du {0}", DeviceAddress);
                    result = GetByte(Header.REQUESTOVERRIDESTATUS);
                    CDevicesManage.Log.Info("Le masque d'override des trieurs du {0} et {1}", DeviceAddress, result);
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Modifie le chemin de sortie par défaut.
        /// </summary>
        /// <remarks>Header 189</remarks>
        private void SetDefaultSorterpath(byte defaultPath)
        {
            try
            {
                CDevicesManage.Log.Info("Enregisrement du chemin de sortie par défaut {0} ({1})", DeviceAddress, defaultPath);
                byte[] bufferParam = { defaultPath };
                if(!IsCmdccTalkSended(DeviceAddress, Header.MODIFYDEFAULTSORTERPATH, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManage.Log.Error("Impossible de modifier le chemin par défaut {0}", DeviceAddress);
                }
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                    CDevicesManage.Log.Info("Lecture du chemin de tri par défaut du {0}", DeviceAddress);
                    CDevicesManage.Log.Info("Le chemin par défaut du {0} est {1}", result = GetByte(Header.REQUESTDEFAULTSORTERPATH));
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return status;
            }
        }

        /// <summary>
        /// Cette fonction analyse le buffer des crédits et des codes erreurs
        /// </summary>
        protected void CheckCreditBuffer()
        {
            try
            {
                CDevicesManage.Log.Debug("Lecture du buffer des crédits et des codes erreurs {0}", DeviceAddress);
                int eventNumber = (BackEventCounter < creditBuffer.EventCounter) ? creditBuffer.EventCounter - BackEventCounter : 255 + creditBuffer.EventCounter - BackEventCounter;
                BackEventCounter = creditBuffer.EventCounter;
                if(eventNumber > 5)
                {
                    throw new Exception("Trop d'évenements dans le buffer de credit ou code erreur");
                }
                for(int i = 0; i < eventNumber; i++)
                {
                    byte channelNumber = creditBuffer.Result[i, 0];
                    if(channelNumber == 0)
                    {
                        errorCV.code = creditBuffer.Result[i, 1];
                        errorCV.errorText = (CVErrorCodes)creditBuffer.Result[i, 1];
                        CDevicesManage.Log.Error("L'erreur {0} a été rencontré sur le {1}", (CCoinValidator.CVErrorCodes)creditBuffer.Result[i, 1], DeviceAddress);
                    }
                    else
                    {
                        denominationInserted.ValeurCent = canaux[channelNumber - 1].coinId.ValeurCent;
                        denominationInserted.CVChannel = channelNumber;
                        denominationInserted.CVPath = creditBuffer.Result[i, 1];
                        denominationInserted.TotalAmount += canaux[channelNumber - 1].coinId.ValeurCent;
                        counters.totalAmountCashInCV += denominationInserted.ValeurCent;
                        counters.amountCoinInAccepted[channelNumber - 1] += denominationInserted.ValeurCent;
                        ++counters.coinsInAccepted[channelNumber - 1];
                        counters.totalAmountInCabinet += denominationInserted.ValeurCent;
                        if(canaux[channelNumber - 1].HopperToLoad == 0)
                        {
                            counters.totalAmountInCB = denominationInserted.ValeurCent;
                        }
                        else
                        {
                            counters.amountInHopper[canaux[channelNumber - 1].HopperToLoad - 1] += denominationInserted.ValeurCent;
                            ++counters.coinsInHopper[canaux[channelNumber - 1].HopperToLoad - 1];
                        }
                        counters.SaveCounters();
                        CDevicesManage.Log.Debug("Une pièce de {0:C2} a été reconnue", (decimal)canaux[channelNumber - 1].coinId.ValeurCent / 100);
                    }
                }
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Initialisation du monnayeur.
        /// </summary>
        public override void  Init()
        {
            DeviceAddress = DefaultDevicesAddress.CoinAcceptor;
            CDevicesManage.Log.Info("Initialisation du {0}", DeviceAddress);
            BackEventCounter = 0;
            MasterDisable();
            creditBuffer = new CCVcreditBuffer(this);
            CDevicesManage.Log.Info("Identification du {0} \r\n//////////////////", DeviceAddress);
            CDevicesManage.Log.Info("Catégorie du périphérique : {0}", EquipementCategory);
            CDevicesManage.Log.Info("Fabricant : {0}", Manufacturer);
            CDevicesManage.Log.Info("Code du périphérique : {0}", ProductCode);
            CDevicesManage.Log.Info("Code de fabrication : {0}", BuildCode);
            CDevicesManage.Log.Info("Révision software : {0}", SWRev);
            CDevicesManage.Log.Info("Version de la base de données : {0}", DataBaseVersion);
            CDevicesManage.Log.Info("Test des bobines a {0}", TestSolenoid(0XFF) ? "réussi" : "echoué ");
            CDevicesManage.Log.Info("Etat du {0} {1}", DeviceAddress, Status);
            SetOverrideStatus(0XFF);
            CDevicesManage.Log.Info("Chemin de sortie par défault : {0}", DefaultSorterPath);
            CDevicesManage.Log.Info("Le délai de polling du périphérique est {0}", PollingDelay = PollingPriority * 2 / 3);
            creditBuffer.GetBufferCredit();
            backEventCounter = creditBuffer.EventCounter;
            SetDefaultSorterpath(1);
            SetInhibitStatus(CDevicesManage.EnableChannels);
        }

        /// <summary>
        /// Machine d'état du monnayeur.
        /// </summary>
        protected virtual void CheckState()
        {
            try
            {
                CDevicesManage.Log.Info("Vérification de la machine d'état du {0}", DeviceAddress);
                switch (State)
                {
                    case Etat.STATE_INIT:
                    {
                        CDevicesManage.Log.Debug("Initialisation monnayeur");
                        Init();
                        break;
                    }
                    case Etat.STATE_RESET:
                    {
                        byte loop = 1;
                        do
                        {
                            CDevicesManage.Log.Debug("Reset du {0} Essai {1}", DeviceAddress, loop);
                        } while (!ResetDevice() && (++loop < 4));
                        Thread.Sleep(100);
                        if (loop == 0)
                        {
                            CDevicesManage.Log.Error("Impossible d'effectuer le reset du Pelicano");
                            //TODO envoyé le message d'erreur.
                        }
                        break;
                    }
                    case Etat.STATE_SIMPLEPOLL:
                    {
                        CDevicesManage.Log.Debug("Simple poll sur {0}", DeviceAddress);
                        if (!SimplePoll)
                        {
                            CDevicesManage.Log.Info("Echec du simple poll sur le {0}", DeviceAddress);
                            //TODO message d'erreur.
                        }
                        break;
                    }
                    case Etat.STATE_GETEQUIPEMENTCATEGORY:
                    {
                        CDevicesManage.Log.Debug("La catégorie de l'equipement est : {0} ", EquipementCategory);
                        break;
                    }
                    case Etat.STATE_GETMANUFACTURERID:
                    {
                        CDevicesManage.Log.Debug("Le code fabricant du {0} est : {1}", DeviceAddress, Manufacturer);
                        break;
                    }
                    case Etat.STATE_GETPRODUCTCODE:
                    {
                        CDevicesManage.Log.Debug("Le code du produit du {0} est : {1}", DeviceAddress, ProductCode);
                        break;
                    }
                    case Etat.STATE_GETBUILDCODE:
                    {
                        CDevicesManage.Log.Debug("Le build code du {0} est : {1}", DeviceAddress, BuildCode);
                        break;
                    }
                    case Etat.STATE_GETSOFTWAREREVISION:
                    {
                        CDevicesManage.Log.Debug("La révision du software du {0} est : {1}", DeviceAddress, SWRev);
                        break;
                    }
                    case Etat.STATE_GETSERIALNUMBER:
                    {
                        CDevicesManage.Log.Info("Le numéro de série du {0} est : {1}", DeviceAddress, SerialNumber);
                        break;
                    }

                    case Etat.STATE_GETDATABASEVERSION:
                    {
                        CDevicesManage.Log.Debug("La version de la base de donnée du {0} est : {1}", DeviceAddress, DataBaseVersion);
                        break;
                    }
                    case Etat.STATE_TESTSOLENOID:
                    {
                        CDevicesManage.Log.Info("Test des bobines du {0}", DeviceAddress);
                        TestSolenoid();
                        break;
                    }
                    case Etat.STATE_GETSTATUS:
                    {
                        CDevicesManage.Log.Info("Lecture du status du {0}", DeviceAddress);
                        CDevicesManage.Log.Info("Le status du {0} est {1}", DeviceAddress, Status);
                        break;
                    }
                    case Etat.STATE_SELF_TEST:
                    {
                        CDevicesManage.Log.Info("Effectue le self-test du {0}", DeviceAddress);
                        SelfTestResult result = SelfTest;
                        string message = string.Format("Le self-test du {0} indque l'erreur {1}", DeviceAddress, result);
                        if (result == SelfTestResult.OK)
                        {
                            CDevicesManage.Log.Info("Le self-test du {0} n'indique pas d'erreur", DeviceAddress);
                        }
                        else
                        {
                            CDevicesManage.Log.Error("Le self-test du {0} indque l'erreur {1}", DeviceAddress, result);
                        }
                        break;
                    }
                    case Etat.STATE_GETCOINID:
                    {
                        foreach (CCanal canal in canaux)
                        {
                            canal.coinId.GetCoinId();
                            CDevicesManage.Log.Info("Le code pays du canal {0} est {1}, la valeur est {2}, la version est {3}", canal.Number, canal.coinId.CountryCode, canal.coinId.ValeurCent, canal.coinId.Issue);
                        }
                        break;
                    }
                    case Etat.STATE_GETINHIBITSTATUS:
                    {
                        CDevicesManage.Log.Info("Lecture des inhibitions des canaux du {0}", DeviceAddress);
                        GetInhibitMask(InhibitMask);
                        CDevicesManage.Log.Info("Le mask d'inhibition du {0} est {1} et {2}", DeviceAddress, InhibitMask[0], InhibitMask[1]);
                        break;
                    }
                    case Etat.STATE_SETINHIBITSTATUS:
                    {
                        CDevicesManage.Log.Info("Inhibition des canaux du {0}, DeviceAddress");
                        SetInhibitStatus(InhibitMask);
                        CDevicesManage.Log.Info("Le masque des inhibitions du {0} est {1} et {2}", DeviceAddress, InhibitMask[0], InhibitMask[1]);
                        break;
                    }
                    case Etat.STATE_GETPOLLINGPRIORITY:
                    {
                        CDevicesManage.Log.Info("Requête du délai de polling");
                        CDevicesManage.Log.Info("Le délai maximum, de polling est de {0} ms pour le {1}", PollingPriority, DeviceAddress);
                        break;
                    }
                    case Etat.STATE_SETPOLLINGDELAY:
                    {
                        PollingDelay = (PollingPriority * 2 / 3);
                        CDevicesManage.Log.Info("Le délai de polling pour le {0} est fixé à {1}", DeviceAddress, PollingDelay);
                        break;
                    }
                    case Etat.STATE_GETCREDITBUFFER:
                    {
                        creditBuffer.GetBufferCredit();
                        break;
                    }
                    case Etat.STATE_DISABLEMASTER:
                    {
                        CDevicesManage.Log.Info("desactive le {0}", DeviceAddress);
                        MasterDisable();
                        state = Etat.STATE_IDLE;
                        break;
                    }
                    case Etat.STATE_ENABLEMASTER:
                    {
                        CDevicesManage.Log.Info("Active le {0}", DeviceAddress);
                        MasterEnable();
                        break;
                    }
                    case Etat.STATE_GETMASTERINHIBT:
                    {
                        CDevicesManage.Log.Info("Le {0} est {1}", DeviceAddress, MasterInhibitStatus);
                        break;
                    }
                    case Etat.STATE_GETOVERRIDE:
                    {

                        CDevicesManage.Log.Info("Le status ovveride du {0} est : {1} ", OverrideStatus);
                        break;
                    }
                    case Etat.STATE_SETOVERRIDE:
                    {
                        CDevicesManage.Log.Info("Enregistre le status du {0} : {1}", OverrideMask);
                        SetOverrideStatus(OverrideMask);
                        break;
                    }
                    case Etat.STATE_GETSORTERPATH:
                    {
                        //todo lire les pièces autorisees dans le fichier de paramètre et comparer les masks
                        //si les masks sont identiques passer à l'dentification des pièces.
                        CDevicesManage.Log.Info("lecture des chemins de tris du {0}", DeviceAddress);
                        foreach (CCanal canal in canaux)
                        {
                            CDevicesManage.Log.Info("La sortie du trieur pour le canal {0} du {1} est {2}", canal.sorter.PathSorter);
                        }
                        break;
                    }
                    case Etat.STATE_SETSORTERPATH:
                    {
                        CDevicesManage.Log.Info("Enregistrement du chemin de tri {0} pour le canal {1}", sorterPath, channelInProgress);
                        canaux[ChannelInProgress].sorter.SetSorterPath(sorterPath);
                        break;
                    }
                    case Etat.STATE_GETDEFAULTSORTERPATH:
                    {
                        CDevicesManage.Log.Info("le chemin de  tri par default du {0} est : {1}", DeviceAddress, DefaultSorterPath);
                        break;
                    }
                    case Etat.STATE_SETDEFAULTSORTERPATH:
                    {
                        break;
                    }
                    case Etat.STATE_CHECKCREDIBUFFER:
                    {
                        CheckCreditBuffer();
                        break;
                    }
                    case Etat.STATE_IDLE:
                    {
                        break;
                    }

                    case Etat.STATE_TRASHEMPTY:
                        break;
                    case Etat.STATE_SETSPEEDMOTOR:
                        break;
                    case Etat.STATE_GETSPEEDMOTOR:
                        break;
                    case Etat.STATE_GETPOCKET:
                        break;
                    case Etat.STATE_CHECKTRASHDOOR:
                        break;
                    case Etat.STATE_CHECKLOWERSENSOR:
                        break;
                    case Etat.STATE_GETOPTION:
                        break;
                    case Etat.STATE_ACCEPTLIMIT:
                        break;
                    case Etat.STATE_COMMSREVISION:
                        break;
                    case Etat.STATE_STOP:
                    {

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
                if (ProductCode != "BV")
                {
                    State = Etat.STATE_GETCREDITBUFFER;
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                    CDevicesManage.Log.Info("Lecture de l'ensemble des variables du {0}", DeviceAddress);
                    if(IsCmdccTalkSended(DeviceAddress, CccTalk.Header.REQUESTVARIABLESET, 0, null, bufferIn))
                    {
                        CDevicesManage.Log.Info("variableSetToRead");
                    }
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return bufferIn;
            }
        }

        /// <summary>
        /// Tâche de la machine d'état du monnayeur.
        /// </summary>
        protected override void Task()
        {
            CDevicesManage.Log.Debug("Tâche de lecture des évenements concernant du {0}", DeviceAddress);
            while(true)
            {
                try
                {
                    mutexCCTalk.WaitOne();
                    CheckState();
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
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), "Exception dans le thread du monnayeur : " + E.Message, E.StackTrace);
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
                CDevicesManage.Log.Info("Instancation de la classe CCoinValidator.");
                DeviceAddress = DefaultDevicesAddress.CoinAcceptor;
                if (!(IsPresent = SimplePoll))
                {
                    Thread.Sleep(100);
                    ResetDevice();
                    IsPresent = SimplePoll;
                }
                state = Etat.STATE_STOP;
                if (IsPresent)
                {
                    canaux = new CCanal[numChannel];
                    for (byte i = 0; i < numChannel; i++)
                    {
                        canaux[i] = new CCanal((byte)(i + 1), this);
                        canaux[i].coinId.GetCoinId();
                    }
                    errorCV = new CErroCV();
                    ResetDevice();
                    PollingDelay = PollingPriority * 2 / 3;
                    CVTask = new Thread(Task);
                    state = Etat.STATE_INIT;
                    CVTask.Start();
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }


    }
}