/// \file CcashReader.cs
/// \brief Fichier contenant la classe abstraite CcashReader
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
{
    /// <summary>
    /// Class abstraite utilisé par les moyens de paiement.
    /// </summary>
    public abstract partial class CcashReader : CccTalk
    {
        #region ENUMERATION

        /// <summary>
        /// Enumération des ihnibitions.
        /// </summary>
        protected enum InhibitStatus
        {
            /// <summary>
            /// Périphérique désactivé
            /// </summary>
            DISABLED = 0,

            /// <summary>
            /// Périphérique activé
            /// </summary>
            ENABLED = 1,
        }

        #endregion ENUMERATION

        #region CONSTANTES

        /// <summary>
        /// Tableau de conversion des unités dans la réponse au polling priority.
        /// </summary>
        private static readonly int[] PriorityUnit = { 0, 1, 10, 1000, 60000 };

        #endregion CONSTANTES

        #region VARIABLES

        private byte[] inhibitMask;

        #endregion VARIABLES

        #region PROPRIETEES

        private int pollingDelay;

        /// <summary>
        /// Renvoi l'activation du monnayeur
        /// </summary>
        /// <returns>
        /// ENABLED si le monnayeur est activé
        /// DISABLED si le monnayeur est désactivé
        /// </returns>
        /// <remarks>Header 227</remarks>
        protected InhibitStatus MasterInhibitStatus
        {
            get
            {
                InhibitStatus result = InhibitStatus.DISABLED;
                try
                {
                    CDevicesManager.Log.Info(messagesText.getMasterInhibt, DeviceAddress);
                    byte[] bufferIn = { (byte)InhibitStatus.DISABLED };
                    if (IsCmdccTalkSended(DeviceAddress, Header.REQUESTMASTERINHIBITSTATUS, 0, null, bufferIn))
                    {
                        result = (InhibitStatus)(bufferIn[0] & 0x01);
                    }
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                CDevicesManager.Log.Info(messagesText.inhibitStatusResult, DeviceAddress, result);
                return result;
            }
        }

        /// <summary>
        /// Délai entre 2 interrogation du coin validator.
        /// </summary>
        protected int PollingDelay
        {
            get => pollingDelay;
            set => pollingDelay = value;
        }

        #endregion PROPRIETEES

        #region METHODES

        /// <summary>
        /// Renvoi le délai maximum du polling du périphérique.
        /// </summary>
        /// <remarks>Header 249</remarks>
        /// <returns>Délai du polling ou 0 en cas d'échec de traitement</returns>
        protected int PollingPriority
        {
            get
            {
                byte[] bufferIn = { 0, 0 };
                try
                {
                    CDevicesManager.Log.Info(messagesText.getPolling, DefaultDevicesAddress.CoinAcceptor);
                    if (!IsCmdccTalkSended(DeviceAddress, Header.REQUESTPOLLINGPRIORITY, 0, null, bufferIn))
                    {
                        CDevicesManager.Log.Error(messagesText.noccTalkDevice);
                    }
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                CDevicesManager.Log.Info(messagesText.delayPolling, DeviceAddress.ToString(), PriorityUnit[bufferIn[0]] * bufferIn[1]);
                return PriorityUnit[bufferIn[0]] * bufferIn[1];
            }
        }

        /// <summary>
        /// Effectue un test interne et renvoie le résultat.
        /// </summary>
        /// <remarks>Header 232</remarks>
        protected SelfTestResult SelfTest
        {
            get
            {
                SelfTestResult result = SelfTestResult.OK;
                try
                {
                    CDevicesManager.Log.Info(messagesText.cmdSelfTest, DeviceAddress);
                    result = (SelfTestResult)GetByte(Header.PERFORMSELFTEST);
                    if (result == SelfTestResult.OK)
                    {
                        CDevicesManager.Log.Info(messagesText.selfTest, DeviceAddress, result);
                    }
                    else
                    {
                        CDevicesManager.Log.Error(messagesText.selfTest, DeviceAddress, result);
                    }
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Active ou desactive le moyen de paiment
        /// </summary>
        /// <param name="status">= 1 pour activer , 0 pour désactiver</param>
        /// <remarks>
        /// Header 228    \n
        /// Les 7 bits de poids fort ne sont pas considérés.
        /// </remarks>
        private void SetMasterInhibit(InhibitStatus status)
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.sendMasterInhibitStatus, status, DeviceAddress);
                byte[] bufferParam = { (byte)status };
                if (!IsCmdccTalkSended(DeviceAddress, Header.MODIFYMASTERINHIBITSTATUS, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManager.Log.Error(messagesText.errMasterInhibitStatus, DeviceAddress);
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Masque d'ihnibition (2 octets pour 16 canaux)
        /// </summary>
        protected byte[] GetInhibitMask()
        {
            return inhibitMask;
        }

        /// <summary>
        /// Retourne le mask d'inhibition des canaux
        /// </summary>
        /// <param name="mask">Buffer contenant les masks d'inhibitions</param>
        protected void GetInhibitMask(byte[] mask)
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.getInhibitStatus, DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, Header.REQUESTINHIBITSTATUS, 0, null, mask))
                {
                    throw new Exception(string.Format("Impossible de lire le mask 'd'inhibition du {0}", DeviceAddress));
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Desactive globalement le moyen de paiement
        /// </summary>
        protected void MasterDisable()
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.deactivationCV, DeviceAddress);
                SetMasterInhibit(InhibitStatus.DISABLED);
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Activation globale du moyen de paiement
        /// </summary>
        protected void MasterEnable()
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.activationCV, DeviceAddress);
                SetMasterInhibit(InhibitStatus.ENABLED);
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Masque d'ihnibition (2 octets pour 16 canaux)
        /// </summary>
        protected void SetInhibitMask(byte[] value)
        {
            inhibitMask = value;
        }

        /// <summary>
        /// Active ou desactive les canaux du monnayeur
        /// </summary>
        /// <param name="mask">masque d'inhibiton</param>
        /// <remarks>Header 231</remarks>
        protected void SetInhibitStatus(byte[] mask)
        {
            try
            {
                CDevicesManager.Log.Info(messagesText.inhibitStatus, DeviceAddress, mask[0], mask[1]);
                if (!IsCmdccTalkSended(DeviceAddress, Header.MODIFYINHIBITSTATUS, (byte)mask.Length, mask, null))
                {
                    CDevicesManager.Log.Error(messagesText.errInhibitStatus, DeviceAddress);
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Tâche vérifiant les activitées du monnayeur
        /// </summary>
        public override void Task()
        {
        }

        #endregion METHODES
    }
}