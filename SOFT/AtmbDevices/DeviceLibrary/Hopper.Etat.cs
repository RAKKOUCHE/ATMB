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
            /// Etat initial
            /// </summary>
            RESET,

            /// <summary>
            /// Traitement d'une demande de distribution
            /// </summary>
            /// 
            DISPENSE,

            /// <summary>
            /// Distribution en cours
            /// </summary>            
            DISPENSEINPROGRESS,
            
            /// <summary>
            /// Traitement fin de distribution
            /// </summary>
            ENDDISPENSE,

            /// <summary>
            /// Vérification des niveaux du  hopper,
            /// </summary>
            CHECKLEVEL,

            /// <summary>
            /// Inactif
            /// </summary>
            IDLE = 0XFF,
        }
    }
}

