/// \file CitemsDispensed.cs
/// \brief Fichier contenant la classe CitemsDispensed
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
        /// <summary>
        /// Class contenant les informations sur la denomination d'un type de billet distribué
        /// </summary>
        public class CitemsDispensed
        {
            /// <summary>
            /// Montant des billets de cette denominations distribués.
            /// </summary>
            public int amount;

            /// <summary>
            /// Valeur de la dénomination.
            /// </summary>
            public int BillValue;

            /// <summary>
            /// Nombres de billets de la dénominations distribués
            /// </summary>
            public int count;
    }
}