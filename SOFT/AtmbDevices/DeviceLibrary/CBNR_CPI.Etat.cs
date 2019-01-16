/// \file CBNR_CPI.Etat.cs
/// \brief Fichier contenant l'énumération des états de la machine d'état du BNR
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    public partial class CBNR_CPI : CDevice
    {
        /// \addtogroup Etats
        /// @{
        /// <summary>
        /// Groupe des états des machines d'états des périphériques.
        /// </summary>
        /// <summary>
        /// Etat de la machine d'état du BNR.
        /// </summary>
        public enum Etat : byte
        {
            /// <summary>
            /// Initialisation du BNR.
            /// </summary>
            STATE_INIT,

            /// <summary>
            /// Ouverture de la communication entre le BNR et le PC.
            /// </summary>
            STATE_OPEN_API,

            /// <summary>
            /// Initialisation hardware.
            /// </summary>
            STATE_RESET,

            /// <summary>
            /// Enregistrement de la date.
            /// </summary>
            /// <remarks>Nécessaire pour les versions antéreures du BNR</remarks>
            STATE_SETDATETIME,

            /// <summary>
            /// Lecture de la liste des modules.
            /// </summary>
            STATE_GETMODULE,

            /// <summary>
            /// Lecture de l'état du BNR.
            /// </summary>
            STATE_GETSTATUS,

            /// <summary>
            /// Etat pour désactiver le périphérique.
            /// </summary>
            STATE_DISABLE,

            /// <summary>
            /// Etat pour activer le périphérique.
            /// </summary>
            STATE_ENABLE,

            /// <summary>
            /// Etat pour distribuer un montant.
            /// </summary>
            STATE_DISPENSE,

            /// <summary>
            /// Test la disponibilité du montant.
            /// </summary>
            STATE_DENOMINATE,

            /// <summary>
            /// création de la liste des unités logiques.
            /// </summary>
            STATE_GETLUS,

            /// <summary>
            /// BNR en attente.
            /// </summary>
            STATE_IDLE,

            /// <summary>
            /// Arrêt du BNR.
            /// </summary>
            STATE_STOP,
        }

        /// @}
    }
}