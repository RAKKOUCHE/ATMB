/// \file CHopper.cs
/// \brief Fichier contenant la classe CHopper.
/// \date 28 11 2018
/// \version 1.1
/// Modification du traitements des erreurs du hopper.
/// Création de sous fichiers contenant les classes.
/// \author Rachid AKKOUCHE

using System;
using System.Threading;

namespace DeviceLibrary
{
    /// <summary>
    /// Classe des hoppers
    /// </summary>
    public partial class CHopper : CccTalk
    {
        /// <summary>
        /// Nombre maximum de hoppers
        /// </summary>
        public const byte maxHopper = 8;

        /// <summary>
        /// Delai en ms entre chaque pollinng du hopper
        /// </summary>
        private const int pollDelayHopper = 100;

        /// <summary>
        /// Delai en secondes entre 2 vérication de niveau
        /// </summary>
        private const int polllDelayLevel = 1;

        /// <summary>
        /// Identification de la dénomination de la pièce gerée par le hopper
        /// </summary>
        public class CHopperCoinId
        {
            /// <summary>
            /// Région sur 2 caractères
            /// </summary>
            public string CountryCode;

            /// <summary>
            /// Valeur en centimes.
            /// </summary>
            public long ValeurCent;

            /// <summary>
            /// Révision du data set
            /// </summary>
            public char Issue;
        }

        /// <summary>
        /// Class contenant les informations du message envoyé lors du changement d'état des niveaux soft.
        /// </summary>
        public class CSoftLevelData
        {
            /// <summary>
            /// Nom du hopper
            /// </summary>
            public string nameOfHopper;

            /// <summary>
            /// Indique si le hopper est critique.
            /// </summary>
            public bool isHCritical;

            /// <summary>
            /// Indique le nombre de pièces dans le hopper.
            /// </summary>
            public long coinsNumber;

            /// <summary>
            /// Seuil atteint.
            /// </summary>
            public CLevel.SoftLevel level;
        }

        /// <summary>
        /// Class contenant les informations du message envoyé lors du changement d'état des niveaux soft.
        /// </summary>
        public class CHardLevelData
        {
            /// <summary>
            /// Nom du hopper.
            /// </summary>
            public string nameOfHopper;

            /// <summary>
            /// Indique si le hopper est critique.
            /// </summary>
            public bool isHCritical;

            /// <summary>
            /// Indique le nombre de pièces dans le hopper.
            /// </summary>
            public long coinsNumber;

            /// <summary>
            /// Seuil atteint.
            /// </summary>
            public CLevel.HardLevel level;
        }

        /// <summary>
        /// Instance de la class CerrorHopper
        /// </summary>
        public CHopperError errorHopper;

        /// <summary>
        /// Objet contenant l'identification des pièces contenus.
        /// </summary>
        private CHopperCoinId hopperCoinID;

        /// <summary>
        /// Chaine d'identification du hopper.
        /// </summary>
        public string name;

        /// <summary>
        /// Flag indiquant qu'un vidage est en cours.
        /// </summary>
        public bool isEmptyingInProgress;

        ///// <summary>
        ///// Indique si le hopper a été vidé
        ///// </summary>
        //public bool isEmptied;

        private bool isInitialized;

        /// <summary>
        /// Indique si le Hopper est initialisé.
        /// </summary>
        public bool IsInitialized
        {
            get => isInitialized;
            set => isInitialized = value;
        }

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

        /// <summary>
        /// Boolean indiquant que le hopper est operationel.
        /// </summary>
        private bool isOnError;

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
        /// Montant contenu dans le hopper.
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
        /// Nombre de pièces chargées dans le hopper
        /// </summary>
        public long CoinsLoadedInHopper
        {
            get => counters.coinsLoadedInHopper[Number - 1];
            set => counters.coinsLoadedInHopper[Number - 1] = value;
        }

        /// <summary>
        /// Montant chargé dans le hopper.
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
        /// Informations sur le maintien des donneés en mémoires
        /// </summary>
        public CMemoryStorage memoryStorage;

        private Etat state;

        /// <summary>
        /// Etat de la machine d'état des hoppers
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

        private bool isMaxCurrentExceeded;

        /// <summary>
        /// Bolean indiquant qu'un dépassement du courant maximun a été atteint.
        /// </summary>
        public bool IsMaxCurrentExceeded
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isMaxCurrentExceeded;
            }
        }

        private bool isTOOccured;

        /// <summary>
        /// Bolean indiquant qu'un dépassement du temps alloué pour l'éjection d'une pièce est survenu.
        /// </summary>
        public bool IsTOOccured
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isTOOccured;
            }
        }

        private bool isMotorReversed;

        /// <summary>
        /// Bolean indiquant que suite à une surconsomation la rotation du moteur a été inversée pour corriger un bourrage.
        /// </summary>
        public bool IsMotorReversed
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isMotorReversed;
            }
        }

        private bool isOptoPathBlocked;

        /// <summary>
        /// Bolean indiquant que la sortie est bloquée.
        /// </summary>
        public bool IsOptoPathBlocked
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isOptoPathBlocked;
            }
        }

        private bool isShortCircuit;

        /// <summary>
        /// Bolean indiquant que la sortie est en court-circuit.
        /// </summary>
        public bool IsShortCircuit
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isShortCircuit;
            }
        }

        private bool isOptoPermanentlyBlocked;

        /// <summary>
        /// Bolean indiquant qu'un optocloupleur est bloqué en permanence.
        /// </summary>
        public bool IsOptoPermanentlyBlocked
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isOptoPermanentlyBlocked;
            }
        }

        private bool isPowerUpDetected;

        /// <summary>
        /// Bolean indiquant que le hopper est alimenté et un reset n'est pas encore effectué
        /// </summary>
        public bool IsPowerUpDetected
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isPowerUpDetected;
            }
        }

        private bool isPayoutDisabled;

        /// <summary>
        /// Bolean indiquant que le hopper n'est pas activé
        /// </summary>
        public bool IsPayoutDisabled
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isPayoutDisabled;
            }
        }

        private bool isOptoShorCircuit;

        /// <summary>
        /// Bolean indiquant qu'un optocoupleur est en court-circuit
        /// </summary>
        public bool IsOptoShorCircuit
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isOptoShorCircuit;
            }
        }

        private bool isSingleCoinMode;

        /// <summary>
        /// Bolean indiquant que le hopper est en mode pièce par pièce.
        /// </summary>
        public bool IsSingleCoinMode
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isSingleCoinMode;
            }
        }

        private bool isCheckSumAError;

        /// <summary>
        /// Bolean indiquant qu'une erreur de checksum dans le bloc de données A est survenue.
        /// </summary>
        public bool IsCheckSumAError
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isCheckSumAError;
            }
        }

        private bool isCheckSumBError;

        /// <summary>
        /// Bolean indiquant qu'une erreur de checksum dans le bloc de données B est survenue.
        /// </summary>
        public bool IsCheckSumBError
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isCheckSumBError;
            }
        }

        private bool isCheckSumCError;

        /// <summary>
        /// Bolean indiquant qu'une erreur de checksum dans le bloc de données B est survenue.
        /// </summary>
        public bool IsCheckSumCError
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isCheckSumCError;
            }
        }

        private bool isCheckSumDError;

        /// <summary>
        /// Bolean indiquant qu'une erreur de checksum dans le bloc de données D est survenue.
        /// </summary>
        public bool IsCheckSumDError
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isCheckSumDError;
            }
        }

        private bool isPowerFailMemoryWrite;

        /// <summary>
        /// Bolean indiquant qu'une chute de tension a eu lieu pendant une écriture mémoire.
        /// </summary>
        public bool IsPowerFailMemoryWrite
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isPowerFailMemoryWrite;
            }
        }

        private bool isPINEnabled;

        /// <summary>
        /// Bolean indiquant qu'un code pin est utilisé par le hopper
        /// </summary>
        public bool IsPINEnabled
        {
            get
            {
                errorHopper.Code = TestHopper();
                return isPINEnabled;
            }
        }

        private byte coinsToDistribute;

        /// <summary>
        /// Nombre de pièce à distribuer
        /// </summary>
        public byte CoinsToDistribute
        {
            get => coinsToDistribute;
            set => coinsToDistribute = value;
        }

        private uint levelFullSoft;

        /// <summary>
        /// Nombre maximum de pièces autorisées
        /// </summary>
        public uint LevelFullSoft
        {
            get => levelFullSoft;
            set => levelFullSoft = value;
        }

        private uint levelHISoft;

        /// <summary>
        /// Nombre de pièce pour le niveau d'alerte haut
        /// </summary>
        public uint LevelHISoft
        {
            get => levelHISoft;
            set => levelHISoft = value;
        }

        private uint levelLOSoft;

        /// <summary>
        /// Nombre de pièce pour le niveau d'alerte bas
        /// </summary>
        public uint LevelLOSoft
        {
            get => levelLOSoft;
            set => levelLOSoft = value;
        }

        private uint levelEmptySoft;

        /// <summary>
        /// Nombre minimum de pièces autorisées
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
        /// Thread de la machine d'état du hopper.
        /// </summary>
        public Thread HTask;

        private bool isDispensed;

        /// <summary>
        /// Boolean indiquant la fin d'une distribution
        /// </summary>
        public bool IsDispensed
        {
            get => isDispensed;
            set => isDispensed = value;
        }

        private bool isCritical;

        /// <summary>
        /// Boolean indiquant si le hopper est indispensable au fonctionnement de la borne.
        /// </summary>
        public bool IsCritical
        {
            get => isCritical;
            set => isCritical = value;
        }

        /// <summary>
        /// Délai pour l'interrogation des niveaux lorsque le hopper est au repos.
        /// </summary>
        private static int delaypollLevel;

        /// <summary>
        /// Niveaux des périphériques.
        /// </summary>
        public CLevel deviceLevel;

        /*--------------------------------------------------------------*/

        /// <summary>
        /// Renvoi les informations sur les niveaux des hoppers
        /// </summary>
        /// <returns>Le masque des niveaux du hopper</returns>
        private byte LevelStatus
        {
            get
            {
                byte result = 0;
                try
                {
                    CDevicesManager.Log.Info("Lecture des informations sur les niveaux du hopper {0}", Number);
                    result = GetByte(Header.REQUESTHIGHLOWSTATUS);
                    if (((result & (byte)LevelMask.LOLEVELREACHED) > 0) && ((result & (byte)LevelMask.HILEVEREACHED) > 0))
                    {
                        throw new Exception(string.Format("Erreur sur la detection des niveaux du hopper {0}", Number));
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Boolean indiquant si la détection du niveau haut est implémentée.
        /// </summary>
        public bool IsHighLevelImplemented
        {
            get
            {
                bool result;
                CDevicesManager.Log.Info("Niveau haut implémenté sur le hopper {0} : {1} : ", Number, result = (LevelStatus & (byte)LevelMask.HILEVELIMPLEMENTED) > 0);
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
                CDevicesManager.Log.Info("Niveau bas implémenté sur le hopper {0} : {1} : ", Number, result = (LevelStatus & (byte)LevelMask.LOLEVELIMPLEMENTED) > 0);
                return result;
            }
        }

        /// <summary>
        /// Boolean indiquant si le niveau hardware haut est atteint
        /// </summary>
        public bool IsHighLevelReached
        {
            get
            {
                bool result;
                CDevicesManager.Log.Info("Niveau haut atteint sur le hopper {0} : {1} : ", Number, result = IsHighLevelImplemented && (LevelStatus & (byte)LevelMask.HILEVEREACHED) > 0);
                return result;
            }
        }

        /// <summary>
        /// Boolean indiquant si le niveau hardware bas est atteint
        /// </summary>
        public bool IsLowLevelReached
        {
            get
            {
                bool result;
                CDevicesManager.Log.Info("Niveau bas atteint sur le hopper {0} : {1} : ", Number, result = IsLowLevelImplemented && (LevelStatus & (byte)LevelMask.LOLEVELREACHED) > 0);
                return result;
            }
        }

        /// <summary>
        /// Nombre de byte restant à payer lorsque l'arrêt d'urgence du hopper est activé.
        /// </summary>
        public byte EmergecyStop
        {
            get
            {
                CDevicesManager.Log.Info("Arrêt d'urgence du hopper {0}", Number);
                return GetByte(Header.EMERGENCYSTOP);
            }
        }

        /// <summary>
        /// Lecture des informations concernant la pièce gérée par le hopper
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
                    CDevicesManager.Log.Info("Lecture de l'identification de la pièce traitée par le hopper {0}", Number);
                    if (IsCmdccTalkSended(DeviceAddress, Header.REQUESTHOPPERCOIN, 0, null, bufferIn))
                    {
                        result.CountryCode = ((char)bufferIn[0]).ToString() + ((char)bufferIn[1]).ToString();
                        if ((bufferIn[2] >= 0x30) && (bufferIn[2] >= 0x30) && (bufferIn[2] >= 0x30))
                        {
                            result.ValeurCent = (byte)(((bufferIn[2] - 0x30) * 100) + ((bufferIn[3] - 0x30) * 10) + (bufferIn[4] - 0x30));
                        }
                        else
                        {
                            result.ValeurCent = 0;
                        }
                        result.Issue = (char)bufferIn[5];
                        CDevicesManager.Log.Debug("Le code pays de la pièce traitée par le hopper {0} est {1}, la valeur est de {2:C2}, la version est {3} ", Number, result.CountryCode, (decimal)result.ValeurCent / 100, result.Issue);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Lit les informations du compteur interne du hopper
        /// </summary>
        /// <returns>Nombre de pièces distribuées par le hopper</returns>
        public int GetResetableCounter()
        {
            int result = 0;
            try
            {
                CDevicesManager.Log.Info("Le nombre de pièces distribuées par le {0} est {1}", Number, result);
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                CDevicesManager.Log.Info("Autorise la distribution du {0}", DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, Header.ENABLEHOPPER, (byte)enable.Length, enable, null))
                {
                    throw new Exception(string.Format("Impossible d'autorisé la distribution sur le {0}", DeviceAddress));
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Envoie un nombre aléatoire qui sera utilisé pour le chiffrement.
        /// </summary>
        public void PumpRNG()
        {
            try
            {
                CDevicesManager.Log.Info("Random number generator pumb");
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
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Récupère la clé qui sera utilisé pour le chiffrement de la distribution.
        /// </summary>
        private void GetCipherKey()
        {
            try
            {
                CDevicesManager.Log.Debug("Récupération de la clé de chiffrement");
                if (!IsCmdccTalkSended(DeviceAddress, Header.REQUESTCIPHERKEY, 0, null, cipherKey))
                {
                    throw new Exception("Impossible de récupérer la clé de chiffrement.");
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Commande demandant la distribution des pièces.
        /// </summary>
        /// <param name="number">Nombre de pièces à distribuer</param>
        /// <returns>true si la commande a été envoyée et comprise</returns>
        /// <remarks>En cas d'échec vérifier la procédure de validation précédent la commande.</remarks>
        public bool DispenseCoins(byte number)
        {
            bool result = false;
            try
            {
                CDevicesManager.Log.Debug("Lancement de la distribution par le hopper {0} de {1} pièces", Number, number);
                byte[] bufferParam = { cipherKey[0], cipherKey[1], cipherKey[2], cipherKey[3], cipherKey[4], cipherKey[5], cipherKey[6], cipherKey[7], number };
                if (!IsCmdccTalkSended(DeviceAddress, Header.DISPENSEHOPPERCOINS, (byte)bufferParam.Length, bufferParam, null))
                {
                    throw new Exception("Impossible de distribuer");
                }
                result = true;
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Lecture des registres du hopper.
        /// </summary>
        private HopperError TestHopper()
        {
            HopperError result = HopperError.NON_IDENTIFIEE;
            try
            {
                CDevicesManager.Log.Info("Lecture des registres et des erreurs du {0}", DeviceAddress);
                byte[] bufferIn = { 0, 0 };
                if (IsCmdccTalkSended(DeviceAddress, Header.TESTHOPPER, 0, null, bufferIn))
                {
                    if (isMaxCurrentExceeded = (bufferIn[0] & (byte)RegistrePos.MAXCURRENTEXCEEDED) > 0)
                    {
                        result = HopperError.MAXCURRENTEXCEEDED;
                    };
                    isTOOccured = (bufferIn[0] & (byte)RegistrePos.TOOCCURED) > 0;
                    if (isMotorReversed = (bufferIn[0] & (byte)RegistrePos.MOTORREVERSED) > 0)
                    {
                        result = HopperError.MOTORREVERSED;
                    };
                    if (isOptoPathBlocked = (bufferIn[0] & (byte)RegistrePos.OPTOPATHBLOCKED) > 0)
                    {
                        result = HopperError.OPTOPATHBLOCKED;
                    };
                    if (isShortCircuit = (bufferIn[0] & (byte)RegistrePos.OPTOSHORTCIRCUITIDLE) > 0)
                    {
                        result = HopperError.OPTOSHORTCIRCUITIDLE;
                    };
                    if (isOptoPermanentlyBlocked = (bufferIn[0] & (byte)RegistrePos.OPTOBLOCKEDPERMANENTLY) > 0)
                    {
                        result = HopperError.OPTOBLOCKEDPERMANENTLY;
                    };
                    isPowerUpDetected = (bufferIn[0] & (byte)RegistrePos.POWERUP) > 0;
                    isPayoutDisabled = (bufferIn[0] & (byte)RegistrePos.DISABLED) > 0;
                    if (isOptoShorCircuit = (bufferIn[1] & (byte)RegistrePos.OPTOSHORTCIRCUIT) > 0)
                    {
                        result = HopperError.OPTOSHORTCIRCUIT;
                    };
                    isSingleCoinMode = (bufferIn[1] & (byte)RegistrePos.SINGLEPAYOUT) > 0;
                    if (isCheckSumAError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMA) > 0)
                    {
                        result = HopperError.CHECKSUMA;
                    }
                    if (isCheckSumBError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMB) > 0)
                    {
                        result = HopperError.CHECKSUMB;
                    }
                    if (isCheckSumCError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMC) > 0)
                    {
                        result = HopperError.CHECKSUMC;
                    };
                    if (isCheckSumDError = (bufferIn[1] & (byte)RegistrePos.CHECKSUMD) > 0)
                    {
                        result = HopperError.CHECKSUMD;
                    };
                    if (isPowerFailMemoryWrite = (bufferIn[1] & (byte)RegistrePos.POWERFAIL) > 0)
                    {
                        result = HopperError.POWERFAIL;
                    };
                    isPINEnabled = (bufferIn[1] & (byte)RegistrePos.PINNUMBER) > 0;
                }
                else
                {
                    throw new Exception(string.Format("Impossible de lire les refistres du hopper {0}", DeviceAddress));
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Initialisation du hopper
        /// </summary>
        public override void Init()
        {
            try
            {
                isEmptyingInProgress = false;
                deviceLevel = new CLevel();
                variables = new CHopperVariableSet(this);
                dispenseStatus = new CHopperStatus(this);
                hopperCoinID = new CHopperCoinId();
                emptyCount = new CEmptyCount();
                cipherKey = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                CoinsToDistribute = 0;

                memoryStorage = new CMemoryStorage(this);
                errorHopper = new CHopperError();
                CDevicesManager.Log.Info("Categorie d'équipement du {0} {1}", ToString(), EquipementCategory);
                CDevicesManager.Log.Info(OptoStates != 0 ? "Au moins un optocoupleur est occupé" : "Les optocoupleur sont libres");
                CDevicesManager.Log.Info("Le courant maximum autorisé pour le hopper {0} est de {1}", DeviceAddress, variables.CurrentLimit);
                CDevicesManager.Log.Info("Le délai pour arrêter le motor du hopper {0} est de {1}", DeviceAddress, variables.MotorStopDelay);

                CDevicesManager.Log.Info("Le délai maximum de vérification pour la distribution de pièce du hopper {0} est de {1} secondes", DeviceAddress, variables.PayoutDelayTO);
                CDevicesManager.Log.Info("Le courant maximum utilisé par le hopper {0} est de {1}", DeviceAddress, variables.Maxcurrent);
                double tension = variables.Tension;
                if ((tension > 26) || (tension < 19))
                {
                    CDevicesManager.Log.Error("Tension d'alimentation anormale.");
                }
                CDevicesManager.Log.Info("La tension sur le hopper {0} est de {1}", DeviceAddress, tension);
                byte connectorAddress = variables.ConnectorAddress;
                if ((connectorAddress + 1 + AddressBaseHoper) != (byte)DeviceAddress)
                {
                    CDevicesManager.Log.Error("Une différence d'adresse existe entre l'adresse logiciel et l'adresse physique du connecteur pour le hopper {0}, l'adresse physique du connecteur est : {1}", DeviceAddress, connectorAddress);
                }
                else
                {
                    CDevicesManager.Log.Info("L'adresse physique du hopper est : {0} ", connectorAddress + 1 + AddressBaseHoper);
                }
                CDevicesManager.Log.Info("Le numéro de série du Hopper {0} {1} ", Number, SerialNumber);
                hopperCoinID = CoinId;
                CDevicesManager.Log.Info("Identification des pièces : Pays : {0}, Valeur : {1:C2}, version {2}", hopperCoinID.CountryCode, (decimal)hopperCoinID.ValeurCent / 100, hopperCoinID.Issue);
                CDevicesManager.Log.Info("Prise en compte du niveau bas : {0}", IsLowLevelImplemented);
                CDevicesManager.Log.Info("Prise en compte du niveau haut : {0} ", IsHighLevelImplemented);
                CDevicesManager.Log.Info("Nivau bas atteint : {0}", IsLowLevelReached);
                CDevicesManager.Log.Info("Niveau haut atteint : {0}", IsHighLevelReached);
                CDevicesManager.Log.Info("Le type de mémoire du hopper {0} est : {1}", Number, memoryStorage.MemoryType);
                CDevicesManager.Log.Info("Le nombre de bytes dans un bloc de lecture du hopper {0} est : {1}", Number, memoryStorage.ReadBytesPerBlock);
                CDevicesManager.Log.Info("Le nombre de blocs pouvant être lus du hopper {0} est : {1}", Number, memoryStorage.ReadBlocks);
                CDevicesManager.Log.Info("Le nombre de bytes dans un bloc d'écriture du hopper {0} est : {1}", Number, memoryStorage.WriteBytesPerBlock);
                CDevicesManager.Log.Info("Le nombre de blocs pouvant être dans le hopper {0} est : {1}", Number, memoryStorage.WriteBlocks);
                CDevicesManager.Log.Info("Il reste {0} pièce(s) à distribuer par le {1}", dispenseStatus.CoinsRemaining, DeviceAddress);
                EnableHopper();
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Ajoute un évenement de niveau à liste des évenements.
        /// </summary>
        /// <param name="reason">Raison de l'évenement.</param>
        /// <param name="data">Donnée concernant l'évenement.</param>
        private void EventLevelHopper(CEvent.Reason reason, object data)
        {
            lock (eventListLock)
            {
                eventsList.Add(new CEvent
                {
                    nameOfDevice = name,
                    reason = reason,
                    data = data
                });
            }
        }

        /// <summary>
        /// Véfication des niveaux du hopper.
        /// </summary>
        public void CheckLevel()
        {
            try
            {
                if (IsPresent)
                {
                    if ((deviceLevel.hardLevel != CLevel.HardLevel.OK) && !IsHighLevelReached && !IsLowLevelReached)
                    {
                        CHardLevelData eventData = new CHardLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.hardLevel = CLevel.HardLevel.OK
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERHWLEVELCHANGED, eventData);
                    }

                    if ((deviceLevel.hardLevel != CLevel.HardLevel.VIDE) && IsLowLevelReached)
                    {
                        CHardLevelData eventData = new CHardLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.hardLevel = CLevel.HardLevel.VIDE
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERHWLEVELCHANGED, eventData);
                    }
                    if ((deviceLevel.hardLevel != CLevel.HardLevel.PLEIN) && IsHighLevelReached)
                    {
                        CHardLevelData eventData = new CHardLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.hardLevel = CLevel.HardLevel.PLEIN
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERHWLEVELCHANGED, eventData);
                    }
                    if ((deviceLevel.softLevel != CLevel.SoftLevel.OK) && (CoinsInHopper >= LevelLOSoft) &&
                        (CoinsInHopper < LevelHISoft))
                    {
                        CSoftLevelData eventData = new CSoftLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.softLevel = CLevel.SoftLevel.OK
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERSWLEVELCHANGED, eventData);
                    }
                    if ((deviceLevel.softLevel != CLevel.SoftLevel.VIDE) && (CoinsInHopper < LevelEmptySoft))
                    {
                        CSoftLevelData eventData = new CSoftLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.softLevel = CLevel.SoftLevel.VIDE
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERSWLEVELCHANGED, eventData);
                    }
                    if ((deviceLevel.softLevel != CLevel.SoftLevel.BAS) && (CoinsInHopper >= LevelEmptySoft) &&
                            (CoinsInHopper < LevelLOSoft))
                    {
                        CSoftLevelData eventData = new CSoftLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.softLevel = CLevel.SoftLevel.BAS
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERSWLEVELCHANGED, eventData);
                    }
                    if ((deviceLevel.softLevel != CLevel.SoftLevel.HAUT) && (CoinsInHopper > LevelHISoft) &&
                        (CoinsInHopper < LevelFullSoft))
                    {
                        CSoftLevelData eventData = new CSoftLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.softLevel = CLevel.SoftLevel.HAUT
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERSWLEVELCHANGED, eventData);
                    }
                    if ((deviceLevel.softLevel != CLevel.SoftLevel.PLEIN) && (LevelFullSoft < CoinsInHopper))
                    {
                        CSoftLevelData eventData = new CSoftLevelData
                        {
                            nameOfHopper = name,
                            isHCritical = IsCritical,
                            coinsNumber = CoinsInHopper,
                            level = deviceLevel.softLevel = CLevel.SoftLevel.PLEIN
                        };
                        EventLevelHopper(CEvent.Reason.HOPPERSWLEVELCHANGED, eventData);
                    }
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Demande le vidage du hopper;
        /// </summary>
        public void Empty()
        {
            try
            {
                isEmptyingInProgress = true;
                emptyCount.nameOfHopper = name;
                emptyCount.delta = CoinsInHopper;
                emptyCount.amountDelta = AmountInHopper;
                emptyCount.counter = 0;
                emptyCount.amountCounter = 0;
                do
                {
                    CoinsToDistribute = 255;
                    State = Etat.STATE_DISPENSE;
                    while (State != Etat.STATE_IDLE)
                    {
                        Thread.Sleep(10);
                    };
                    emptyCount.counter += dispenseStatus.CoinsPaid;
                    emptyCount.amountCounter = emptyCount.counter * CoinValue;
                    emptyCount.delta -= dispenseStatus.CoinsPaid;
                    emptyCount.amountDelta = emptyCount.delta * CoinValue;
                } while (!IsTOOccured && !isOnError);
                AmountInHopper = CoinsInHopper = 0;
                counterSerializer.Serialize(countersFile, counters);
                isEmptyingInProgress = false;
                lock (eventListLock)
                {
                    eventsList.Add(new CEvent()
                    {
                        reason = CEvent.Reason.HOPPEREMPTIED,
                        nameOfDevice = name,
                        data = emptyCount,
                    });
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Ìdentification du hopper.
        /// </summary>
        /// <returns>Chaine de caractères identifiant le hopper.</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Provoque la distribution par le hopper en utilisant la machine d'état
        /// </summary>
        /// <param name="coinsToDispense">Nombre de token à distribuer</param>
        public void Dispense(byte coinsToDispense)
        {
            if (!((deviceLevel.hardLevel == CLevel.HardLevel.VIDE) || (deviceLevel.softLevel == CLevel.SoftLevel.VIDE)))
            {
                CoinsToDistribute = coinsToDispense;
                State = Etat.STATE_DISPENSE;
                while (State != Etat.STATE_IDLE)
                    ;
            }
        }

        /// <summary>
        /// Mise à jour des compteurs après une distribution.
        /// </summary>
        /// <param name="CoinsNumber"></param>
        public void SubCounters(long CoinsNumber)
        {
            counters.totalAmountInCabinet -= CoinsNumber * CoinValue;
            counters.totalAmountCashOut += CoinsNumber * CoinValue;
            if (!isOnError && IsTOOccured)
            {
                CoinsInHopper = 0;
                AmountInHopper = 0;
            }
            else
            {
                CoinsInHopper -= CoinsNumber;
                AmountInHopper = CoinsInHopper * CoinValue;
            }
            CoinsOut += CoinsNumber;
            AmountOut = CoinsOut * coinValue;
            counters.SaveCounters();
        }

        /// <summary>
        /// Recharge les compteurs du hopper
        /// </summary>
        /// <param name="CoinsNumber"></param>
        public void LoadHopper(long CoinsNumber)
        {
            counters.totalAmountInCabinet += CoinsNumber * CoinValue;
            CoinsInHopper += CoinsNumber;
            AmountInHopper = CoinsInHopper * CoinValue;
            CoinsLoadedInHopper += CoinsNumber;
            AmountLoadedInHopper += CoinsLoadedInHopper * CoinValue;
            counters.totalAmountReload += CoinsInHopper * CoinValue;
            counters.SaveCounters();
            State = Etat.STATE_CHECKLEVEL;
        }

        /// <summary>
        /// Création d'un evénement.
        /// </summary>
        private void AddErrorHopperEvent()
        {
            errorHopper.nameOfHopper = name;
            lock (eventListLock)
            {
                eventsList.Add(new CEvent()
                {
                    reason = CEvent.Reason.HOPPERERROR,
                    nameOfDevice = ToString(),
                    data = errorHopper,
                });
            }
            errorHopper = new CHopperError();
        }

        /// <summary>
        /// Tâche de la machine d'état du hopper.
        /// </summary>
        public override void Task()
        {
            while (true)
            {
                mutexCCTalk.WaitOne();
                try
                {
                    switch (State)
                    {
                        case Etat.STATE_INIT:
                        {
                            Init();
                            State = Etat.STATE_RESET;
                            break;
                        }
                        case Etat.STATE_RESET:
                        {
                            delaypollLevel = polllDelayLevel * 1000;
                            deviceLevel.ID = ToString();
                            isDispensed = false;
                            if (isOnError = (errorHopper.Code = TestHopper()) != HopperError.NON_IDENTIFIEE)
                            {
                                AddErrorHopperEvent();
                            };
                            state = ((CoinsToDistribute > 0) || ((CoinsToDistribute = dispenseStatus.CoinsRemaining) > 0)) ? Etat.STATE_DISPENSE : Etat.STATE_IDLE;
                            break;
                        }
                        case Etat.STATE_DISPENSE:
                        {
                            if (CoinsToDistribute > 0)
                            {
                                CDevicesManager.Log.Debug(string.Format("Le hopper {0 } doit distribuer {1} pièces", ToString(), coinsToDistribute));
                                IsDispensed = false;
                                if (isOnError = (errorHopper.Code = TestHopper()) != HopperError.NON_IDENTIFIEE)
                                {
                                    AddErrorHopperEvent();
                                }
                                dispenseStatus.dispensedResult.CoinToDispense = CoinsToDistribute;
                                dispenseStatus.dispensedResult.AmountToDispense = (int)(CoinsToDistribute * CoinValue);
                                EnableHopper();
                                PumpRNG();
                                GetCipherKey();
                                DispenseCoins(CoinsToDistribute);
                                State = Etat.STATE_DISPENSEINPROGRESS;
                            }
                            else
                            {
                                CDevicesManager.Log.Debug(string.Format("Pas de pièces à distribuer pour le {0}", DeviceAddress));
                            }
                            break;
                        }
                        case Etat.STATE_DISPENSEINPROGRESS:
                        {
                            if (dispenseStatus.CoinsRemaining == 0)
                            {
                                State = Etat.STATE_ENDDISPENSE;
                            }
                            break;
                        }
                        case Etat.STATE_ENDDISPENSE:
                        {
                            if (isOnError = (errorHopper.Code = TestHopper()) != HopperError.NON_IDENTIFIEE)
                            {
                                AddErrorHopperEvent();
                            }
                            if (!isEmptyingInProgress)
                            {
                                if (CoinsToDistribute != dispenseStatus.CoinsPaid)
                                {
                                    //TODO indiquant l'erreur
                                }
                                lock (eventListLock)
                                {
                                    CEvent hopperEvent = new CEvent()
                                    {
                                        reason = CEvent.Reason.HOPPERDISPENSED,
                                        nameOfDevice = ToString(),
                                        data = dispenseStatus.dispensedResult,
                                    };
                                    eventsList.Add(hopperEvent);
                                }
                            }
                            IsDispensed = true;
                            SubCounters(dispenseStatus.CoinsPaid);
                            CoinsToDistribute = 0;
                            State = Etat.STATE_CHECKLEVEL;
                            break;
                        }
                        case Etat.STATE_CHECKLEVEL:
                        {
                            CheckLevel();
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_GETEQUIPEMENCATEGORY:
                            break;

                        case Etat.STATE_GETOPTOSTATE:
                            break;

                        case Etat.STATE_IDLE:
                        {
                            if (!IsInitialized)
                            {
                                isInitialized = true;
                                evReady.Set();
                            }

                            if (isOnError && IsDeviceReseted())
                            {
                                if (!(isOnError = TestHopper() != HopperError.NON_IDENTIFIEE))
                                {
                                    CoinsToDistribute = 0;
                                    isEmptyingInProgress = false;
                                    EnableHopper();
                                }
                            }

                            if (--delaypollLevel <= 0)
                            {
                                delaypollLevel = polllDelayLevel * 200;
                                State = Etat.STATE_CHECKLEVEL;
                            }
                            break;
                        }
                        case Etat.STATE_STOP:
                        {
                            IsPresent = false;
                            HTask.Abort();
                            evReady.Set();
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                mutexCCTalk.ReleaseMutex();
                Thread.Sleep(pollDelayHopper);
            }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="hopperNumber">Numéro du hopper</param>
        public CHopper(byte hopperNumber)
        {
            try
            {
                CDevicesManager.Log.Debug("Instanciation  du hopper {0}", hopperNumber);
                IsInitialized = false;
                DeviceAddress = (DefaultDevicesAddress)(hopperNumber + AddressBaseHoper);
                Number = hopperNumber;

                if (!(IsPresent = SimplePoll))
                {
                    Thread.Sleep(100);
                    IsDeviceReseted();
                    IsPresent = SimplePoll;
                }
                if (IsPresent)
                {
                    CDevicesManager.Log.Info("Hopper {0} présent", hopperNumber);
                    state = Etat.STATE_INIT;
                    HTask = new Thread(Task);
                    HTask.Start();
                }
                else
                {
                    throw new Exception($"Hopper numéro {Number} non trouvé");
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                evReady.Set();
            }
            evReady.WaitOne(30000);
        }
    }
}