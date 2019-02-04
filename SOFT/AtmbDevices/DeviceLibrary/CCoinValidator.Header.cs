/// \file CCoinValidator.Header.cs
/// \brief Fichier contenant l'énumération des commandes ccTalk spécifiques aux monnayeurs.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    /// <summary>
    /// Class gérant le monnayeur.
    /// </summary>
    public partial class CCoinValidator : CcashReader
    {
        /// \addtogroup Headers
        /// @{
        /// <summary>
        /// Groupe des commandes ccTalk.
        /// </summary>
        /// <summary>
        /// Liste des headers spécifiques au monnayeur
        /// </summary>
        public new enum Header : byte
        {
            /// <summary>
            /// Cette commande demande le status du monnayeur.
            /// </summary>
            REQUESTSTATUS = 248,

            /// <summary>
            /// Cette commande demande la base de donnée de calibration du monnayeur.
            /// </summary>
            REQUESTDATABASEVER = 243,

            /// <summary>
            /// Cette commande active un ou plusieurs activateurs du monnayeur.
            /// </summary>
            TESTSOLENOID = 240,

            /// <summary>
            /// Cette commande active les lignes de sorties du monnayyeur.
            /// </summary>
            TESTOUTPUTLINES = 238,

            /// <summary>
            /// Cette commande demande l'état des lignes d'entrées du monnayeur.
            /// </summary>
            READINPUTLINES = 237,

            /// <summary>
            /// Cette commande demande l'état des optocoupleurs du monnayeur.
            /// </summary>
            READOPTOSTATES = 236,

            /// <summary>
            /// Cette commande demande la lecture du buffer de crédit ou des codes d'erreur du monnayeur.
            /// </summary>
            READBUFFERCREDIT = 229,

            /// <summary>
            /// Cette commande enregistre l'octet d'override des tris du monnayeur.
            /// </summary>
            MODIFYOVERRIDESTATUS = 222,

            /// <summary>
            /// Cette commande demande l'octet d'override des tris du monnayeur.
            /// </summary>
            REQUESTOVERRIDESTATUS = 221,

            /// <summary>
            /// Cette commande demande l'octet des options des tris du monnayeur.
            /// </summary>
            REQUESTOPTIONFLAG = 213,

            /// <summary>
            /// Cette commande enregistre les chemins de tris d'un canal du monnayeur.
            /// </summary>
            MODIFYSORTERPATH = 210,

            /// <summary>
            /// Cette commande enregistre le chemin de tri par défaut du monnayeur.
            /// </summary>
            MODIFYDEFAULTSORTERPATH = 189,

            /// <summary>
            /// Cette commande demande le chemin de tri par défaut du monnayeur.
            /// </summary>
            REQUESTDEFAULTSORTERPATH = 188,

            /// <summary>
            /// Cette commande enregistre les limites d'acceptation du monnayeur par transaction.
            /// </summary>
            SETACCEPTLIMIT = 135,
        }

        /// @}
    }
}