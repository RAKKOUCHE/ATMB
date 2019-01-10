/// \file CPelicano.CmdMotors.cs
/// \brief Fichier contenant l'énumération des commandes moteurs du Pelicano
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    /// <summary>
    /// Classe du Pelicano.
    /// </summary>
    public partial class CPelicano : CCoinValidator
    {
        /// <summary>
        /// Enumération des commandes du moteur du Pelicano
        /// </summary>
        private enum CcmdMotors : byte
        {
            /// <summary>
            /// Ouve la trappe de rejet, fait tourner le moteur et referme la trappe de rejet.
            /// </summary>
            TRASH1 = 1,

            /// <summary>
            /// non utilisé.
            /// </summary>
            TRASH2 = 2,

            /// <summary>
            /// Non utilisé.
            /// </summary>
            TRASH3 = 3,

            /// <summary>
            /// Non documentée
            /// </summary>
            CPR = 4,

            /// <summary>
            /// Definie la vitesse de rotation du disque
            /// </summary>
            /// <remarks>100 = 3 pièces, 133 = 4 pièces, ect...</remarks>
            SETSPEED = 10,

            /// <summary>
            /// Lit la vitesse de rotation du disque.
            /// </summary>
            GETSPEED = 11,

            /// <summary>
            /// Lit le délai entre 2 trous du disque par pas de 4ms.
            /// </summary>
            GETPOCKETIME = 12,
        }
    }
}