/// \file CDevicesManager.CCashReceived.cs
/// \brief Fichier principal de la dll.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System.Collections.Generic;

namespace DeviceLibrary
{
    /// <summary>
    /// Class concernant les montants introduits.
    /// </summary>
    public class CCashReceived
    {
        /// <summary>
        /// Montant total introduit dnas la transaction.
        /// </summary>
        public int amountIntroduced;

        /// <summary>
        /// Liste contenant chaque denomination introduite.
        /// </summary>
        public List<int> listValueIntroduced;

        /// <summary>
        /// Contructeur.
        /// </summary>
        public CCashReceived()
        {
            listValueIntroduced = new List<int>();
            listValueIntroduced.Clear();
            amountIntroduced = 0;
        }
    }
}