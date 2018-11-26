using System;

namespace DeviceLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class CMemoryStorage
    {
        /// <summary>
        /// 
        /// </summary>
        public enum MemoryKeepType : byte
        {
            VOLATILLOSTONRESET = 0,
            VOLATILLOSTONPOWERDOWN = 1,
            PERMANENTLIMITED = 2,
            PERMANENTUNLIMITED = 3,
        }

        /// <summary>
        /// 
        /// </summary>
        private MemoryKeepType memoryType;

        /// <summary>
        /// 
        /// </summary>
        private byte readBlocks;

        /// <summary>
        /// 
        /// </summary>
        private byte readBytesPerBlock;

        /// <summary>
        /// 
        /// </summary>
        private byte writeBlocks;

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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