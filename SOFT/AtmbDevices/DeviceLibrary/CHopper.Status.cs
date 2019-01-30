﻿/// \file CHopper.Status.cs
/// \brief Fichier contenant la classe CHopperStatus
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
{
    public partial class CHopper : CccTalk
    {
        /// <summary>
        /// Classe des status des hoppers
        /// </summary>
        public class CHopperStatus
        {
            /// <summary>
            /// Propriétaire de la class
            /// </summary>
            private readonly CHopper Owner;

            private byte coinsPaid;

            private byte coinsRemaining;

            private byte coinsUnpaid;

            private byte eventCounter;

            /// <summary>
            /// Instance de la classe CDispenseResult.
            /// </summary>
            public CDispensedResult dispensedResult;

            /// <summary>
            /// Contructeur;
            /// </summary>
            /// <param name="owner"></param>
            public CHopperStatus(CHopper owner)
            {
                Owner = owner;
                dispensedResult = new CDispensedResult
                {
                    nameOfHopper = Owner.name
                };
            }

            /// <summary>
            /// Nombre de pièces distribuées dans l'opération en cours.
            /// </summary>
            public byte CoinsPaid
            {
                get
                {
                    GetHopperStatus();
                    CDevicesManager.Log.Debug("Nombre de pièces payées {0}", coinsPaid);
                    return coinsPaid;
                }
                set => coinsPaid = value;
            }

            /// <summary>
            /// Nombre de pièces restant à distribuer dans l'opération en cours.
            /// </summary>
            public byte CoinsRemaining
            {
                get
                {
                    GetHopperStatus();
                    CDevicesManager.Log.Debug("Nombre de pièces restant à distribuer {0}", coinsRemaining);
                    return coinsRemaining;
                }
                set => coinsRemaining = value;
            }

            /// <summary>
            /// Nombre de pièces distribuées dans l'opération en cours.
            /// </summary>
            public byte CoinsUnpaid
            {
                get
                {
                    GetHopperStatus();
                    CDevicesManager.Log.Debug("Nombre de pièces restant à payer {0}", coinsUnpaid);
                    return coinsUnpaid;
                }
                set => coinsUnpaid = value;
            }

            /// <summary>
            /// Nombre de distribution effectuée.
            /// </summary>
            public byte EventCounter
            {
                get
                {
                    GetHopperStatus();
                    CDevicesManager.Log.Debug("Nombre d'évenemets {0}", eventCounter);
                    return eventCounter;
                }
                set => eventCounter = value;
            }

            /// <summary>
            /// Lecture des status du hopper
            /// </summary>
            private void GetHopperStatus()
            {
                try
                {
                    CDevicesManager.Log.Info("Lecture des status du {0}", Owner.DeviceAddress);
                    byte[] bufferIn = { 0, 0, 0, 0 };
                    if (Owner.IsCmdccTalkSended(Owner.DeviceAddress, Header.REQUESTHOPPERSTATUS, 0, null, bufferIn))
                    {
                        EventCounter = bufferIn[0];
                        dispensedResult.CoinsRemaining = coinsRemaining = bufferIn[1];
                        dispensedResult.CoinsPaid = coinsPaid = bufferIn[2];
                        dispensedResult.MontantPaid = (int)(coinsPaid * Owner.CoinValue);
                        dispensedResult.CoinsUnpaid = coinsUnpaid = bufferIn[3];
                        dispensedResult.MontantUnpaid = (int)(coinsUnpaid * Owner.CoinValue);
                    }
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
            }

            /// <summary>
            /// Classe des resultats d'une distribution.
            /// </summary>
            public class CDispensedResult
            {
                /// <summary>
                /// Montant
                /// </summary>
                public int AmountToDispense;

                /// <summary>
                /// Nombre de pièces distribuées.
                /// </summary>
                public byte CoinsPaid;

                /// <summary>
                /// Nombre de pièce restant à payer.
                /// </summary>
                ///<remarks>Passe à zéro à la fin de la distribtion quelque soit la raison.</remarks>
                public byte CoinsRemaining;

                /// <summary>
                /// Nombre de pièces non distribuées
                /// </summary>
                public byte CoinsUnpaid;

                /// <summary>
                /// Nombre de pièces devant être distribuées
                /// </summary>
                public byte CoinToDispense;

                /// <summary>
                /// Montant distribué
                /// </summary>
                public int MontantPaid;

                /// <summary>
                /// Montant non distribué
                /// </summary>
                public int MontantUnpaid;

                /// <summary>
                /// Numéro du hopper.
                /// </summary>
                public string nameOfHopper;

                /// <summary>
                /// Constructeur
                /// </summary>
                /// <param name="hopperNumber"></param>
            }
        }
    }
}