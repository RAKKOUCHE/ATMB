/// \file CCtalk.cs
/// \brief Fichier contenant la classe CccTalk
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;
using System.IO;

namespace DeviceLibrary
{
    //public abstract partial class CccTalk
    //{
    /// <summary>
    /// Class des compteurs
    /// </summary>
    [Serializable()]
    public class CcoinsCounters
    {
        /// <summary>
        /// Montant total dans la caisse
        /// </summary>
        public long totalAmountInCB;

        /// <summary>
        /// Montant total accepté dans le monnayeur.
        /// </summary>
        public long totalAmountCashInCV;

        /// <summary>
        /// Montant total retrourné
        /// </summary>
        public long totalAmountCashOut;

        /// <summary>
        /// Montant rechargé dans les hoppers.
        /// </summary>
        public long totalAmountReload;

        /// <summary>
        /// Montant total contenu dans les hoppers et la caisse.
        /// </summary>
        public long totalAmountInCabinet;

        /// <summary>
        ///Montant des trop perçus.
        /// </summary>
        public long amountOverPay;

        /// <summary>
        /// Nombre de pièces acceptées pour chaque canal.
        /// </summary>
        public long[] coinsInAccepted;

        /// <summary>
        /// Montant accepté pour chaque canal.
        /// </summary>
        public long[] amountCoinInAccepted;

        /// <summary>
        /// Nombre de pièces rendues pour chaque hopper.
        /// </summary>
        public long[] coinsOut;

        /// <summary>
        /// Montant rendu pour chaque hopper
        /// </summary>
        public long[] amountCoinOut;

        /// <summary>
        /// Nombre de pièces dans chaque hopper.
        /// </summary>
        public long[] coinsInHopper;

        /// <summary>
        /// Montant dans chaque hopper.
        /// </summary>
        public long[] amountInHopper;

        /// <summary>
        /// Nombre de pièces rechargées
        /// </summary>
        public long[] coinsLoadedInHopper;

        /// <summary>
        /// Montant des pièces chargées dans le hopper.
        /// </summary>
        public long[] amountLoadedInHopper;

        /// <summary>
        /// Constructeur
        /// </summary>
        public CcoinsCounters()
        {
            totalAmountCashInCV = 0;
            totalAmountCashOut = 0;
            totalAmountInCabinet = 0;
            amountOverPay = 0;
            coinsInAccepted = new long[CCoinValidator.numChannel];
            amountCoinInAccepted = new long[CCoinValidator.numChannel];
            coinsOut = new long[CHopper.maxHopper];
            amountCoinOut = new long[CHopper.maxHopper];
            coinsInHopper = new long[CHopper.maxHopper];
            amountInHopper = new long[CHopper.maxHopper];
            coinsLoadedInHopper = new long[CHopper.maxHopper];
            amountLoadedInHopper = new long[CHopper.maxHopper];
        }

        /// <summary>
        ///
        /// </summary>
        public void SaveCounters()
        {
            CccTalk.countersFile.Seek(0, SeekOrigin.Begin);
            CccTalk.counterSerializer.Serialize(CccTalk.countersFile, this);
        }
    }
    //   }
}