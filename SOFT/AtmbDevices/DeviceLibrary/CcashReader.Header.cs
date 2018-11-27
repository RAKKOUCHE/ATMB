namespace DeviceLibrary
{

    public abstract partial class CcashReader : CccTalk
    {
        /// <summary>
        /// Enumeration des headers concernant les moyens de peiement
        /// </summary>
        protected new enum Header : byte
        {
            /// <summary>
            /// Délai recommandé pour vérifier le buffer de crédits et de code d'erreur
            /// </summary>
            /// <remarks>Au dela de ce délai le périphérique devrait être désactivé</remarks>
            REQUESTPOLLINGPRIORITY = 249,
            /// <summary>
            /// Cette commande effectue un test interne.
            /// </summary>
            PERFORMSELFTEST = 232,
            /// <summary>
            /// Cette commande envoie un masque d'inhibition. Chaque bit correspondant à un canal
            /// </summary>
            /// <remarks>0 = desactivé, 1 = activé</remarks>
            MODIFYINHIBITSTATUS = 231,
            /// <summary>
            /// Demande le masque d'inhibition
            /// </summary>
            /// <remarks>voir <see cref = "MODIFYINHIBITSTATUS"/> </remarks>

            REQUESTINHIBITSTATUS = 230,
            /// <summary>
            /// Cette commande envoie un masque d'inhibition géneral au périphérique.
            /// </summary>
            /// <remarks>Seul le bit de poid faible est pris en considération</remarks>
            MODIFYMASTERINHIBITSTATUS = 228,
            /// <summary>
            ///Cette commande demande le masque d'inhibition géneral du périphérique            
            /// </summary>
            /// <remarks>voir <see cref="MODIFYMASTERINHIBITSTATUS"/></remarks>
            REQUESTMASTERINHIBITSTATUS = 227,
        }
    }
}