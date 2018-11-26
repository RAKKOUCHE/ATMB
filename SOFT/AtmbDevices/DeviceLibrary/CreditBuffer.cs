﻿using NLog;
using System;

namespace DeviceLibrary
{

    public partial class CCoinValidator : CcashReader
    {
        /// <summary>
        /// 
        /// </summary>
        public class CCVcreditBuffer
        {
            private CCoinValidator owner;

            /// <summary>
            /// Compteur d'évenements crédits ou erreurs
            /// </summary>
            private byte eventCounter;
            public byte EventCounter
            {
                get => eventCounter;
                set => eventCounter = value;
            }

            /// <summary>
            /// Buffer contenant les informations sur les évenements.
            /// </summary>
            private byte[,] result;
            public byte[,] Result
            {
                get => result;
                set => result = value;
            }

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
            /// Lit le buffer de credit ou de code erreur.
            /// </summary>
            /// <remarks>Header 229</remarks>
            public void GetBufferCredit()
            {
                try
                {
                    CDevicesManage.Log.Info("Lecture du buffer de credit ou des code d'erreur {0}", owner.DeviceAddress);
                    byte[] bufferIn = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    if (owner.IsCmdccTalkSended(owner.DeviceAddress , Header.READBUFFERCREDIT, 0, null, bufferIn))
                    {
                        EventCounter = bufferIn[0];
                        Buffer.BlockCopy(bufferIn, 1, Result, 0, 10);
                        CDevicesManage.Log.Debug("Le buffer des credits ou des erreurs du {0} est pour (event counter : {1}) (result1 : {2}-{3}) (result2 : {4}-{5}) (result3 : {6}-{7}) (result4 : {8}-{9}) (result5 : {10}-{11})", owner.DeviceAddress, EventCounter, Result[0, 0], Result[0, 1], Result[1, 0], Result[1, 1], Result[2, 0], Result[2, 1], Result[3, 0], Result[3, 1], Result[4, 0], Result[4, 1]);
                    }
                    else
                    {
                        throw new Exception("Impossible de lire le buffer des crédits ou des codes erreurs.");
                    }
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }        
        }
    }
}