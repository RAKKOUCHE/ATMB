﻿/// \file CCtalk.cs
/// \brief Fichier contenant la classe CccTalk
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace DeviceLibrary
{
    /// <summary>
    /// Class ccTalk
    /// </summary>
    public abstract partial class CccTalk : CDevice
    {
        /// <summary>
        /// Taille des buffers
        /// </summary>
        private const byte sizeOfBufferOut = 32;

        /// <summary>
        /// Adresse par defaut du péripérique;
        /// </summary>
        private DefaultDevicesAddress deviceAddress;

        /// <summary>
        /// Verrou du thread
        /// </summary>
        protected static object verrou = new object();

        /// <summary>
        /// Nom du fichier contenant les compteurs.
        /// </summary>
        public const string fileCounterName = "Counters.mtr";

        /// <summary>
        /// Compteurs
        /// </summary>
        public static CcoinsCounters counters;

        /// <summary>
        ///
        /// </summary>
        public static BinaryFormatter counterSerializer;

        /// <summary>
        /// Fichier des compteurs.
        /// </summary>
        public static Stream countersFile;

        /// <summary>
        ///
        /// </summary>
        public static Mutex mutexCCTalk = new Mutex();

        /// <summary>
        /// Port série utilisée par le bus ccTalk
        /// </summary>
        public static SerialPort PortSerie;

        /// <summary>
        /// Construteur de la class
        /// </summary>
        protected CccTalk()
        {
            try
            {
                CDevicesManager.Log.Debug(messagesText.ccTalkInstance);
                if (PortSerie == null)
                {
                    CDevicesManager.Log.Info(messagesText.search_ccTalk);
                    string[] ports = SerialPort.GetPortNames();
                    foreach (string port in ports)
                    {
                        PortSerie = new SerialPort(port, 9600, Parity.None, 8, StopBits.One)
                        {
                            ReadTimeout = 100,
                            WriteTimeout = 100,
                        };
                        if (IsCcTalkPort(port))
                        {
                            CDevicesManager.Log.Info(messagesText.busOKfinded, port);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Code de production du périphérique.
        /// </summary>
        protected string BuildCode
        {
            get
            {
                string buildCode = string.Empty;
                try
                {
                    CDevicesManager.Log.Info(messagesText.getBuildCode, DeviceAddress);
                    buildCode = GetASCII(Header.REQUESTBUILDCODE);
                    CDevicesManager.Log.Info(messagesText.buildCode, DeviceAddress, buildCode);
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }

                return buildCode;
            }
        }

        /// <summary>
        /// Lecture de version ccTalk utilisée par le périphérique.
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
                        CDevicesManager.Log.Info("La version ccTalk du {0} est {1}", DeviceAddress, result);
                    }
                    else
                    {
                        CDevicesManager.Log.Error("Impossible de lire les informations de la version ccTalk dans le {0}", DeviceAddress);
                    }
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
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
                string equipementCategory = string.Empty;
                try
                {
                    CDevicesManager.Log.Info(messagesText.getEquipementID, DeviceAddress);
                    equipementCategory = GetASCII(Header.REQUESTEQUIPEMENTCATEGORYID);
                    CDevicesManager.Log.Info(messagesText.equipementID, DeviceAddress, equipementCategory);
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }

                return equipementCategory;
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
                    CDevicesManager.Log.Info(messagesText.getSWRev, DeviceAddress);
                    string swRev = GetASCII(Header.REQUESTSWREV);
                    CDevicesManager.Log.Info(messagesText.swRev, DeviceAddress, swRev);
                    return swRev;
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                return "";
            }
        }

        /// <summary>
        /// Adresse du périphérique.
        /// </summary>
        public DefaultDevicesAddress DeviceAddress
        {
            get => deviceAddress;
            set => deviceAddress = value;
        }

        /// <summary>
        /// Retourne l'identifiant du fabricant.
        /// </summary>
        public override string Manufacturer
        {
            get
            {
                string manufacturer = string.Empty;
                try
                {
                    CDevicesManager.Log.Info(messagesText.getManufacturer, DeviceAddress);
                    manufacturer = GetASCII(Header.REQUESTMANUFACTURERID);
                    CDevicesManager.Log.Info(messagesText.manufacturer, DeviceAddress, manufacturer);
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                return manufacturer;
            }
        }

        /// <summary>
        /// Lecture de l'etat des otpocoupleurs du périphériques.
        /// </summary>
        public virtual byte OptoStates
        {
            get
            {
                byte result = 0XFF;
                try
                {
                    CDevicesManager.Log.Info("Lecture de l'état des optos du {0}", DeviceAddress);
                    result = GetByte(Header.READOPTOSTATES);
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Code produit du périphérique.
        /// </summary>
        public override string ProductCode
        {
            get
            {
                string productCode = string.Empty;
                try
                {
                    CDevicesManager.Log.Info(messagesText.getProductCode, DeviceAddress);
                    productCode = GetASCII(Header.REQUESTPRODUCTCODE);
                    CDevicesManager.Log.Info(messagesText.productCode, DeviceAddress, productCode);
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                return productCode;
            }
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
                    CDevicesManager.Log.Info(messagesText.getSN, DeviceAddress);
                    if (!IsCmdccTalkSended(DeviceAddress, Header.REQUESTSN, 0, null, bufferIn))
                    {
                        CDevicesManager.Log.Error(messagesText.erreurCmd, Header.REQUESTSN, DeviceAddress);
                    }
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                CDevicesManager.Log.Info("Le numéro de série du {0} est {1}", DeviceAddress, bufferIn[0] + (0x100 * bufferIn[1]) + (0x10000 * bufferIn[2]));
                return bufferIn[0] + (0x100 * bufferIn[1]) + (0x10000 * bufferIn[2]);
            }
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
                    CDevicesManager.Log.Info("Simple poll du {0}", DeviceAddress);
                    if (IsCmdccTalkSended(DeviceAddress, Header.SIMPLEPOLL, 0, null, null))
                    {
                        CDevicesManager.Log.Info("Simple poll du {0} effectué.", DeviceAddress);
                        return true;
                    }
                    CDevicesManager.Log.Error(messagesText.erreurCmd, Header.SIMPLEPOLL, DeviceAddress);
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
                return false;
            }
        }

        /// <summary>
        /// Retourne le checksum du buffer
        /// </summary>
        /// <param name="buffer">Buffer sur lequel le calcul s'effectue</param>
        /// <param name="len">Longeur du buffer</param>
        /// <returns></returns>
        private static byte CheckSum(byte[] buffer, byte len)
        {
            uint result = 0;
            try
            {
                for (byte index = 0; index < len + 4; index++)
                {
                    result += buffer[index];
                }
                result = 256 - (result % 256);
            }
            catch (Exception exception)
            {
                {
                    CDevicesManager.Log.Error("{0} {1}", exception.GetType(), exception.Message);
                }
            }
            return (byte)result;
        }

        /// <summary>
        /// Lit une chaîne ascii dans le périphérique.
        /// </summary>
        /// <param name="header">Header de la requête</param>
        /// <returns>La chaine de caractères demandée</returns>
        private string GetASCII(Header header)
        {
            byte[] bufferIn = new byte[32];
            string result = string.Empty;
            try
            {
                CDevicesManager.Log.Debug(messagesText.getText, DeviceAddress);
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
                    CDevicesManager.Log.Error(messagesText.erreurCmd, header, DeviceAddress);
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Vérifie si un bus ccTalk est branché sur un port série.
        /// </summary>
        /// <param name="nameOfPort">Nom du port série</param>
        /// <returns>true si un bus ccTalk est detecté sur ce port série</returns>
        private static bool IsCcTalkPort(string nameOfPort)
        {
            CDevicesManager.Log.Info(messagesText.verifSerialPort, nameOfPort);
            try
            {
                byte[] bufferIn = { 0x00, 0xFF, 0x55, 0xAA };
                byte[] bufferOut = { 0XFF, 0X00, 0XAA, 0X55 };

                PortSerie.PortName = nameOfPort;
                PortSerie.Open();
                PortSerie.Write(bufferOut, 0, bufferOut.Length);
                for (byte byIndex = 0; byIndex < bufferOut.Length; byIndex++)
                {
                    bufferIn[byIndex] = (byte)PortSerie.ReadByte();
                    if (bufferOut[byIndex] != bufferIn[byIndex])
                    {
                        throw new Exception(string.Format(messagesText.erreurPort, nameOfPort));
                    }
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error("{0} {1}", exception.GetType(), exception.Message);
                PortSerie.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vide le buffer passé en paramètre
        /// </summary>
        /// <param name="buffer">Buffer devant être vidé</param>
        /// <param name="len">Longueur du buffer</param>
        private static void ZeroMemory(byte[] buffer, byte len)
        {
            try
            {
                for (byte index = 0; index < len; index++)
                {
                    buffer[index] = 0;
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
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
                CDevicesManager.Log.Debug(messagesText.getByte, DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, header, 0, null, result))
                {
                    CDevicesManager.Log.Error(messagesText.erreurCmd, header, DeviceAddress);
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
            return result[0];
        }

        /// <summary>
        /// Remise à zéro des compteurs
        /// </summary>
        public static void ResetCounters()
        {
            counters = null;
            counters = new CcoinsCounters();
            counters.SaveCounters();
            countersFile.Seek(0, SeekOrigin.Begin);
            counters = (CcoinsCounters)counterSerializer.Deserialize(countersFile);
        }

        /// <summary>
        /// Initialisation des périphériques ccTalk
        /// </summary>
        public override void Init()
        {
        }

        /// <summary>
        /// Envoie une commande et recoit la réponse
        /// </summary>
        /// <param name="peripherique">Adresse du périphérique</param>
        /// <param name="commande">Header de la commande envoyée</param>
        /// <param name="lenParam">Longeur des paramètres</param>
        /// <param name="parameter">Buffer des paramètres</param>
        /// <param name="answer">Objet contenant les paramètres de retour le cas échéant</param>
        /// <returns></returns>
        public static bool  IsCmdccTalkSended(DefaultDevicesAddress peripherique, object commande, byte lenParam, byte[] parameter, object answer)
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
                    CDevicesManager.Log.Info(messagesText.sendCmd, commande.ToString(), peripherique.ToString());
                    byte[] bufferOut = new byte[sizeOfBufferOut];
                    ZeroMemory(bufferOut, sizeOfBufferOut);
                    bufferOut[0] = (byte)peripherique;
                    bufferOut[1] = lenParam;
                    bufferOut[2] = (byte)DefaultDevicesAddress.Host;
                    bufferOut[3] = Convert.ToByte(commande);
                    if (lenParam > 0)
                    {
                        Buffer.BlockCopy(parameter, 0, bufferOut, 4, lenParam);
                    }
                    bufferOut[4 + lenParam] = CheckSum(bufferOut, lenParam);

                    for (byIndex = 0; byIndex < (lenParam + 5); byIndex++)
                    {
                        strLog += string.Format("{0} ", bufferOut[byIndex]);
                    }
                    CDevicesManager.Log.Debug(strLog);
                    PortSerie.Write(bufferOut, 0, 5 + lenParam);
                    CDevicesManager.Log.Debug(messagesText.readEcho);
                    strLog = messagesText.echo;
                    for (byIndex = 0; byIndex < (lenParam + 5); byIndex++)
                    {
                        bufferIn[byIndex] = (byte)PortSerie.ReadByte();
                        strLog += string.Format("{0} ", bufferIn[byIndex]);
                    }
                    CDevicesManager.Log.Debug(strLog);
                    CDevicesManager.Log.Debug(messagesText.readAnswerDevice);
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
                        Buffer.BlockCopy(bufferIn, 4, (Array)answer, 0, bufferIn[1]);
                    }
                    result = true;
                }
                catch (TimeoutException exception)
                {
                    CDevicesManager.Log.Error(messagesText.noccTalkDevice, exception.Message, PortSerie.PortName);
                }
                catch (Exception exception)
                {
                    {
                        CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                    }
                }
                CDevicesManager.Log.Debug(strLog);
                CDevicesManager.Log.Debug("Commande {0} executée sur le {1}", commande, peripherique);
            }
            return result;
        }

        /// <summary>
        /// Reset software du périphérique.
        /// </summary>
        public bool IsDeviceReseted()
        {
            bool result = false;
            try
            {
                CDevicesManager.Log.Info("Reset du {0}", DeviceAddress);
                if (!IsCmdccTalkSended(DeviceAddress, Header.RESETDEVICE, 0, null, null))
                {
                    Thread.Sleep(200);
                    if (!IsCmdccTalkSended(DeviceAddress, Header.RESETDEVICE, 0, null, null))
                    {
                        throw new Exception(string.Format(messagesText.erreurCmd, Header.RESETDEVICE, DeviceAddress));
                    }
                }
                CDevicesManager.Log.Info("Reset du {0} effectué.", DeviceAddress);
                Thread.Sleep(200);
                result = true;
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
            return result;
        }
    }
}