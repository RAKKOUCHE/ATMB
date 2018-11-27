using System;

namespace DeviceLibrary
{
    public partial class CCanal
    {
        /// <summary>
        /// Class gérant l'identification des pièces
        /// </summary>
        public class CCoindID
        {
            /// <summary>
            /// Command ccTalk demandant l'identification de la pièce du canal.
            /// </summary>
            private const byte REQUESTCOINID = 184;

            /// <summary>
            /// Instance du canal de la pièce.
            /// </summary>
            private CCanal CanalOwner;

            private string countryCode;
            /// <summary>
            /// Code d'identification du pays émetteur.
            /// </summary>
            public string CountryCode
            {
                get => countryCode;
                set => countryCode = value;
            }

            private byte valeurCent;
            /// <summary>
            /// Valeur de la pièces.
            /// </summary>
            public byte ValeurCent
            {
                get => valeurCent;
                set => valeurCent = value;
            }

            private char issue;
            /// <summary>
            /// Version des données d'identification.
            /// </summary>
            public char Issue
            {
                get => issue;
                set => issue = value;
            }

            /// <summary>
            /// Lit l'identification de la pièce acceptée dans le canal passé en paramètre.
            /// </summary>
            /// <remarks>voir <see cref = "REQUESTCOINID"/> </remarks>
            public void GetCoinId()
            {
                try
                {
                    CDevicesManage.Log.Info("Lecture de l'identification de la pièce acceptée dans le canal {0} du {1}", CanalOwner.Number, CanalOwner.CVOwner.DeviceAddress);
                    byte[] bufferIn = { 46, 46, 46, 46, 46, 46 };
                    byte[] bufferParam = { CanalOwner.Number };
                    if (CanalOwner.CVOwner.IsCmdccTalkSended(CanalOwner.CVOwner.DeviceAddress, REQUESTCOINID, (byte)bufferParam.Length, bufferParam, bufferIn))
                    {
                        CountryCode = ((char)bufferIn[0]).ToString() + ((char)bufferIn[1]).ToString();
                        if (CountryCode == "..")
                        {
                            valeurCent = 0;
                            Issue = '-';
                        }
                        else
                        {
                            ValeurCent = (byte)(((bufferIn[2] - 0x30) * 100) + ((bufferIn[3] - 0x30) * 10) + (bufferIn[4] - 0x30));
                            Issue = (char)bufferIn[5];
                            CDevicesManage.Log.Info("Canal {0} du {1} : Le code pays est {2}, la valeur est de {3:C2}, la version est {4}", CanalOwner.Number, CanalOwner.CVOwner.DeviceAddress, CountryCode, (decimal)ValeurCent / 100, Issue);
                        }
                    }
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }

            /// <summary>
            /// Constructeur
            /// </summary>
            /// <param name="owner">Le canal correspondant</param>
            public CCoindID(CCanal owner)
            {
                CanalOwner = owner;
                CountryCode = "..";
                ValeurCent = 0;
                Issue = '.';
            }
        }
    }
}
