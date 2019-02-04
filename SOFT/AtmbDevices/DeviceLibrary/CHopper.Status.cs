/// \file CHopper.Status.cs
/// \brief Fichier contenant la classe CHopperStatus
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
{
        /// <summary>
        /// Classe des status des hoppers
        /// </summary>
        public partial class CHopperStatus
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
            public CHopperDispensedResult dispensedResult;

            /// <summary>
            /// Contructeur;
            /// </summary>
            /// <param name="owner"></param>
            public CHopperStatus(CHopper owner)
            {
                Owner = owner;
                dispensedResult = new CHopperDispensedResult
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
                    if (CccTalk.IsCmdccTalkSended(Owner.DeviceAddress, CHopper.Header.REQUESTHOPPERSTATUS, 0, null, bufferIn))
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
        }
}