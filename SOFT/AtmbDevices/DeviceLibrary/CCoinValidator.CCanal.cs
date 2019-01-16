/// \file CCoinValidator.cs
/// \brief Fichier contenant la classe CCoinValidator
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
{
    /// <summary>
    /// Class gérant le monnayeur.
    /// </summary>
    public partial class CCoinValidator
    {
        /// <summary>
        /// Class d'un canal d'un périphérique de paiement
        /// </summary>
        /// \details Cette classe contient et gère les paramètres d'un canal*/
        public partial class CCanal
        {
            /// <summary>
            /// Class gérant l'identification des pièces
            /// </summary>
            public class CCoindID
            {
                /// \addtogroup Headers
                /// @{
                /// <summary>
                /// Groupe des commandes ccTalk.
                /// </summary>
                /// <summary>
                /// Command ccTalk demandant l'identification de la pièce du canal.
                /// </summary>
                private const byte REQUESTCOINID = 184;

                /** @}*/

                /// <summary>
                /// Instance du canal de la pièce.
                /// </summary>
                private readonly CCanal CanalOwner;

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
                        CDevicesManager.Log.Info("Lecture de l'identification de la pièce acceptée dans le canal {0} du {1}", CanalOwner.Number, CanalOwner.CVOwner.DeviceAddress);
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
                                CDevicesManager.Log.Info("Canal {0} du {1} : Le code pays est {2}, la valeur est de {3:C2}, la version est {4}", CanalOwner.Number, CanalOwner.CVOwner.DeviceAddress, CountryCode, (decimal)ValeurCent / 100, Issue);
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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

            /// <summary>
            /// Class gérant les informations concernant un chemin de triage d'un canal
            /// </summary>
            public class CSorter
            {
                /// <summary>
                /// Header ccTalk utilisé pour le triage
                /// </summary>
                private enum Header : byte
                {
                    ///<summary>
                    ///Demande du chemin utilisé pour trier une pièce reconnue
                    ///</summary>
                    REQUESTSORTERPATH = 209,

                    ///<summary>
                    ///Enregistre chemin utilisé pour trier une pièce reconnue
                    ///</summary>
                    MODIFYSORTERPATH = 210,
                }

                /// <summary>
                /// Canal propriétaire de la class
                /// </summary>
                private readonly CCanal CanalOwner;

                /// <summary>
                /// Chemin utilisé dans le trieur (de 1 à 8)
                /// </summary>
                public byte PathSorter
                {
                    get => SorterPath;
                    set => SetSorterPath(value);
                }

                /// <summary>
                /// Chemin de substitusion.
                /// </summary>
                public byte[] OverPath;

                /// <summary>
                /// Lit les informations concernant le tri des pièces en sortie du monnayeur.
                /// </summary>
                private byte SorterPath
                {
                    get
                    {
                        byte result = 1;
                        try
                        {
                            CDevicesManager.Log.Info("Lecture des informations sur le tri des pièces en sortie du canal {0} du {1}", CanalOwner.Number, CanalOwner.CVOwner.DeviceAddress);
                            byte[] bufferIn = { 0, 0, 0, 0 };
                            byte[] bufferParam = { CanalOwner.Number };
                            if (!CanalOwner.CVOwner.IsCmdccTalkSended(CanalOwner.CVOwner.DeviceAddress, Header.REQUESTSORTERPATH, (byte)bufferParam.Length, bufferParam, bufferIn))
                            {
                                throw new Exception(string.Format("Impossible de lire les  informations sur le tri des pièces en sortie du canal {0} du {1}", CanalOwner.Number, CanalOwner.CVOwner.DeviceAddress));
                            }
                            result = bufferIn[0];
                            Buffer.BlockCopy(bufferIn, 0, OverPath, 0, 3);
                        }
                        catch (Exception E)
                        {
                            CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                        }
                        return result;
                    }
                }

                /// <summary>
                /// Definie le chemin utilisé par le trieur pour ce canal.
                /// </summary>
                /// <param name="pathSorter">Chemin dans le trieur</param>
                public void SetSorterPath(byte pathSorter)
                {
                    try
                    {
                        CDevicesManager.Log.Info("Modifie le chemin de sortie du trieur pour le canal {0} chemin {1}", CanalOwner.Number, pathSorter);
                        byte[] bufferParam = { CanalOwner.Number, pathSorter, OverPath[0], OverPath[1], OverPath[2] };
                        if (!CanalOwner.CVOwner.IsCmdccTalkSended(CanalOwner.CVOwner.DeviceAddress, Header.MODIFYSORTERPATH, (byte)bufferParam.Length, bufferParam, null))
                        {
                            throw new Exception(string.Format(messagesText.erreurCmd, Header.MODIFYSORTERPATH, CanalOwner.CVOwner.DeviceAddress));
                        }
                    }
                    catch (Exception E)
                    {
                        CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                    }
                }

                /// <summary>
                /// Constructeur
                /// </summary>
                /// <param name="owner">Canal correspondant</param>
                public CSorter(CCanal owner)
                {
                    CanalOwner = owner;
                    OverPath = new byte[] { 1, 1, 1 };
                }
            }

            /// <summary>
            /// Instance propriètaire du canal
            /// </summary>
            protected CCoinValidator CVOwner;

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
                get => counters.coinsInAccepted[Number - 1];
                set => counters.coinsInAccepted[Number - 1] = value;
            }

            /// <summary>
            /// Montant introduit dans le canal
            /// </summary>
            public long MontantIn
            {
                get => counters.amountCoinInAccepted[Number - 1];
                set => counters.amountCoinInAccepted[Number - 1] = value;
            }

            /// <summary>
            /// Nombre de pièces de ce canal dans la caisse
            /// </summary>
            public long CoinInInCB
            {
                get => counters.coinInCashBox[Number - 1];
                set => counters.coinInCashBox[Number - 1] = value;
            } 

            /// <summary>
            /// Montant des pièces de ce canal en caisse.
            /// </summary>
            public long AmountCoinInCB
            {
                get => counters.amountCoinInCashBox[Number - 1];
                set => counters.amountCoinInCashBox[Number - 1] = value;
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
}