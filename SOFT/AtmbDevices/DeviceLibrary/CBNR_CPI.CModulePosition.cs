/// \file CBNR_CPI.CModulePosition.cs
/// \brief Fichier contenant la classe CModulePosition
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    /// <summary>
    /// Class du bnr
    /// </summary>
    public partial class CBNR_CPI : CDevice
    {
        /// <summary>
        /// Class indiquant la présence ou non d'un module.
        /// </summary>
        /// <remarks>Cette classe est utlisée pour les retraits et les insertions des modles.</remarks>
        private class CModulePosition
        {
            /// <summary>
            /// Nom du module.
            /// </summary>
            public readonly string moduleName;

            /// <summary>
            /// Flag indiquant si le module est présent.
            /// </summary>
            public bool isPresent;

            /// <summary>
            /// Indique si un module a été reinséré.
            /// </summary>
            public bool isReinserted;

            /// <summary>
            /// Constructeur
            /// </summary>
            /// <param name="moduleName">Indique le nom du module contenu dans l'instance de la classe.</param>
            /// <param name="isPresent">Initialise la présence du module.</param>
            public CModulePosition(string moduleName, bool isPresent)
            {
                this.moduleName = moduleName;
                this.isPresent = isPresent;
                isReinserted = false;
            }

            /// <summary>
            /// Renvoi le nom du module.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return moduleName;
            }
        }
    }
}