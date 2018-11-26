using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace DeviceLibrary
{
    public partial class CccTalk : CDevice
    {

        /// <summary>
        /// Nom du fichier contenant les compteurs.
        /// </summary>
        public const string fileCounterName = "Counters.mtr";

        /// <summary>
        /// Verrou du thread
        /// </summary>
        protected static object verrou = new object();

        private DefaultDevicesAddress deviceAddress;
        /// <summary>
        /// Adresse du périphérique.
        /// </summary>
        public DefaultDevicesAddress DeviceAddress
        {
            get => deviceAddress;
            set => deviceAddress = value;
        }

        /// <summary>
        /// Taille des buffers
        /// </summary>
        private const byte sizeOfBufferOut = 32;

        /// <summary>
        /// Port série utilisée par le bus ccTalk
        /// </summary>
        public static SerialPort PortSerie;

        /// <summary>
        /// 
        /// </summary>
        public static Mutex mutexCCTalk = new Mutex();

        /// <summary>
        /// Compteurs 
        /// </summary>
        public static CcoinsCounters counters;

        /// <summary>
        /// Fichier des compteurs.
        /// </summary>
        public static Stream countersFile;

        /// <summary>
        /// 
        /// </summary>
        public static BinaryFormatter counterSerializer;

        /// <summary>
        /// Construteur de la class
        /// </summary>
        public CccTalk()
        {
            try
            {
                CDevicesManage.Log.Info(messagesText.ccTalkInstance);

                if (PortSerie == null)
                {
                    CDevicesManage.Log.Info(messagesText.search_ccTalk);
                    PortSerie = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One)
                    {
                        ReadTimeout = 100,
                        WriteTimeout = 100
                    };
                    string[] ports = SerialPort.GetPortNames();
                    foreach (string port in ports)
                    {
                        if (IsCcTalkPort(port))
                        {
                            CDevicesManage.Log.Info(messagesText.busOKfinded, port);
                            break;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Vérifie si un bus ccTal est brancé sur un port série.
        /// </summary>
        /// <bufferParam name="NameOfPort"> Nom du port série à vérifier</bufferParam>
        /// <returns></returns>
        private bool IsCcTalkPort(string NameOfPort)
        {
            CDevicesManage.Log.Info(messagesText.verifSerialPort, NameOfPort);
            try
            {
                byte[] bufferIn = { 0x00, 0xFF, 0x55, 0xAA };
                byte[] bufferOut = { 0XFF, 0X00, 0XAA, 0X55 };
                PortSerie.PortName = NameOfPort;
                PortSerie.Open();
                PortSerie.Write(bufferOut, 0, 4);
                for (byte byIndex = 0; byIndex < 4; byIndex++)
                {
                    bufferIn[byIndex] = (byte)PortSerie.ReadByte();
                    if (!(bufferOut[byIndex] == bufferIn[byIndex]))
                    {
                        throw new Exception(string.Format(messagesText.erreurPort, NameOfPort));
                    }
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error("{0} {1}", E.GetType(), E.Message);
                PortSerie.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vide le buffer passé en paramètre
        /// </summary>
        /// <bufferParam name="Buffer">Buffer devant être vidé</bufferParam>
        /// <bufferParam name="Len">Longueur du buffer</bufferParam>
        private void ZeroMemory(byte[] Buffer, byte Len)
        {
            try
            {
                for (byte Index = 0; Index < Len; Index++)
                {
                    Buffer[Index] = 0;
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Envoie une commande et recoit la réponse
        /// </summary>
        /// <param name="Peripherique">Adresse du périphérique</param>
        /// <param name="Commande">Header de la commande envoyée</param>
        /// <param name="LenParam">Longeur des paramètres</param>
        /// <param name="Parameter">Buffer des paramètres</param>
        /// <param name="answer">Objet contenant les paramètres de retour le cas échéant</param>
        /// <returns>True si la commande et le retour s'effectuent correctement</returns>
        public bool IsCmdccTalkSended(DefaultDevicesAddress Peripherique, object Commande, byte LenParam, byte[] Parameter, object answer)
        {
            bool result = false;
            lock (verrou)
            {
                string strLog = messagesText.txtMessageSended;
                try
                {
                    byte byIndex;
                    byte[] bufferIn = new byte[32];
                    for (int i = 0; i < bufferIn.Length; i++)
                    {
                        bufferIn[i] = 0XFF;
                    }
                    CDevicesManage.Log.Info(messagesText.sendCmd, Commande.ToString(), Peripherique.ToString());
                    byte[] bufferOut = new byte[sizeOfBufferOut];
                    ZeroMemory(bufferOut, sizeOfBufferOut);
                    bufferOut[0] = (byte)Peripherique;
                    bufferOut[1] = LenParam;
                    bufferOut[2] = (byte)DefaultDevicesAddress.Host;
                    bufferOut[3] = Convert.ToByte(Commande);
                    if (LenParam > 0)
                    {
                        Buffer.BlockCopy(Parameter, 0, bufferOut, 4, LenParam);
                    }
                    bufferOut[4 + LenParam] = CheckSum(bufferOut, LenParam);

                    for (byIndex = 0; byIndex < (LenParam + 5); byIndex++)
                    {
                        strLog += string.Format("{0} ", bufferOut[byIndex]);
                    }
                    CDevicesManage.Log.Debug(strLog);
                    PortSerie.Write(bufferOut, 0, 5 + LenParam);
                    CDevicesManage.Log.Debug(messagesText.readEcho);
                    strLog = messagesText.echo;
                    for (byIndex = 0; byIndex < (LenParam + 5); byIndex++)
                    {
                        bufferIn[byIndex] = (byte)PortSerie.ReadByte();
                        strLog += string.Format("{0} ", bufferIn[byIndex]);
                    }
                    CDevicesManage.Log.Debug(strLog);
                    CDevicesManage.Log.Debug(messagesText.readAnswerDevice);
                    strLog = messagesText.txtAnswer;
                    for (byIndex = 0; byIndex < 2; byIndex++)
                    {
                        bufferIn[byIndex] = (byte)PortSerie.ReadByte();
                        strLog += string.Format("{0} ", bufferIn[byIndex]);
                    }
                    for (; byIndex < bufferIn[1] + 5; byIndex++)
                    {
                        bufferIn[byIndex] = (byte)PortSerie.ReadByte();
                        strLog += string.Format("{0} ", bufferIn[byIndex]);
                    }
                    if ((bufferIn[bufferIn[1] + 4] != 0) && (CheckSum(bufferIn, bufferIn[1]) != bufferIn[bufferIn[1] + 4]))
                    {
                        throw new Exception("Checksum erreur");
                    }
                    if (bufferIn[3] == (byte)Header.NAK)
                    {
                        throw new Exception("ccTalk erreur :" + Header.NAK.ToString());
                    }
                    if (bufferIn[3] == (byte)Header.BUSY)
                    {
                        throw new Exception("ccTal erreur :" + Header.BUSY);
                    }
                    if (answer != null)
                    {
                        Buffer.BlockCopy(bufferIn, 4, (System.Array)answer, 0, bufferIn[1]);
                    }
                    result = true;
                }
                catch (TimeoutException E)
                {
                    CDevicesManage.Log.Error(messagesText.noccTalkDevice, E.Message, PortSerie.PortName);
                }
                catch (Exception E)
                {
                    {
                        CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                    }
                }
                CDevicesManage.Log.Debug(strLog);
                CDevicesManage.Log.Info("Commande {0} executée sur le {1}", Commande, Peripherique);
            }
            return result;
        }

        /// <summary>
        /// Retourne le checksum dy buffer
        /// </summary>
        /// <bufferParam name="Buffer">Buffer sur lequel le calcul s'effectue</bufferParam>
        /// <bufferParam name="Len">Longeur du buffer</bufferParam>
        private byte CheckSum(byte[] Buffer, byte Len)
        {
            uint Result = 0;
            try
            {
                for (byte Index = 0; Index < Len + 4; Index++)
                {
                    Result += Buffer[Index];
                }
                Result = (256 - (Result % 256));
            }
            catch (Exception E)
            {
                {
                    CDevicesManage.Log.Error("{0} {1}", E.GetType(), E.Message);
                }
            }
            return (byte)Result;
        }

        /// <summary>
        /// Retourne l'identifiant du fabricant.
        /// </summary>
        public override string Manufacturer
        {
            get
            {
                CDevicesManage.Log.Info(messagesText.getManufacturer, DeviceAddress);
                string manufacturer = GetASCII(Header.REQUESTMANUFACTURERID);
                CDevicesManage.Log.Info(messagesText.manufacturer, DeviceAddress, manufacturer);
                return manufacturer;
            }
        }

        /// <summary>
        /// Code de production du périphérique.
        /// </summary>
        protected string BuildCode
        {
            get
            {
                CDevicesManage.Log.Info(messagesText.getBuildCode, DeviceAddress);
                string buildCode = GetASCII(Header.REQUESTBUILDCODE);
                CDevicesManage.Log.Info(messagesText.buildCode, DeviceAddress, buildCode);
                return buildCode;
            }
        }

        /// <summary>
        /// Code produit du périphérique.
        /// </summary>
        public override string ProductCode
        {
            get
            {
                CDevicesManage.Log.Info(messagesText.getProductCode, DeviceAddress);
                string result = GetASCII(Header.REQUESTPRODUCTCODE);
                CDevicesManage.Log.Info(messagesText.productCode, DeviceAddress, result);
                return result;
            }
        }

        /// <summary>
        /// Code produit du périphérique.
        /// </summary>
        protected string EquipementCategory
        {
            get
            {
                CDevicesManage.Log.Info(messagesText.getEquipementID, DeviceAddress);
                string equipement = GetASCII(Header.REQUESTEQUIPEMENTCATEGORYID);
                CDevicesManage.Log.Info(messagesText.equipementID, DeviceAddress, equipement);
                return equipement;
            }
        }

        /// <summary>
        /// Lit un octet dans le périphérique
        /// </summary>
        /// <param name="header">Header de la requête</param>
        /// <remarks>La valeur de l'octet retourné est 255</remarks>
        /// <returns>la valeur de l'octet demandé</returns>
        protected byte GetByte(object header)
        {
            byte[] result = { 0 };
            try
            {
                CDevicesManage.Log.Debug(messagesText.getByte, DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, header, 0, null, result))
                {
                    CDevicesManage.Log.Error(messagesText.erreurCmd, header, DeviceAddress);
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return result[0];
        }

        /// <summary>
        /// Lit une chaîne ascii dans le périphérique.
        /// </summary>
        /// <bufferParam name="header">Header de la requête</bufferParam>
        /// <remarks>En cas d'echec, la chaine retournée est vide</remarks>
        /// <returns>La chaine de caractères demandée</returns>
        private string GetASCII(Header header)
        {
            byte[] bufferIn = new byte[32];
            string result = string.Empty;
            try
            {
                CDevicesManage.Log.Debug(messagesText.getText, DeviceAddress);
                if (IsCmdccTalkSended(DeviceAddress, header, 0, null, bufferIn))
                {
                    foreach (byte item in bufferIn)
                    {
                        if (item > 0)
                        {
                            result += (char)item;
                        }
                    }
                    //return System.Text.Encoding.Default.GetString(bufferIn);
                }
                else
                {
                    CDevicesManage.Log.Error(messagesText.erreurCmd, header, DeviceAddress);
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Retourne le numéro de série du pelicano
        /// </summary>
        public override int SerialNumber
        {
            get
            {
                byte[] bufferIn = { 0, 0, 0 };
                try
                {
                    CDevicesManage.Log.Info(messagesText.getSN, DeviceAddress);
                    if (!IsCmdccTalkSended(DeviceAddress, Header.REQUESTSN, 0, null, bufferIn))
                    {
                        CDevicesManage.Log.Error(messagesText.erreurCmd, Header.REQUESTSN, DeviceAddress);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                CDevicesManage.Log.Info("Le numéro de série du {0} est {1}", DeviceAddress, bufferIn[0] + (0x100 * bufferIn[1]) + (0x10000 + bufferIn[2]));
                return bufferIn[0] + (0x100 * bufferIn[1]) + (0x10000 + bufferIn[2]);
            }
        }

        /// <summary>
        /// Retourne la révision software
        /// </summary>
        protected string SWRev
        {
            get
            {
                try
                {
                    CDevicesManage.Log.Info(messagesText.getSWRev, DeviceAddress);
                    string swRev = GetASCII(Header.REQUESTSWREV);
                    CDevicesManage.Log.Info(messagesText.swRev, DeviceAddress, swRev);
                    return swRev;
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string CommsRevision
        {
            get
            {
                byte[] bufferIn = { 0x30, 0x30, 0x30 };
                string result = "";
                try
                {
                    if (IsCmdccTalkSended(DeviceAddress, Header.REQUESTCOMMSREVISION, 0, null, bufferIn))
                    {
                        result = (char)(bufferIn[0] + 0x30) + "." +
                        (char)(bufferIn[1] + 0x30) + "." +
                        (char)(bufferIn[2] + 0x30);
                        CDevicesManage.Log.Info("La version ccTalk du {0} est {1}", DeviceAddress, result);
                    }
                    else
                    {
                        CDevicesManage.Log.Error("Impossible de lire les informations de la version ccTalk dans le {0}", DeviceAddress);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// StateReset software du périphérique.
        /// </summary>
        protected bool ResetDevice()
        {
            bool result = false;
            try
            {
                CDevicesManage.Log.Info("Reset du {0}", DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, Header.RESETDEVICE, 0, null, null))
                {
                    Thread.Sleep(200);
                    if (!IsCmdccTalkSended(DeviceAddress, Header.RESETDEVICE, 0, null, null))
                    {
                        throw new Exception(string.Format(messagesText.erreurCmd, Header.RESETDEVICE, DeviceAddress));
                    }
                }
                CDevicesManage.Log.Info("Reset du {0} effectué.", DeviceAddress);
                Thread.Sleep(200);
                result = true;
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Simple poll du périphérique.
        /// </summary>
        /// <returns></returns>
        public bool SimplePoll
        {
            get
            {
                try
                {
                    CDevicesManage.Log.Info("Simple poll du {0}", DeviceAddress);
                    if (IsCmdccTalkSended(DeviceAddress, Header.SIMPLEPOLL, 0, null, null))
                    {
                        CDevicesManage.Log.Info("Simple poll du {0} effectué.", DeviceAddress);
                        return true;
                    }
                    CDevicesManage.Log.Error(messagesText.erreurCmd, Header.SIMPLEPOLL, DeviceAddress);
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return false;
            }
        }

        /// <summary>
        /// Remise à zéro des compteurs
        /// </summary>
        public static void ResetCounters()
        {
            counters = null;
            counters = new CcoinsCounters();
            counters.SaveCounters();
            CccTalk.countersFile.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual byte OptoStates
        {
            get
            {
                byte result = 0XFF;
                try
                {

                    CDevicesManage.Log.Info("Lecture de l'état des optos du {0}", DeviceAddress);
                    result = GetByte(Header.READOPTOSTATES);
                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Destructeur
        /// </summary>
        ~CccTalk()
        {
            try
            {
                countersFile.Close();
            }
            catch (Exception)
            {

            }

        }

    }
}