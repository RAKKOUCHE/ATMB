/// \file CHopper.cs
/// \brief Fichier contenant la classe CHopper.
/// \date 28 11 2018
/// \version 1.1
/// Modification du traitements des erreurs du hopper.
/// Création de sous fichiers contenant les classes.
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    public partial class CHopper
    {
        /// <summary>
        /// Classe des niveaux des hopeprs.
        /// </summary>
        public class CLevel
        {
            /// <summary>
            /// Variable contenant le niveau du périphérique par rapport à ses sondes hardware.
            /// </summary>
            public HardLevel hardLevel;

            /// <summary>
            /// Nom du périphérique
            /// </summary>
            public string ID;

            /// @}
            /// <summary>
            /// Variable contenant le niveau du périphérique par rapport aux paramètres et au compteurs.
            /// </summary>
            public SoftLevel softLevel;

            /// <summary>
            /// Constructeur
            /// </summary>
            /// <param name="softlevel">Niveau soft, par défaut inconnu</param>
            /// <param name="hardLevel">Niveau hard, par défaut inconnu</param>
            /// <remarks>Le fait d'avoir les niveaux à l'état inconnu permet de provoqué un évenement permettant de déterminé le niveau actuel.</remarks>
            public CLevel(SoftLevel softlevel = SoftLevel.INCONNU, HardLevel hardLevel = HardLevel.INCONNU)
            {
                softLevel = softlevel;
                this.hardLevel = hardLevel;
            }

            /// <summary>
            /// Enumération des niveaux Hard
            /// </summary>
            /// <remarks>Determinés par la lecture des sondes de niveau du périphérique</remarks>
            public enum HardLevel : byte
            {
                /// <summary>
                /// Le périphérique est vide.
                /// </summary>
                VIDE,

                /// <summary>
                /// Aucune sonde n'est activée.
                /// </summary>
                OK,

                /// <summary>
                /// Le périphérique est plein
                /// </summary>
                PLEIN,

                /// <summary>
                /// Les sondes n'ont pas été encore interrogées.
                /// </summary>
                INCONNU,
            }

            /// \addtogroup Niveaux Niveaux hoppers
            /// <summary>Groupe des niveaux physiques et logiciels.</summary>
            /// @{
            /// <summary>
            /// Enumération des niveaux soft
            /// </summary>
            public enum SoftLevel : byte
            {
                /// <summary>
                /// Le nombre de pièces dans périphérique est au dessous ou égal au niveau vide indiqué dans le fichier paramètres.
                /// </summary>
                VIDE,

                /// <summary>
                /// Le nombre de pièces dans périphérique est au dessous ou égal au niveau d'alerte bas indiqué dans le fichier paramètres.
                /// </summary>
                BAS,

                /// <summary>
                /// Le nombre de pièces dans périphérique est compris entre le niveau d'alerte bas et le niveau d'alerte au indiqués dans le fichier paramètres.
                /// </summary>
                OK,

                /// <summary>
                /// Le nombre de pièces dans périphérique est au dessus ou égal au niveau d'alerte haut indiqué dans le fichier paramètres.
                /// </summary>
                HAUT,

                /// <summary>
                /// Le nombre de pièces dans périphérique est au dessus ou égal au niveau plein indiqué dans le fichier paramètres.
                /// </summary>
                PLEIN,

                /// <summary>
                /// Le nombre de pièces dans périphérique ou le fichier paramètre n'ont pas été encore lus.
                /// </summary>
                INCONNU,
            }
        }
    }
}