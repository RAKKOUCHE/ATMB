/// \file CCoinValidator.CreditBuffer.cs
/// \brief Fichier contenant la classe CCVcreditBuffer.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
{
    /// <summary>
    /// Class gérant le monnayeur.
    /// </summary>
    public partial class CCoinValidator : CcashReader
    {
        /// <summary>
        /// Classe du buffer des credits ou des codes erreurs.
        /// </summary>
        public class CCVcreditBuffer
        {
            /// <summary>
            /// Instance de la classe proprietaire du buffer.
            /// </summary>
            private readonly CCoinValidator owner;

            private byte eventCounter;

            private byte[,] result;

            /// <summary>
            /// Contructeur
            /// </summary>
            public CCVcreditBuffer(CCoinValidator Owner)
            {
                owner = Owner;
                EventCounter = 0;
                Result = new byte[,] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
            }

            /// <summary>
            /// Compteur d'évenements crédits ou erreurs
            /// </summary>
            public byte EventCounter
            {
                get => eventCounter;
                set => eventCounter = value;
            }

            /// <summary>
            /// Buffer contenant les informations sur les évenements.
            /// </summary>
            public byte[,] Result
            {
                get => result;
                set => result = value;
            }

            /// <summary>
            /// Lit le buffer de credit ou de code erreur.
            /// </summary>
            /// <remarks>Header 229</remarks>
            public void GetBufferCredit()
            {
                try
                {
                    CDevicesManager.Log.Info("Lecture du buffer de credit ou des code d'erreur {0}", owner.DeviceAddress);
                    byte[] bufferIn = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    if (owner.IsCmdccTalkSended(owner.DeviceAddress, Header.READBUFFERCREDIT, 0, null, bufferIn))
                    {
                        EventCounter = bufferIn[0];
                        Buffer.BlockCopy(bufferIn, 1, Result, 0, 10);
                        CDevicesManager.Log.Debug("Le buffer des credits ou des erreurs du {0} est pour (event counter : {1}) (result1 : {2}-{3}) (result2 : {4}-{5}) (result3 : {6}-{7}) (result4 : {8}-{9}) (result5 : {10}-{11})", owner.DeviceAddress, EventCounter, Result[0, 0], Result[0, 1], Result[1, 0], Result[1, 1], Result[2, 0], Result[2, 1], Result[3, 0], Result[3, 1], Result[4, 0], Result[4, 1]);
                    }
                    else
                    {
                        throw new Exception("Impossible de lire le buffer des crédits ou des codes erreurs.");
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }
        }
    }
}