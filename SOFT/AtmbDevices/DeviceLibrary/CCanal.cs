﻿/// \file CCanal.cs
/// \brief Fichier contenant la classe CCanal
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    /// <summary>
    /// Class d'un canal d'un périphérique de paiement
    /// </summary>    
    /// \details Cette classe contient et gère les paramètres d'un canal*/
    public partial class CCanal
    {
        /// <summary>
        /// Instance propriètaire du canal
        /// </summary>
        private CCoinValidator CVOwner;

        /// <summary>
        /// Numéro du canal
        /// </summary>
        public byte Number;

        /// <summary>
        /// Identification de la pièce reconnue dans le canal
        /// </summary>
        public CCoindID coinId;

        /// <summary>
        /// Instance des chemins utilisés pour trier la pièce reconnue dans le canal.
        /// </summary>
        public CSorter sorter;
        
        private byte hopperToLoad;
        /// <summary>
        /// Hopper vers lequel sera dirigé la pièce reconnue
        /// </summary>
        public byte HopperToLoad
        {
            get => hopperToLoad;
            set => hopperToLoad = value;
        }

        /// <summary>
        /// Nombre de pièces reconnues par le canal.
        /// </summary>
        public long CoinIn
        {
            get => CccTalk.counters.coinsInAccepted[Number - 1];
            set => CccTalk.counters.coinsInAccepted[Number - 1] = value;
        }

        /// <summary>
        /// Montant introduit dans le canal
        /// </summary>
        public long MontantIn
        {
            get => CccTalk.counters.amountCoinInAccepted[Number - 1];
            set => CccTalk.counters.amountCoinInAccepted[Number - 1] = value;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="number">Numéro du canal</param>
        /// <param name="owner">Nécessaire pour effectuer les commandes</param>
        public CCanal(byte number, CCoinValidator owner)
        {
            Number = number;
            CVOwner = owner;
            coinId = new CCoindID(this);
            sorter = new CSorter(this);
        }
    }
}
