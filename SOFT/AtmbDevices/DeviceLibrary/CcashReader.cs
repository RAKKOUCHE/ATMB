using System;

namespace DeviceLibrary
{
    /// <summary>
    /// Class abstraite utilisé par les moyens de paiement.
    /// </summary>
    public abstract partial class CcashReader : CccTalk
    {
        /// <summary>
        /// Tableau de conversion des unités dans la réponse au polling priority.
        /// </summary>
        private static readonly int[] priorityUnit = { 0, 1, 10, 1000, 60000 };

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

        private byte[] inhibitMask;
        /// <summary>
        /// Masque d'ihnibition (2 octets pour 16 canaux)
        /// </summary>
        protected byte[] InhibitMask
        {
            get => inhibitMask;
            set => inhibitMask = value;
        }

        private int pollingDelay;
        /// <summary>
        /// Délai entre 2 interrogation du coin validator.
        /// </summary>
        protected int PollingDelay
        {
            get => pollingDelay;
            set => pollingDelay = value;
        }

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
                    CDevicesManage.Log.Info(messagesText.getMasterInhibt, DeviceAddress);
                    ;
                    byte[] bufferIn = { (byte)InhibitStatus.DISABLED };
                    if(IsCmdccTalkSended(DeviceAddress, Header.REQUESTMASTERINHIBITSTATUS, 0, null, bufferIn))
                    {
                        result = (InhibitStatus)(bufferIn[0] & 0x01);
                    }
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                CDevicesManage.Log.Info(messagesText.inhibitStatusResult, DeviceAddress, result);
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
                CDevicesManage.Log.Info(messagesText.sendMasterInhibitStatus, status, DeviceAddress);
                byte[] bufferParam = { (byte)status };
                if(!IsCmdccTalkSended(DeviceAddress, Header.MODIFYMASTERINHIBITSTATUS, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManage.Log.Error(messagesText.errMasterInhibitStatus, DeviceAddress);
                }
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }

        }

        /// <summary>
        /// Activation globale du moyen de paiement
        /// </summary>       
        public void MasterEnable()
        {
            try
            {
                CDevicesManage.Log.Info(messagesText.activationCV, DeviceAddress);
                SetMasterInhibit(InhibitStatus.ENABLED);
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Desactive globalement le moyen de paiement
        /// </summary>
        public void MasterDisable()
        {
            try
            {
                CDevicesManage.Log.Info(messagesText.deactivationCV, DeviceAddress);
                SetMasterInhibit(InhibitStatus.DISABLED);
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

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
                    CDevicesManage.Log.Info(messagesText.getPolling, DefaultDevicesAddress.CoinAcceptor);
                    if(!IsCmdccTalkSended(DeviceAddress, Header.REQUESTPOLLINGPRIORITY, 0, null, bufferIn))
                    {
                        CDevicesManage.Log.Error(messagesText.noccTalkDevice);
                    }
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                CDevicesManage.Log.Info(messagesText.delayPolling, DeviceAddress.ToString(), priorityUnit[bufferIn[0]] * bufferIn[1]);
                return priorityUnit[bufferIn[0]] * bufferIn[1];
            }
        }

        /// <summary>
        /// Active ou desactive les canaux du monnayeur
        /// </summary>
        /// <param name="mask">masque d'inhibiton</param>
        /// <remarks>Header 231</remarks>
        public void SetInhibitStatus(byte[] mask)
        {
            try
            {
                CDevicesManage.Log.Info(messagesText.inhibitStatus, DeviceAddress, mask[0], mask[1]);
                if(!IsCmdccTalkSended(DeviceAddress, Header.MODIFYINHIBITSTATUS, (byte)mask.Length, mask, null))
                {
                    CDevicesManage.Log.Error(messagesText.errInhibitStatus, DeviceAddress);
                }
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Retourne le mask d'inhibition des canaux
        /// </summary>
        /// <param name="mask">Buffer contenant les masks d'inhibitions</param>
        public void GetInhibitMask(byte[] mask)
        {
            try
            {
                CDevicesManage.Log.Info(messagesText.getInhibitStatus, DeviceAddress);
                if(!IsCmdccTalkSended(DeviceAddress, Header.REQUESTINHIBITSTATUS, 0, null, mask))
                {
                    throw new Exception(string.Format("Impossible de lire le mask 'd'inhibition du {0}", DeviceAddress));
                }
            }
            catch(Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                    CDevicesManage.Log.Info(messagesText.cmdSelfTest, DeviceAddress);
                    result = (SelfTestResult)GetByte(Header.PERFORMSELFTEST);
                    if(result == SelfTestResult.OK)
                    {
                        CDevicesManage.Log.Info(messagesText.selfTest, DeviceAddress, result);
                    }
                    else
                    {
                        CDevicesManage.Log.Error(messagesText.selfTest, DeviceAddress, result);
                    }
                }
                catch(Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Tâche vérifiant les activitées du monnayeur
        /// </summary>
        public abstract void TaskCheckEventCV();
    }
}