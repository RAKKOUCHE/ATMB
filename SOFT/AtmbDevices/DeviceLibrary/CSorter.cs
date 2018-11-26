using System;
namespace DeviceLibrary
{
    public partial class CCanal
    {

        /// <summary>
        /// 
        /// </summary>
        public class CSorter
        {
            /// <summary>
            /// 
            /// </summary>
            private enum Header : byte
            {
                REQUESTSORTERPATH = 209,
                MODIFYSORTERPATH = 210,
            }

            /// <summary>
            /// 
            /// </summary>
            private CCanal CanalOwner;

            /// <summary>
            /// Chemin utilisé dans le trieur de 1 à 8
            /// </summary>
            public byte PathSorter
            {
                get => SorterPath;
                set => SetSorterPath(value);
            }

            /// <summary>
            /// 
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
                        CDevicesManage.Log.Info("Lecture des informations sur le tri des pièces en sortie du canal {0} du {1}", CanalOwner.Number, CanalOwner.CVOwner.DeviceAddress);
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
                        CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                    CDevicesManage.Log.Info("Modifie le chemin de sortie du trieur pour le canal {0} chemin {1}", CanalOwner.Number, pathSorter);
                    byte[] bufferParam = { CanalOwner.Number, pathSorter, OverPath[0], OverPath[1], OverPath[2] };
                    if (!CanalOwner.CVOwner.IsCmdccTalkSended(CanalOwner.CVOwner.DeviceAddress, Header.MODIFYSORTERPATH, (byte)bufferParam.Length, bufferParam, null))
                    {
                        throw new Exception(string.Format(messagesText.erreurCmd, Header.MODIFYSORTERPATH, CanalOwner.CVOwner.DeviceAddress));
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
            /// <param name="owner">Canal correspondant</param>
            public CSorter(CCanal owner)
            {
                CanalOwner = owner;
                OverPath = new byte[] { 1, 1, 1 };
            }
        }
    }
}