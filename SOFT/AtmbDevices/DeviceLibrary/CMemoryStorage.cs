/// \file CMemoryStorage.cs
/// \brief Fichier contenant la classe CMemoryStorage
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
{
    /// <summary>
    /// Classe des paramètres de stockage des variables en mémoire.
    /// </summary>
    public class CMemoryStorage
    {
        /// <summary>
        /// Mode de gestion du maintien des informations en mémoire
        /// </summary>
        public enum MemoryKeepType : byte
        {
            /// <summary>
            /// Volatiles effacées par un reset.
            /// </summary>
            VOLATILLOSTONRESET = 0,
            /// <summary>
            /// Volatiles effacées lors de la coupure de l'alimentation
            /// </summary>
            VOLATILLOSTONPOWERDOWN = 1,
            /// <summary>
            /// Permanent usage limité
            /// </summary>
            PERMANENTLIMITED = 2,
            /// <summary>
            /// Permanent sans limite
            /// </summary>
            PERMANENTUNLIMITED = 3,
        }

        /// <summary>
        /// Contient le type de maintien de la mémoire
        /// </summary>
        private MemoryKeepType memoryType;

        /// <summary>
        /// Nombre de blocs de données valable en lecture.
        /// </summary>
        private byte readBlocks;

        /// <summary>
        /// Nombre d'octet contenu dans un bloc de lecture.
        /// </summary>
        private byte readBytesPerBlock;

        /// <summary>
        /// Nombre de blocs de données valable en écriture.
        /// </summary>
        private byte writeBlocks;

        /// <summary>
        /// Nombre d'octets contenus dans un bloc d'écriture.
        /// </summary>
        private byte writeBytesPerBlock;

        private CccTalk Owner;

        /// <summary>
        /// Constructeur
        /// </summary>
        public CMemoryStorage(CccTalk owner)
        {
            Owner = owner;
            GetDataStorageAvailability();
        }

        /// <summary>
        /// Lit les informations sur la gestion de la mémoire de sauvegarde
        /// </summary>
        public void GetDataStorageAvailability()
        {
            try
            {
                byte[] bufferIn = { 0, 0, 0, 0, 0 };
                CDevicesManage.Log.Info("Lecture des informations sur les capacités de lecture et écriture des données du {0} : ", Owner.DeviceAddress);
                if (Owner.IsCmdccTalkSended(Owner.DeviceAddress, CccTalk.Header.REQUESTDATASTORAGEAVAILABILITY, 0, null, bufferIn))
                {
                    memoryType = (MemoryKeepType)bufferIn[0];
                    readBlocks = bufferIn[1];
                    readBytesPerBlock = bufferIn[2];
                    writeBlocks = bufferIn[3];
                    writeBytesPerBlock = bufferIn[4];
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Type de maintien de la mémoire
        /// </summary>
        /// <returns></returns>
        public MemoryKeepType MemoryType
        {
            get
            {
                GetDataStorageAvailability();
                return memoryType;
            }
        }

        /// <summary>
        /// Nombre de blocs en lecture
        /// </summary>
        public byte ReadBlocks
        {
            get
            {
                GetDataStorageAvailability();
                return readBlocks;
            }
        }

        /// <summary>
        /// Nombre d'octets par bloc en lecture
        /// </summary>
        public byte ReadBytesPerBlock
        {
            get
            {
                GetDataStorageAvailability();
                return readBytesPerBlock;
            }
        }

        /// <summary>
        /// Nombre de blocs en lectures.
        /// </summary>
        public byte WriteBlocks
        {
            get
            {
                GetDataStorageAvailability();
                return writeBlocks;
            }
        }

        /// <summary>
        /// Nombre d'octets par bloc en lectures
        /// </summary>
        public byte WriteBytesPerBlock
        {
            get
            {
                GetDataStorageAvailability();
                return writeBytesPerBlock;
            }
        }

        /// <summary>
        /// Lecture d'un bloc de données.
        /// </summary>
        /// <param name="BlockNumber"></param>
        /// <param name="data"></param>
        public void GetDataBlock(byte BlockNumber, ref object data)
        {
            try
            {
                CDevicesManage.Log.Info("Lecture du bloc de données {0} du périphérique à l'adresse {1}", Owner.DeviceAddress);
                if (BlockNumber >= ReadBlocks)
                {
                    throw new Exception(string.Format("Le bloc {0} n'est pas accessible", BlockNumber));
                }
                byte[] bufferParam = { BlockNumber };
                CDevicesManage.Log.Info("Lecture du bloc de données {0} du périphérique à l'adresse {1}", Owner.DeviceAddress);
                if (!Owner.IsCmdccTalkSended(Owner.DeviceAddress, CccTalk.Header.READDATABLOCK, (byte)bufferParam.Length, bufferParam, data))
                {
                    throw new Exception(string.Format("Impossible de lire les données dans le bloc {0} du périphérique {1}", BlockNumber, Owner.DeviceAddress));
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Enregistrement d'un bloc de données.
        /// </summary>
        /// <param name="BlockNumber"></param>
        /// <param name="data"></param>
        public void SetDataBlock(byte BlockNumber, object data)
        {
            try
            {
                CDevicesManage.Log.Info("Ecriture du bloc de données {0} du périphérique à l'adresse {1}", Owner.DeviceAddress);
                if (BlockNumber >= ReadBlocks)
                {
                    throw new Exception(string.Format("Le bloc {0} n'est pas accessible", BlockNumber));
                }
                byte lenParam = (byte)(writeBytesPerBlock + 1);
                byte[] bufferParam = new byte[lenParam + 1];
                bufferParam[0] = BlockNumber;
                Buffer.BlockCopy((byte[])data, 0, bufferParam, 1, writeBytesPerBlock);
                if (!Owner.IsCmdccTalkSended(Owner.DeviceAddress, CccTalk.Header.WRITEDATABLOCK, lenParam, bufferParam, null))
                {
                    throw new Exception(string.Format("Impossible d'écrire le bloc {0} dans le périphérique {1}", BlockNumber, Owner.DeviceAddress));
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }
    }
}