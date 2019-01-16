using System;

/// \file CDevice.cs
/// \brief Fichier contenant la classe CDevice.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE
namespace DeviceLibrary
{
    /// <summary>
    /// Class contenant les informations sur u évenement.
    /// </summary>
    public class CEvent
    {
        /// <summary>
        /// Enumération des causes des événements
        /// </summary>
        public enum Reason
        {
            /// <summary>
            /// Une pièce a été reconnue par le monnayeur.
            /// </summary>
            MONEYINTRODUCTED,

            /// <summary>
            /// Une erreur a été dectée sur le monnayeur.
            /// </summary>
            COINVALIDATORERROR,

            /// <summary>
            /// Les moyens de paiement ont été fermés.
            /// </summary>
            CASHCLOSED,

            /// <summary>
            /// Les moyens de paiement ont été ouverts.
            /// </summary>
            CASHOPENED,

            /// <summary>
            /// Une erreur a été detectée sur un hopper.
            /// </summary>
            HOPPERERROR,

            /// <summary>
            /// Le hopper a terminé la distribution.
            /// </summary>
            HOPPERDISPENSED,

            /// <summary>
            /// Une des sondes hardwares d'un hopper a changé d'état.
            /// </summary>
            HOPPERHWLEVELCHANGED,

            /// <summary>
            /// Un seuil de niveau a été atteint
            /// </summary>
            HOPPERSWLEVELCHANGED,

            /// <summary>
            /// Le hopper est vidé.
            /// </summary>
            HOPPEREMPTIED,

            /// <summary>
            /// Une erreur sur le BNR est survenue.
            /// </summary>
            BNRERREUR,

            /// <summary>
            /// Un module a été reitré.
            /// </summary>
            BNRMODULEMANQUANT,

            /// <summary>
            /// Un module a été réinséré.
            /// </summary>
            BNRMODULEREINSERE,

            /// <summary>
            /// Les compteurs de la caisse du BNR on été remis à zéro.
            /// </summary>
            BNRRAZMETER,

            /// <summary>
            /// Le BNR a distribué des billets.
            /// </summary>
            BNRDISPENSE,

            /// <summary>
            /// La dll est prête.
            /// </summary>
            DLLLREADY,
        }

        /// <summary>
        /// Class d'évenement
        /// </summary>
        public class FireEventArg : EventArgs
        {
            /// <summary>
            /// Cause de l'événement.
            /// </summary>
            public Reason reason;

            /// <summary>
            /// Objet contenant les infomations concernant l'événement.
            /// </summary>
            public object donnee;
        }

        /// <summary>
        /// Raison de l'évenement.
        /// </summary>
        public Reason reason;

        /// <summary>
        /// Nom du périphérique
        /// </summary>
        public string nameOfDevice;

        /// <summary>
        /// donnée concernant l'évenenement.
        /// </summary>
        ///<remarks>Dépend du périphérique et de l'évenement.</remarks>
        public object data;
    }
}