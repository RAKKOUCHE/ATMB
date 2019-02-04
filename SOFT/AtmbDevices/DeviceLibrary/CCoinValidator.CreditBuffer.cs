/// \file CCoinValidator.CreditBuffer.cs
/// \brief Fichier contenant la classe CCVcreditBuffer.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
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
        public CCVcreditBuffer(CCoinValidator owner)
        {
            this.owner = owner;
            EventCounter = 0;
            SetResult(new byte[,] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } });
        }

        /// <summary>
        /// Compteur d'évenements crédits ou erreurs
        /// </summary>
        public byte EventCounter
        {
            get => eventCounter;
            private set => eventCounter = value;
        }

        /// <summary>
        /// Buffer contenant les informations sur les évenements.
        /// </summary>
        private void SetResult(byte[,] value)
        {
            result = value;
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
                if (CccTalk.IsCmdccTalkSended(owner.DeviceAddress, CCoinValidator.Header.READBUFFERCREDIT , 0, null, bufferIn))
                {
                    EventCounter = bufferIn[0];
                    Buffer.BlockCopy(bufferIn, 1, GetResult(), 0, 10);
                    CDevicesManager.Log.Debug("Le buffer des credits ou des erreurs du {0} est pour (event counter : {1}) (result1 : {2}-{3}) (result2 : {4}-{5}) (result3 : {6}-{7}) (result4 : {8}-{9}) (result5 : {10}-{11})", owner.DeviceAddress, EventCounter, GetResult()[0, 0], GetResult()[0, 1], GetResult()[1, 0], GetResult()[1, 1], GetResult()[2, 0], GetResult()[2, 1], GetResult()[3, 0], GetResult()[3, 1], GetResult()[4, 0], GetResult()[4, 1]);
                }
                else
                {
                    throw new Exception("Impossible de lire le buffer des crédits ou des codes erreurs.");
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Buffer contenant les informations sur les évenements.
        /// </summary>
        public byte[,] GetResult()
        {
            return result;
        }
    }
}