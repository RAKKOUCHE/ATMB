/// \file CPelicano.cs
/// \brief Fichier contenant la classe CPelicano
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;
using System.Threading;

namespace DeviceLibrary
{
    /// <summary>
    /// Classe du Pelicano.
    /// </summary>
    public partial class CPelicano : CCoinValidator
    {
        /// <summary>
        /// Enumération des status de la limitation de l'acceptation.
        /// </summary>
        private enum OptionFlag
        {
            /// <summary>
            /// Limitation de l'acceptation non implémentée
            /// </summary>
            UNSUPPORTED = 0,

            /// <summary>
            /// Limitation de l'acceptation implémentée
            /// </summary>
            SUPPORTED = 1,
        }

        /// <summary>
        /// Enumération des présences de pièces dans le bac.
        /// </summary>
        protected enum CoinPresent
        {
            /// <summary>
            /// Pas de pièce dans le container.
            /// </summary>
            NOCOIN = 0,

            /// <summary>
            /// Au monis une pièce présent dans le container.
            /// </summary>
            COINPRESENT = 1,
        }

        private bool isPelicanoInitialized;

        /// <summary>
        /// Indique si le Pelicano est initialisé.
        /// </summary>
        public bool IsPelicanoInitialized
        {
            get => isPelicanoInitialized;
            set => isPelicanoInitialized = value;
        }

        /// <summary>
        /// Contient le prochaine etat.
        /// </summary>
        private Etat nextState;

        private CoinPresent coinInContainer;

        /// <summary>
        /// Contient l'indicateur de présence de pièce dans le container.
        /// </summary>
        protected CoinPresent CoinInContainer
        {
            get => coinInContainer;
            set => coinInContainer = value;
        }

        /// <summary>
        /// Verification des opto-coupleurs
        /// </summary>
        /// <remarks>Header 236</remarks>
        public override byte OptoStates
        {
            get
            {
                byte result = 0XFF;
                try
                {
                    if ((result = base.OptoStates) == 0XFF)
                    {
                        CDevicesManager.Log.Error("Impossible de lire l'état des optos coupleur du {0}", DeviceAddress);
                    }
                    else
                    {
                        CoinInContainer = ((result & 0x01) > 0) ? CoinPresent.COINPRESENT : CoinPresent.NOCOIN;
                        TrashLid = ((result & 0x02) > 0) ? TrashDoor.OPEN : TrashDoor.CLOSED;
                        ExitSensor = ((result & 0x04) > 0) ? LowerSensor.BUSY : LowerSensor.FREE;
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
        /// Vérifie si une pièce est présente dans le cointainer.
        /// </summary>
        /// <returns>
        /// true si au moins une pièce est présente dans le bol\n
        /// false s'il n'y a pas de pièce
        /// </returns>
        public bool IsCoinPresent
        {
            get
            {
                bool result = false;
                try
                {
                    if (OptoStates != 0xFF)
                    {
                        result = coinInContainer == CoinPresent.COINPRESENT;
                    }
                    else
                    {
                        CDevicesManager.Log.Error("Impossible de lire l'état des optocoupleurs du pelicano");
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Retourne le traitement d'une limite d'acceptation.
        /// </summary>
        /// <returns>
        /// UNSUPPORTED si l'option de limit d'acceptation est désactivée.\n
        /// SUPPORTED si l'option de limit d'acceptation est activée.
        /// </returns>
        private OptionFlag AcceptLimitFeature
        {
            get
            {
                OptionFlag result = OptionFlag.UNSUPPORTED;
                try
                {
                    byte byResult = GetByte(CCoinValidator.Header.REQUESTOPTIONFLAG);
                    result = (byResult & 16) > 0 ? OptionFlag.SUPPORTED : OptionFlag.UNSUPPORTED;
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Lecture du numéro de série du Pelicano
        /// </summary>
        /// <returns>Le numéro de série</returns>
        public override int SerialNumber
        {
            get
            {
                byte[] bufferIn = { 0, 0, 0, 0 };
                try
                {
                    CDevicesManager.Log.Info(messagesText.getSN, DeviceAddress);
                    if (!IsCmdccTalkSended(DeviceAddress, CccTalk.Header.REQUESTSN, 0, null, bufferIn))
                    {
                        CDevicesManager.Log.Error(messagesText.erreurCmd, CccTalk.Header.REQUESTSN, DeviceAddress);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                CDevicesManager.Log.Info("Le numéro de série du {0} est {1}", DeviceAddress, bufferIn[0] + (0x100 * bufferIn[1]) + (0x10000 * bufferIn[2]) + (0x1000000 * bufferIn[3]));
                return bufferIn[0] + (0x100 * bufferIn[1]) + 0x10000 * bufferIn[2] + (0x1000000 * bufferIn[3]);
            }
        }

        /// <summary>
        /// Commande moteur du pelicano.
        /// </summary>
        /// <param name="command">Sous-command de la fonction.</param>
        /// <param name="data">vitesse en pourcentage.</param>
        /// <returns>Donnée dépendant de la sous command.</returns>
        private byte ActivateMotor(CcmdMotors command, byte data = 0)
        {
            try
            {
                byte[] bufferIn = { 0 };
                byte[] bufferParam;
                CDevicesManager.Log.Info(messagesText.cmdMotorPelicano, command, data);
                if (command == CcmdMotors.SETSPEED)
                {
                    bufferParam = new byte[] { (byte)command, data };
                }
                else
                {
                    bufferParam = new byte[] { (byte)command };
                }
                if (!IsCmdccTalkSended(DeviceAddress, CccTalk.Header.OPERATEMOTOR, (command == CcmdMotors.SETSPEED) ? (byte)2 : (byte)1, bufferParam, bufferIn))
                {
                    CDevicesManager.Log.Error(messagesText.errMotorPelicano);
                }
                if ((command == CcmdMotors.GETSPEED) || (command == CcmdMotors.GETPOCKETIME))
                {
                    return bufferIn[0];
                }
                else
                {
                    return 0xFF;
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return 0;
        }

        /// <summary>
        /// Active le moteur en mode trash.
        /// </summary>
        private void TrashCycle(CcmdMotors level = CcmdMotors.TRASH1)
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.cmdMotorPelicanoMode, level);
                if (ActivateMotor(level) != 0xFF)
                {
                    CDevicesManager.Log.Error("Impossible d'activer le moteur du pelicano");
                }
                else
                {
                    CDevicesManager.Log.Info(messagesText.cmdMotorPelicanoInProgress);
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

#pragma warning disable IDE0051 // Remove unused private members
        /// <summary>
        /// Active le moteur en mode CPR.
        /// </summary>
        private void CPRStart()
#pragma warning restore IDE0051 // Remove unused private members
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.cmdMotorPelicanoMode, CcmdMotors.CPR);
                if (ActivateMotor(CcmdMotors.CPR) != 0xFF)
                {
                    CDevicesManager.Log.Error(messagesText.errMotorPelicano);
                }
                else
                {
                    CDevicesManager.Log.Info(messagesText.cmdMotorPelicanoInProgress);
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Renvoie la vitesse du moteur.
        /// </summary>
        /// <returns>
        /// La vitesse du moteur en pourcentage.\n
        /// 0 si la command ne renvoit rien.
        /// </returns>
        public byte SpeedMotor
        {
            get
            {
                byte result = 0;
                try
                {
                    CDevicesManager.Log.Info(messagesText.cmdMotorPelicanoMode, CcmdMotors.GETSPEED);
                    result = ActivateMotor(CcmdMotors.GETSPEED);
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
            set
            {
                try
                {
                    CDevicesManager.Log.Info(messagesText.cmdMotorPelicanoMode, CcmdMotors.SETSPEED);
                    if (ActivateMotor(CcmdMotors.SETSPEED, value) != 0xFF)
                    {
                        CDevicesManager.Log.Error(messagesText.errMotorPelicano);
                    }
                    else
                    {
                        CDevicesManager.Log.Info("La vitesse du moteur est fixée à {0}%", value);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }
        }

        /// <summary>
        /// Renvoie le pocket time
        /// </summary>
        /// <returns>
        /// Le délai entre 2 trous dans le disque * 4 ms.\n
        /// 0 si la fonction ne retourne pas la valeur
        /// </returns>
        private byte PocketTime
        {
            get
            {
                byte result = 0;
                try
                {
                    CDevicesManager.Log.Info(messagesText.cmdMotorPelicanoMode, CcmdMotors.GETPOCKETIME);
                    result = ActivateMotor(CcmdMotors.GETPOCKETIME);
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Tâche de la machine d'état du Pelicano
        /// </summary>
        public override void Task()
        {
            byte nombre = 0;
            while (true)
            {
                try
                {
                    CheckState();
                    mutexCCTalk.WaitOne();
                    if (IsCVInitialized)
                    {
                        nextState = Etat.STATE_GETCREDITBUFFER;
                    }
                    switch (State)
                    {
                        case Etat.STATE_TRASHEMPTY:
                        {
                            CDevicesManager.Log.Info("Evacuation des pièces du container du Pelicano");
                            TrashCycle();
                            nextState = Etat.STATE_DISABLEMASTER;
                            Thread.Sleep(5000);
                            break;
                        }
                        case Etat.STATE_CHECKTRASHDOOR:
                        {
                            CDevicesManager.Log.Info("Vérification de la porte du container du Pelicano");
                            if (OptoStates != 0xFF)
                            {
                                CDevicesManager.Log.Info("La porte du container du Pelicano est", TrashLid);
                            }
                            else
                            {
                                throw new Exception("Impossible de lire l'état des optocoupleurs.");
                            }
                            break;
                        }
                        case Etat.STATE_CHECKLOWERSENSOR:
                        {
                            CDevicesManager.Log.Info("Vérification de l'absence d'obstacle dans les sorties du Pelicano.");
                            if (OptoStates != 0xFF)
                            {
                                CDevicesManager.Log.Info("Les sortie du Pelicano est {0}", ExitSensor);
                            }
                            else
                            {
                                throw new Exception("Impossible de lire l'état des optocoupleurs.");
                            }
                            break;
                        }
                        case Etat.STATE_GETSPEEDMOTOR:
                        {
                            CDevicesManager.Log.Info("La vitesse de rotation du disque du Pelicano est de {0}%", SpeedMotor);
                            break;
                        }
                        case Etat.STATE_SETSPEEDMOTOR:
                        {
                            CDevicesManager.Log.Info("Fixe la vitesse de rotation du disque du Pelicano à {0}%", SpeedMotor);
                            break;
                        }
                        case Etat.STATE_GETPOCKET:
                        {
                            CDevicesManager.Log.Info("Le temps de rotation entre 2 trous du disque du Pelicano est de {0}", PocketTime);
                            break;
                        }
                        case Etat.STATE_GETOPTION:
                        {
                            CDevicesManager.Log.Info("La limite d'acceptation est {0}", AcceptLimitFeature);
                            break;
                        }
                        case Etat.STATE_DISABLEMASTER:
                        {
                            if (IsCoinPresent)
                            {
                                nextState = Etat.STATE_TRASHEMPTY;
                            }
                            break;
                        }
                        case Etat.STATE_IDLE:
                        {
                            if (IsCoinPresent)
                            {
                                if (CDevicesManager.ToPay < 1)
                                {
                                    nextState = Etat.STATE_TRASHEMPTY;
                                }
                                else
                                {
                                    if (MasterInhibitStatus == InhibitStatus.DISABLED)
                                    {
                                        nextState = Etat.STATE_ENABLEMASTER;
                                        nombre++;
                                    }
                                }
                            }
                            else
                            {
                                if (MasterInhibitStatus == InhibitStatus.ENABLED)
                                {
                                    nextState = Etat.STATE_DISABLEMASTER;
                                }
                            }
                            if (BackEventCounter != creditBuffer.EventCounter)
                            {
                                nextState = Etat.STATE_CHECKCREDIBUFFER;
                            }
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                    if (IsCVInitialized)
                    {
                        State = nextState;
                    }
                    mutexCCTalk.ReleaseMutex();
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                finally
                {
                    mutexCCTalk.WaitOne();
                    mutexCCTalk.ReleaseMutex();
                    Thread.Sleep(PollingDelay);
                }
            }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public CPelicano()
        {
            IsPelicanoInitialized = false;
            CDevicesManager.Log.Info("Instanciation de la classe CPelicano.");

            coinInContainer = CoinPresent.NOCOIN;
            ExitSensor = LowerSensor.FREE;
            TrashCycle();
            mutexCCTalk.WaitOne();
            Thread.Sleep(6000);
            mutexCCTalk.ReleaseMutex();
            CDevicesManager.Log.Info("La porte du container est {0}", TrashLid);
            CDevicesManager.Log.Info("Les optos de sortie sont {0}", ExitSensor);
            CDevicesManager.Log.Info("Etat des pièces dans le container {0}", CoinInContainer);
            IsPelicanoInitialized = true;
        }
    }
}