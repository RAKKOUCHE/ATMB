/// \file CBNR_CPI.cs
/// \brief Fichier contenant la classe CBNR_CPI
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE
namespace DeviceLibrary
{
    /// <summary>
    /// Class contenant les informations d'un module vidé.
    /// </summary>
    public class CModuleEmptied
    {
        /// <summary>
        /// Nom du module.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Montant transféré dans la caisse du BNR.
        /// </summary>
        public int amount;

        /// <summary>
        /// Nombre de billets transférés.
        /// </summary>
        public int count;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="moduleName">Nom du module.</param>
        public CModuleEmptied(string moduleName)
        {
            name = moduleName;
        }
    }
}