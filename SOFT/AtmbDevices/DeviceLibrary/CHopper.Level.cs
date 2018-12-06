/// \file CHopper.Level.cs
/// \brief Fichier contenant l'énumération des niveaux hard.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE


namespace DeviceLibrary
{
    public partial class CHopper : CccTalk
    {
        /// <summary>
        /// Masque des niveau             
        /// </summary>
        private enum LevelMask : byte
        {
            /// <summary>
            /// Mask indiquant si le niveau base est atteint.
            /// </summary>
            LOLEVELREACHED = 0x01,

            /// <summary>
            /// Mask indiquant si le niveau haut est atteint.
            /// </summary>
            HILEVEREACHED = 0x02,

            /// <summary>
            /// Mask indiquant si le niveau bas est implémenté.
            /// </summary>
            LOLEVELIMPLEMENTED = 0x10,

            /// <summary>
            /// Mask indiquant si le niveau haut est implémenté.
            /// </summary>
            HILEVELIMPLEMENTED = 0x20,
        }
    }
}

