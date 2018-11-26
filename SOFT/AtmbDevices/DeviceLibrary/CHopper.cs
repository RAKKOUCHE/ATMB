using System;
using System.Threading;

namespace DeviceLibrary
{
    public partial class CHopper : CccTalk
    {
        /// <summary>
        /// Nombre maximum de hoppers
        /// </summary>
        public const byte maxHopper = 8;

        /// <summary>
        /// Delai en ms entre chaque pollinng du hopper
        /// </summary>
        private const int pollDelayHopper = 150;

        /// <summary>
        /// Delai en secondes entre 2 vérication de niveau
        /// </summary>
        private const int polllDelayLevel = 1;

        /// <summary>
        /// 
        /// </summary>
        public enum Etat : byte
        {
            RESET,
            DISPENSE,
            DISPENSEINPROGRESS,
            ENDDISPENSE,
            IDLE = 0XFF,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum RegistrePos : byte
        {
            MAXCURRENTEXCEEDED = 0X01,
            OPTOSHORTCIRCUIT = 0X01,
            TOOCCURED = 0X02,
            SINGLEPAYOUT = 0X02,
            MOTORREVERSED = 0X04,
            CHECKSUMA = 0X04,
            OPTOPATHBLOCKED = 0X08,
            CHECKSUMB = 0X08,
            OPTOSHORTCIRCUITIDLE = 0X10,
            CHECKSUMC = 0X10,
            OPTOBLOCKEDPERMANENTLY = 0X20,
            CHECKSUMD = 0X20,
            POWERUP = 0X40,
            POWERFAIL = 0X40,
            DISABLED = 0X80,
            PINNUMBER = 0X80,
        }

        /// <summary>
        /// 
        /// </summary>
        private enum LevelMask : byte
        {
            /// <summary>
            /// Mask indiquant si le niveau base est atteint.
            /// </summary>
            LOLEVELREACHED = 0x01,

            /// <summary>
            /// Mask indiquant si le niveau haut est atteint.
            /// </summary>
            HILEVEREACHED = 0x02,

            /// <summary>
            /// Mask indiquant si le niveau bas est implémenté.
            /// </summary>
            LOLEVELIMPLEMENTED = 0x10,

            /// <summary>
            /// Mask indiquant si le niveau haut est implémenté.
            /// </summary>
            HILEVELIMPLEMENTED = 0x20,
        }

        /// <summary>
        /// Class contenant les résultats du vidage du hopper.
        /// </summary>
        public class CEmptyCount
        {
            public long counter;
            public long amountCounter;
            public long delta;
            public long amountDelta;
        }

        /// <summary>
        /// 
        /// </summary>
        public class CHopperCoinId
        {
            public string CountryCode;
            public long ValeurCent;
            public char Issue;
        }

        /// <summary>
        /// Objet contenant l'identification des pièces contenus.
        /// </summary>
        private CHopperCoinId hopperCoinID;


        /// <summary>
        /// Indique si le hopper a été vidé
        /// </summary>
        public bool isEmptied;

        /// <summary>
        /// Adresse de base des hoppers.
        /// </summary>
        /// <remarks>
        /// Les numéros des hoppers commencent à 1 et la première adresse est 3
        /// donc l'adresse d'un hopper s'obtient par : 
        /// Numéro du Hopper + (première adresse(3) - 1) : (Adresse de base : 2)
        /// </remarks>
        public const byte AddressBaseHoper = 2;

        /// <summary>
        /// Objet contenant l'ensemble des variables du hopper
        /// </summary>
        public CHopperVariableSet variables;

        /// <summary>
        /// Objet contenant le status de la distribution en cours.
        /// </summary>
        public CHopperStatus dispenseStatus;

        private uint coinValue;
        /// <summary>
        /// Valeur de la pièce distribuée par le hopper.
        /// </summary>
        public uint CoinValue
        {
            get => coinValue;
            set => coinValue = value;
        }

        /// <summary>
        /// Compteur du nombre de pièce dans le hopper
        /// </summary>
        public long CoinsInHopper
        {
            get => counters.coinsInHopper[Number - 1];
            set => counters.coinsInHopper[Number - 1] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public long AmountInHopper
        {
            get => counters.amountInHopper[Number - 1];
            set => counters.amountInHopper[Number - 1] = value;
        }

        /// <summary>
        /// Compteur de pièces distribuées
        /// </summary>
        public long CoinsOut
        {
            get => counters.coinsOut[Number - 1];
            set => counters.coinsOut[Number - 1] = value;
        }

        /// <summary>
        /// Montant distribués
        /// </summary>
        public long AmountOut
        {
            get => counters.amountCoinOut[Number - 1];
            set => counters.amountCoinOut[Number - 1] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public long CoinsLoadedInHopper
        {
            get => counters.coinsLoadedInHopper[Number - 1];
            set => counters.coinsLoadedInHopper[Number - 1] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public long AmountLoadedInHopper
        {
            get => counters.amountLoadedInHopper[Number - 1];
            set => counters.amountLoadedInHopper[Number - 1] = value;
        }

        /// <summary>
        /// Numéro d'ordre du hopper
        /// </summary>
        public byte Number;

        /// <summary>
        /// 
        /// </summary>
        public CMemoryStorage memoryStorage;

        private Etat state;
        /// <summary>
        /// 
        /// </summary>
        public Etat State
        {
            get => state;
            set => state = value;
        }

        /// <summary>
        /// Clé de chiffrement pour la distribution
        /// </summary>
        private byte[] cipherKey;

        /// <summary>
        /// Compteur de vidage;
        /// </summary>
        public CEmptyCount emptyCount;

        /// <summary>
        /// 
        /// </summary>
        private bool isMaxCurrentExceeded;

        /// <summary>
        /// 
        /// </summary>
        public bool IsMaxCurrentExceeded
        {
            get
            {
                TestHopper();
                return isMaxCurrentExceeded;
            }
            set => isMaxCurrentExceeded = value;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool isTOOccured;
        public bool IsTOOccured
        {
            get
            {
                TestHopper();
                return isTOOccured;
            }
            set => isTOOccured = value;
        }

        private bool isMotorReversed;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMotorReversed
        {
            get
            {
                TestHopper();
                return isMotorReversed;
            }
            set => isMotorReversed = value;
        }

        private bool isPathBlocked;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPathBlocked
        {
            get
            {
                TestHopper();
                return isPathBlocked;
            }
            set => isPathBlocked = value;
        }

        private bool isShortCircuit;
        /// <summary>
        /// 
        /// </summary>
        public bool IsShortCircuit
        {
            get
            {
                TestHopper();
                return isShortCircuit;
            }
            set => isShortCircuit = value;
        }

        private bool isOptoPermanentlyBlocked;
        /// <summary>
        /// 
        /// </summary>
        public bool IsOptoPermanentlyBlocked
        {
            get
            {
                TestHopper();
                return isOptoPermanentlyBlocked;
            }
            set => isOptoPermanentlyBlocked = value;
        }

        private bool isPowerUpDetected;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPowerUpDetected
        {
            get
            {
                TestHopper();
                return isPowerUpDetected;
            }
            set => isPowerUpDetected = value;
        }

        private bool isPayoutDisabled;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPayoutDisabled

        {
            get
            {
                TestHopper();
                return isPayoutDisabled;
            }
            set => isPayoutDisabled = value;
        }

        private bool isOptoShorCircuit;
        /// <summary>
        /// 
        /// </summary>
        public bool IsOptoShorCircuit
        {
            get
            {
                TestHopper();
                return isOptoShorCircuit;
            }
            set => isOptoShorCircuit = value;
        }

        private bool isSingleCoinMode;
        /// <summary>
        /// 
        /// </summary>
        public bool IsSingleCoinMode
        {
            get
            {
                TestHopper();
                return isSingleCoinMode;
            }
            set => isSingleCoinMode = value;
        }

        private bool isCheckSumAError;
        /// <summary>
        /// 
        /// </summary>
        public bool IsCheckSumAError
        {
            get
            {
                TestHopper();
                return isCheckSumAError;
            }
            set => isCheckSumAError = value;
        }

        private bool isCheckSumBError;
        /// <summary>
        /// 
        /// </summary>
        public bool IsCheckSumBError
        {
            get
            {
                TestHopper();
                return isCheckSumBError;
            }
            set => isCheckSumBError = value;
        }

        private bool isCheckSumCError;
        /// <summary>
        /// 
        /// </summary>
        public bool IsCheckSumCError
        {
            get
            {
                TestHopper();
                return isCheckSumCError;
            }
            set => isCheckSumCError = value;
        }

        private bool isCheckSumDError;
        /// <summary>
        /// 
        /// </summary>
        public bool IsCheckSumDError
        {
            get
            {
                TestHopper();
                return isCheckSumDError;
            }
            set => isCheckSumDError = value;
        }

        private bool isPowerFailMemoryWrite;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPowerFailMemoryWrite
        {
            get
            {
                TestHopper();
                return isPowerFailMemoryWrite;
            }
            set => isPowerFailMemoryWrite = value;
        }

        private bool isPINEnabled;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPINEnabled
        {
            get
            {
                TestHopper();
                return isPINEnabled;
            }
            set => isPINEnabled = value;
        }

        private byte coinsToDistribute;
        /// <summary>
        /// 
        /// </summary>
        public byte CoinsToDistribute
        {
            get => coinsToDistribute;
            set => coinsToDistribute = value;
        }

        private uint levelFullSoft;
        /// <summary>
        /// 
        /// </summary>
        public uint LevelFullSoft
        {
            get => levelFullSoft;
            set => levelFullSoft = value;
        }

        private uint levelHISoft;
        /// <summary>
        /// 
        /// </summary>
        public uint LevelHISoft
        {
            get => levelHISoft;
            set => levelHISoft = value;
        }

        private uint levelLOSoft;
        /// <summary>
        /// 
        /// </summary>
        public uint LevelLOSoft
        {
            get => levelLOSoft;
            set => levelLOSoft = value;
        }

        private uint levelEmptySoft;
        /// <summary>
        /// 
        /// </summary>
        public uint LevelEmptySoft
        {
            get => levelEmptySoft;
            set => levelEmptySoft = value;
        }

        private uint defaultFilling;
        /// <summary>
        /// Nombre de pièces par défaut pour un remplissage. 
        /// </summary>
        public uint DefaultFilling
        {
            get => defaultFilling;
            set => defaultFilling = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Thread HTask;

        private bool isDispensed;
        /// <summary>
        /// Indique la fin d'une distribution
        /// </summary>
        public bool IsDispensed
        {
            get => isDispensed;
            set => isDispensed = value;
        }

        private bool isCritical;
        /// <summary>
        /// 
        /// </summary>
        public bool IsCritical
        {
            get => isCritical;
            set => isCritical = value;
        }

        /// <summary>
        /// 
        /// </summary>
        private static int delaypollLevel;

        /// <summary>
        /// 
        /// </summary>
        private static Mutex mutexLevel = new Mutex();

        /*--------------------------------------------------------------*/

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="hopperNumber">Numéro du hopper</param>
        public CHopper(byte hopperNumber)
        {
            CDevicesManage.Log.Info("Instanciation  du hopper {0}", hopperNumber);
            DeviceAddress = (DefaultDevicesAddress)(hopperNumber + AddressBaseHoper);
            Number = hopperNumber;
            if (!(IsPresent = SimplePoll))
            {
                Thread.Sleep(200);
                ResetDevice();
                IsPresent = SimplePoll;
            }
            if (IsPresent)
            {
                deviceLevel = new CLevel();
                variables = new CHopperVariableSet(this);
                dispenseStatus = new CHopperStatus(this);
                hopperCoinID = new CHopperCoinId();

                emptyCount = new CEmptyCount();
                cipherKey = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                CoinsToDistribute = 0;
            }
        }

        /// <summary>
        /// Renvoi les informations sur les niveaux des hoppers
        /// </summary>
        /// <returns></returns>     
        private byte LevelStatus
        {
            get
            {
                byte result = 0;
                try
                {
                    CDevicesManage.Log.Info("Lecture des informations sur les niveaux du hopper {0}", Number);
                    result = GetByte(Header.REQUESTHIGHLOWSTATUS);
                    if (((result & (byte)LevelMask.LOLEVELREACHED) > 0) && ((result & (byte)LevelMask.HILEVEREACHED) > 0))
                    {
                        throw new Exception(string.Format("Erreur sur la detection des niveaux du hopper {0}", Number));
                    }
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Indique si la détection du niveau haut est implémentée.
        /// </summary>
        /// <returns></returns>
        public bool IsHighLevelImplemented
        {
            get
            {
                bool result;
                CDevicesManage.Log.Info("Niveau haut implémenté sur le hopper {0} : {1} : ", Number, result = ((LevelStatus & (byte)LevelMask.HILEVELIMPLEMENTED) > 0));
                return result;
            }
        }

        /// <summary>
        /// Indique si la détection du niveau haut est implémentée.
        /// </summary>
        /// <returns></returns>
        public bool IsLowLevelImplemented
        {
            get
            {
                bool result;
                CDevicesManage.Log.Info("Niveau bas implémenté sur le hopper {0} : {1} : ", Number, result = (LevelStatus & (byte)LevelMask.LOLEVELIMPLEMENTED) > 0);
                return result;
            }
        }

        /// <summary>
        /// Indique si le niveau est atteint
        /// </summary>
        public bool IsHighLevelReached
        {
            get
            {
                bool result;
                CDevicesManage.Log.Info("Niveau haut atteint sur le hopper {0} : {1} : ", Number, result = IsHighLevelImplemented && (LevelStatus & (byte)LevelMask.HILEVEREACHED) > 0);
                return result;
            }
        }

        /// <summary>
        /// Indique si le niveau est atteint
        /// </summary>
        public bool IsLowLevelReached
        {
            get
            {
                bool result;
                CDevicesManage.Log.Info("Niveau bas atteint sur le hopper {0} : {1} : ", Number, result = IsLowLevelImplemented && (LevelStatus & (byte)LevelMask.LOLEVELREACHED) > 0);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte EmergecyStop
        {
            get
            {
                CDevicesManage.Log.Info("Arrêt d'urgence du hopper {0}", Number);
                return GetByte(Header.EMERGENCYSTOP);
            }
        }

        /// <summary>
        /// Identification interne du hopper
        /// </summary>
        /// <remarks>N'est pas pris en compte par la dll</remarks>
        public CHopperCoinId CoinId
        {
            get
            {
                CHopperCoinId result = new CHopperCoinId();
                try
                {
                    byte[] bufferIn = { 46, 46, 46, 46, 46, 46 };
                    CDevicesManage.Log.Info("Lecture de l'identification de la pièce traitée par le hopper {0}", Number);
                    if (IsCmdccTalkSended(DeviceAddress, Header.REQUESTHOPPERCOIN, 0, null, bufferIn))
                    {
                        result.CountryCode = bufferIn[0].ToString() + bufferIn[1].ToString();
                        if ((bufferIn[2] >= 0x30) && (bufferIn[2] >= 0x30) && (bufferIn[2] >= 0x30))
                        {
                            result.ValeurCent = (byte)(((bufferIn[2] - 0x30) * 100) + ((bufferIn[3] - 0x30) * 10) + (bufferIn[4] - 0x30));
                        }
                        else
                        {
                            result.ValeurCent = 0;
                        }
                        result.Issue = (char)bufferIn[5];
                        CDevicesManage.Log.Debug("Le code pays de la pièce traitée par le hopper {0} est {1}, la valeur est de {2:C2}, la version est {3} ", Number, result.CountryCode, (decimal)result.ValeurCent / 100, result.Issue);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetResetableCounter()
        {
            int result = 0;
            try
            {
                CDevicesManage.Log.Info("Le nombre de pièces distribuées par le {0} est {1}", Number, result);
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return result;
        }


        /// <summary>
        /// Autorise la distribution sur le hopper.
        /// </summary>
        private void EnableHopper()
        {
            try
            {
                byte[] enable = { 165 };
                CDevicesManage.Log.Info("Autorise la distribution du {0}", DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, Header.ENABLEHOPPER, (byte)enable.Length, enable, null))
                {
                    throw new Exception(string.Format("Impossible d'autorisé la distribution sur le {0}", DeviceAddress));
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Desactive l'autorisation de distribution du hopper
        /// </summary>
        private void DisableHopper()
        {
            try
            {
                byte[] disable = { 0 };
                CDevicesManage.Log.Info("Desactive l'autorisation de distribution du {0}", DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, Header.ENABLEHOPPER, (byte)disable.Length, disable, null))
                {
                    throw new Exception(string.Format("Impossible de désactiver le {0}", DeviceAddress));
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void PumpRNG()
        {
            try
            {
                CDevicesManage.Log.Info("Random number generator pumb");
                Random rnd = new Random();
                byte[] bufferParam = new byte[8];
                rnd.NextBytes(bufferParam);
                if (!IsCmdccTalkSended(DeviceAddress, Header.PUMPRNG, (byte)bufferParam.Length, bufferParam, null))
                {
                    throw new Exception("Echec d'initialisation de la clé de chiffrement.");
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetCipherKey()
        {
            try
            {
                CDevicesManage.Log.Debug("Récupération de la clé de chiffrement");
                if (!IsCmdccTalkSended(DeviceAddress, Header.REQUESTCIPHERKEY, 0, null, cipherKey))
                {
                    throw new Exception("Impossible de récupérer la clé de chiffrement.");
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public bool DispenseCoins(byte number)
        {
            bool result = false;
            try
            {
                CDevicesManage.Log.Debug("Lancement de la distribution par le hopper {0} de {1} pièces", Number, number);
                byte[] bufferParam = { cipherKey[0], cipherKey[1], cipherKey[2], cipherKey[3], cipherKey[4], cipherKey[5], cipherKey[6], cipherKey[7], number };
                if (!IsCmdccTalkSended(DeviceAddress, Header.DISPENSEHOPPERCOINS, (byte)bufferParam.Length, bufferParam, null))
                {
                    throw new Exception("Impossible de distribuer");
                }
                result = true;
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Lecture des registres du hopper.
        /// </summary>
        private void TestHopper()
        {
            try
            {
                CDevicesManage.Log.Info("Lecture des registres du {0}", DeviceAddress);
                byte[] bufferIn = { 0, 0 };
                if (IsCmdccTalkSended(DeviceAddress, Header.TESTHOPPER, 0, null, bufferIn))
                {
                    IsMaxCurrentExceeded = (bufferIn[0] & (byte)RegistrePos.MAXCURRENTEXCEEDED) > 0;
                    IsTOOccured = (bufferIn[0] & (byte)RegistrePos.TOOCCURED) > 0;
                    IsMotorReversed = (bufferIn[0] & (byte)RegistrePos.MOTORREVERSED) > 0;
                    IsPathBlocked = (bufferIn[0] & (byte)RegistrePos.OPTOPATHBLOCKED) > 0;
                    IsShortCircuit = (bufferIn[0] & (byte)RegistrePos.OPTOSHORTCIRCUITIDLE) > 0;
                    IsOptoPermanentlyBlocked = (bufferIn[0] & (byte)RegistrePos.OPTOBLOCKEDPERMANENTLY) > 0;
                    IsPowerUpDetected = (bufferIn[0] & (byte)RegistrePos.MAXCURRENTEXCEEDED) > 0;
                    IsPayoutDisabled = (bufferIn[0] & (byte)RegistrePos.DISABLED) > 0;

                    IsOptoShorCircuit = (bufferIn[1] & (byte)RegistrePos.OPTOSHORTCIRCUIT) > 0;
                    IsSingleCoinMode = (bufferIn[1] & (byte)RegistrePos.SINGLEPAYOUT) > 0;
                    IsCheckSumAError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMA) > 0;
                    IsCheckSumBError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMB) > 0;
                    IsCheckSumCError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMC) > 0;
                    IsCheckSumDError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMD) > 0;
                    IsPowerFailMemoryWrite = (bufferIn[1] & (byte)RegistrePos.POWERFAIL) > 0;
                    IsPINEnabled = (bufferIn[1] & (byte)RegistrePos.PINNUMBER) > 0;
                }
                else
                {
                    throw new Exception(string.Format("Impossible de lire les refistres du hopper {0}", DeviceAddress));
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Initialisation du hopper
        /// </summary>
        public void Init()
        {
            try
            {
                State = Etat.RESET;
                if (IsPresent)
                {
                    memoryStorage = new CMemoryStorage(this);
                    CDevicesManage.Log.Info("Categorie d'équipement du Hopper {0} {1}", Number, EquipementCategory);
                    CDevicesManage.Log.Info(OptoStates != 0 ? "Au moins un optocoupleur est occupé" : "Les optocoupleur sont libres");
                    CDevicesManage.Log.Info("Le courant maximum auorisé pour le hopper {0} est de {1}", DeviceAddress, variables.CurrentLimit);
                    CDevicesManage.Log.Info("Le délai pour arrêter le motor du hopper {0} est de {1}", DeviceAddress, variables.MotorStopDelay);

                    CDevicesManage.Log.Info("Le délai maximum de vérification pour la distribution de pièce du hopper {0} est de {1}", DeviceAddress, variables.PayoutDelayTO);
                    CDevicesManage.Log.Info("Le courant maximum utilisé par le hopper {0} est de {1}", DeviceAddress, variables.Maxcurrent);
                    double tension = variables.Tension;
                    if ((tension > 26) || (tension < 19))
                    {
                        CDevicesManage.Log.Error("Tension d'alimentation anormale.");
                    }
                    CDevicesManage.Log.Info("La tension sur le hopper {0} est de {1}", DeviceAddress, tension);
                    byte connectorAddress = variables.ConnectorAddress;
                    if ((connectorAddress + 1 + AddressBaseHoper) != (byte)DeviceAddress)
                    {
                        CDevicesManage.Log.Error("Une différence d'adresse existe entre l'adresse logiciel et l'adresse physique du connecteur pour le hopper {0}, l'adresse physique du connecteur est : {1}", DeviceAddress, connectorAddress);
                    }
                    else
                    {
                        CDevicesManage.Log.Info("L'adresse physique du hopper est : {0} ", connectorAddress + 1 + AddressBaseHoper);
                    }
                    CDevicesManage.Log.Info("Le numéro de série du Hopper {0} {1} ", Number, SerialNumber);
                    hopperCoinID = CoinId;
                    CDevicesManage.Log.Info("Identification des pièces : Pays : {0}, Valeur : {1:C2}, version {2}", hopperCoinID.CountryCode, (decimal)hopperCoinID.ValeurCent / 100, hopperCoinID.Issue);
                    CDevicesManage.Log.Info("Prise en compte du niveau bas : {0}", IsLowLevelImplemented);
                    CDevicesManage.Log.Info("Prise en compte du niveau haut : {0} ", IsHighLevelImplemented);
                    CDevicesManage.Log.Info("Nivau bas atteint : {0}", IsLowLevelReached);
                    CDevicesManage.Log.Info("Niveau haut atteint : {0}", IsHighLevelReached);
                    CDevicesManage.Log.Info("Le type de mémoire du hopper {0} est : {1}", Number, memoryStorage.MemoryType);
                    CDevicesManage.Log.Info("Le nombre de bytes dans un bloc de lecture du hopper {0} est : {1}", Number, memoryStorage.ReadBytesPerBlock);
                    CDevicesManage.Log.Info("Le nombre de blocs pouvant être lus du hopper {0} est : {1}", Number, memoryStorage.ReadBlocks);
                    CDevicesManage.Log.Info("Le nombre de bytes dans un bloc d'écriture du hopper {0} est : {1}", Number, memoryStorage.WriteBytesPerBlock);
                    CDevicesManage.Log.Info("Le nombre de blocs pouvant être dans le hopper {0} est : {1}", Number, memoryStorage.WriteBlocks);
                    CDevicesManage.Log.Info("Il reste {0} pièce(s) à distribuer par le {1}", dispenseStatus.CoinsRemaining, DeviceAddress);
                    EnableHopper();
                    if ((CoinsToDistribute > 0) || ((CoinsToDistribute = dispenseStatus.CoinsRemaining) > 0))
                    {
                        State = Etat.DISPENSE;
                    };
                    HTask = new Thread(TaskHopper);
                    HTask.Start();
                }
                else
                {
                    throw new Exception(string.Format("Le hopper {0} n'est pas opérationel et ne peut donc pas être initialisé. ", Number));
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Véfication des niveaux du hopper.
        /// </summary>
        public void CheckLevel(object state = null)
        {
            mutexLevel.WaitOne();
            try
            {
                if (IsPresent)
                {
                    if (!deviceLevel.isHardLevelChanged && (deviceLevel.hardLevel != CLevel.HardLevel.OK) && !IsHighLevelReached && !IsLowLevelReached)
                    {
                        deviceLevel.isHardLevelChanged = true;
                        deviceLevel.hardLevel = CLevel.HardLevel.OK;
                    }
                    if (!deviceLevel.isHardLevelChanged && (deviceLevel.hardLevel != CLevel.HardLevel.VIDE) && IsLowLevelReached)
                    {
                        deviceLevel.isHardLevelChanged = true;
                        deviceLevel.hardLevel = CLevel.HardLevel.VIDE;
                    }
                    if (!deviceLevel.isHardLevelChanged && (deviceLevel.hardLevel != CLevel.HardLevel.PLEIN) && IsHighLevelReached)
                    {
                        deviceLevel.isHardLevelChanged = true;
                        deviceLevel.hardLevel = CLevel.HardLevel.PLEIN;
                    }
                    if (!deviceLevel.isSoftLevelChanged && (deviceLevel.softLevel != CLevel.SoftLevel.OK) && (counters.coinsInHopper[Number - 1] > LevelLOSoft) &&
                        (counters.coinsInHopper[Number - 1] < LevelHISoft))
                    {
                        deviceLevel.isSoftLevelChanged = true;
                        deviceLevel.softLevel = CLevel.SoftLevel.OK;
                    }
                    if (!deviceLevel.isSoftLevelChanged && (deviceLevel.softLevel != CLevel.SoftLevel.VIDE) && (counters.coinsInHopper[Number - 1] < LevelEmptySoft))
                    {
                        deviceLevel.isSoftLevelChanged = true;
                        deviceLevel.softLevel = CLevel.SoftLevel.VIDE;
                    }
                    if (!deviceLevel.isSoftLevelChanged && (deviceLevel.softLevel != CLevel.SoftLevel.BAS) && (counters.coinsInHopper[Number - 1] > LevelEmptySoft) &&
                            (counters.coinsInHopper[Number - 1] < LevelLOSoft))
                    {
                        deviceLevel.isSoftLevelChanged = true;
                        deviceLevel.softLevel = CLevel.SoftLevel.BAS;
                    }
                    if (!deviceLevel.isSoftLevelChanged && (deviceLevel.softLevel != CLevel.SoftLevel.HAUT) && (counters.coinsInHopper[Number - 1] > LevelHISoft) &&
                        (counters.coinsInHopper[Number - 1] < LevelFullSoft))
                    {
                        deviceLevel.isSoftLevelChanged = true;
                        deviceLevel.softLevel = CLevel.SoftLevel.HAUT;
                    }
                    if (!deviceLevel.isSoftLevelChanged && (deviceLevel.softLevel != CLevel.SoftLevel.PLEIN) && (LevelFullSoft < counters.coinsInHopper[Number - 1]))
                    {
                        deviceLevel.isSoftLevelChanged = true;
                        deviceLevel.softLevel = CLevel.SoftLevel.PLEIN;
                    }
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            mutexLevel.ReleaseMutex();
        }

        /// <summary>
        /// Demande le vidage du hopper;
        /// </summary>
        public void Empty()
        {
            try
            {
                emptyCount.delta = CoinsInHopper;
                emptyCount.amountDelta = AmountInHopper;
                emptyCount.counter = 0;
                emptyCount.amountCounter = 0;
                do
                {
                    CoinsToDistribute = 250;
                    State = Etat.DISPENSE;
                    while (State != Etat.IDLE) ;
                    emptyCount.counter += dispenseStatus.CoinsPaid;
                    emptyCount.amountCounter = emptyCount.counter * CoinValue;
                    emptyCount.delta -= dispenseStatus.CoinsPaid;
                    emptyCount.amountDelta -= emptyCount.delta * CoinValue;
                } while (!IsTOOccured);
                SubCounters(emptyCount.counter);
                counters.amountInHopper[Number - 1] = counters.coinsInHopper[Number - 1] = 0;
                counterSerializer.Serialize(countersFile, counters);
                CheckLevel();
                isEmptied = true;
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Polling de la machine d'état des hoppers.
        /// </summary>
        public void CheckState()
        {
            switch (State)
            {
                case Etat.RESET:
                {
                    delaypollLevel = polllDelayLevel * 1000;
                    deviceLevel.ID = ToString();
                    CheckLevel();
                    isEmptied =
                    isDispensed = false;
                    State = Etat.IDLE;
                    break;
                }
                case Etat.DISPENSE:
                {
                    if (coinsToDistribute > 0)
                    {
                        IsDispensed = false;
                        if (IsMaxCurrentExceeded || isMotorReversed || isPathBlocked || isOptoPermanentlyBlocked ||
                            isOptoShorCircuit || isCheckSumAError || isCheckSumBError || isCheckSumCError || isCheckSumDError ||
                            isPowerFailMemoryWrite)
                        {
                            ResetDevice();
                            Init();
                            //TODO réenregister la valeur des paramètres
                        }
                        dispenseStatus.dispensedResult.CoinToDispense = coinsToDistribute;
                        dispenseStatus.dispensedResult.AmountToDispense = (int)(coinsToDistribute * CoinValue);
                        EnableHopper();
                        PumpRNG();
                        GetCipherKey();
                        DispenseCoins(CoinsToDistribute);
                        State = Etat.DISPENSEINPROGRESS;
                    }
                    else
                    {
                        CDevicesManage.Log.Debug(string.Format("Pas de pièces à distribuer pour le {0}", DeviceAddress));
                    }
                    break;
                }
                case Etat.DISPENSEINPROGRESS:
                {
                    CheckLevel();
                    if (dispenseStatus.CoinsRemaining == 0)
                    {
                        if (IsTOOccured)
                        {
                            CoinsInHopper = 0;
                            AmountInHopper = 0;
                        }
                        State = Etat.ENDDISPENSE;
                    }
                    break;
                }
                case Etat.ENDDISPENSE:
                {
                    //Attention ici seul 
                    if (IsMaxCurrentExceeded || isTOOccured || isMotorReversed || isPathBlocked || isOptoPermanentlyBlocked ||
                        isOptoShorCircuit || isCheckSumAError || isCheckSumBError || isCheckSumCError || isCheckSumDError ||
                        isPowerFailMemoryWrite)
                    {
                        //TODO message indiquant l'erreur
                    }
                    if (CoinsToDistribute != dispenseStatus.CoinsPaid)
                    {
                        //TODO indiquant l'erreur
                    }
                    IsDispensed = true;
                    SubCounters(dispenseStatus.CoinsPaid);
                    CoinsToDistribute = 0;
                    State = Etat.IDLE;
                    break;
                }
                case Etat.IDLE:
                {
                    if (--delaypollLevel <= 0)
                    {
                        delaypollLevel = polllDelayLevel * 1000;
                        CheckLevel();
                    }
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Hopper {Number}";
        }


        /// <summary>
        /// Provoque la distribution parale hopper en utilisant la machine d'état
        /// </summary>
        /// <param name="numberToDispense">Nombre de token à distribuer</param>
        public void Distribute(byte numberToDispense)
        {
            CoinsToDistribute = numberToDispense;
            State = Etat.DISPENSE;
            while (State != Etat.IDLE) ;
            SubCounters(dispenseStatus.CoinsPaid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CoinsNumber"></param>
        public void SubCounters(long CoinsNumber)
        {
            counters.totalAmountCash -= CoinsNumber * CoinValue;
            counters.totalAmountCashOut += CoinsNumber * CoinValue;
            CoinsInHopper -= CoinsNumber;
            AmountInHopper = CoinsInHopper * CoinValue;
            CoinsOut += CoinsNumber;
            AmountOut = CoinsOut * coinValue;
            counters.SaveCounters();
            CheckLevel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CoinsNumber"></param>
        public void LoadHopper(long CoinsNumber)
        {
            counters.totalAmountCash += CoinsNumber * CoinValue;
            CoinsInHopper += CoinsNumber;
            AmountInHopper = CoinsInHopper * CoinValue;
            CoinsLoadedInHopper += CoinsNumber;
            AmountLoadedInHopper += CoinsLoadedInHopper * CoinValue;
            counters.SaveCounters();
            CheckLevel();
        }


        /// <summary>
        /// 
        /// </summary>
        public void TaskHopper()
        {
            while (true)
            {
                try
                {
                    mutexCCTalk.WaitOne();
                    if (IsPresent)
                    {
                        try
                        {
                            CheckState();
                        }
                        catch (Exception E)
                        {
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                        }
                    }
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                try
                {
                    mutexCCTalk.ReleaseMutex();

                }
                catch (Exception)
                {
                    throw;
                }
                Thread.Sleep(pollDelayHopper);
            }
        }
    }
}

