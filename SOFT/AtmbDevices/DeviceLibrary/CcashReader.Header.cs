/// \file CcashReader.Header.cs
/// \brief Fichier contenant l'énumération des commandes ccTalk spécifiques aux accepteurs.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    public abstract partial class CcashReader : CccTalk
    {
        /// \addtogroup Headers
        /// @{
        /// <summary>
        /// Groupe des commandes ccTalk.
        /// </summary>
        /// <summary>
        /// Enumeration des concernant concernant les moyens de paiement
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

        /// @}
    }
}