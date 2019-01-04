/// \file CHopper.CEmptyCount.cs
/// \brief Fichier contenant la classe des compteurs utilisés pour le vidage d'un hopper.
/// \date 30 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    public partial class CHopper : CccTalk
    {
        /// <summary>
        /// Class contenant les résultats du vidage du hopper.
        /// </summary>
        public class CEmptyCount
        {
            /// <summary>
            /// Nom du hopper.
            /// </summary>
            public string nameOfHopper;
            /// <summary>
            /// Nombre de pièces comptées durant le vidage
            /// </summary>
            public long counter;
            /// <summary>
            /// Montant distribué pendant le vidage.
            /// </summary>
            public long amountCounter;
            /// <summary>
            /// Différence entre le nombre de pièces dans les compteurs et le nombre de pièces distribuées pendant le vidage.
            /// </summary>
            public long delta;
            /// <summary>
            /// Différence entre le montant distribué et le montant dans les compteurs.
            /// </summary>
            public long amountDelta;
        }
    }
}

