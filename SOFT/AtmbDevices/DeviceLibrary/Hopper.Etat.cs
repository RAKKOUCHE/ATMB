/// \file Hopper.Etat.cs
/// \brief Fichier contenant l'énumération des états de la machine d'état des hoppers.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE
/// 

namespace DeviceLibrary
{
    public partial class CHopper : CccTalk
    {
        /// \addtogroup Etats
        /// @{
        /// <summary>
        /// Groupe des états des machines d'états des périphériques.
        /// </summary>
        /// <summary>
        /// Etat de la machine d'état des hoppers
        /// </summary>
        public enum Etat : byte
        {
            /// <summary>
            /// Initialisation du hopper
            /// </summary>
            STATE_INIT,
            /// <summary>
            /// Reset du hopper
            /// </summary>
            STATE_RESET,
            /// <summary>
            /// Traitement d'une demande de distribution
            /// </summary>       
            STATE_DISPENSE,
            /// <summary>
            /// Distribution en cours
            /// </summary>            
            STATE_DISPENSEINPROGRESS,            
            /// <summary>
            /// Traitement fin de distribution
            /// </summary>
            STATE_ENDDISPENSE,
            /// <summary>
            /// Vérification des niveaux du  hopper,
            /// </summary>
            STATE_CHECKLEVEL,
            /// <summary>
            /// Inactif
            /// </summary>
            STATE_IDLE,
            /// <summary>
            /// interrompt la tâche.
            /// </summary>
            STATE_STOP,
        }
        /// @}
    }
}

