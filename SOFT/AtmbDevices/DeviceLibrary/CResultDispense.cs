/// \file CResultDispense.cs
/// \brief Fichier contenant la classe CResultDispense
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System.Collections.Generic;

namespace DeviceLibrary
{
    /// <summary>
    /// Class contenant le resultat d'une opération de distribution.
    /// </summary>
    public class CResultDispense
    {
        /// <summary>
        /// List contenant les montants par devise distribuée.
        /// </summary>
        public List<CitemsDispensed> listValue;

        /// <summary>
        /// Montant total distribué.
        /// </summary>
        public int Montant;

        /// <summary>
        ///
        /// </summary>
        public CResultDispense()
        {
            listValue = new List<CitemsDispensed>();
        }
    }
}