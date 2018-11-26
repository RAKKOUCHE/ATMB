using System;
using NLog;

namespace DeviceLibrary
{
    public class CHopperVariableSet
    {
        /// <summary>
        /// 
        /// </summary>
        public enum CoinMode : byte
        {
            MULTICOINSMODE = 0,
            SINGLECOINMODE = 1,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Variable : byte
        {
            CURRENTLIMIT = 0,
            MOTORSTOPDELAY = 1,
            PAYOUTTIMEOUT = 2,
            MAXCURRENT = 3,
            SINGLECOINMODE = 3,
            SUPPLYVOLTAGE = 4,
            CONNECTORADDRESS = 5,
        }



        /// <summary>
        /// 
        /// </summary>
        private CHopper Owner;

        /// <summary>
        /// Buffer du résultat de la commande de lecture des variables du hopper.
        /// </summary>
        private byte[] variableSetToRead;
        public byte[] VariableSetToRead
        {
            get => variableSetToRead;
            set => variableSetToRead = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public double CurrentLimit => Math.Round(GetVariable(Variable.CURRENTLIMIT) / 17.1, 2);

        /// <summary>
        /// 
        /// </summary>
        public byte MotorStopDelay => GetVariable(Variable.MOTORSTOPDELAY);

        /// <summary>
        /// 
        /// </summary>
        public byte PayoutDelayTO => (byte)(GetVariable(Variable.PAYOUTTIMEOUT) / 3);

        /// <summary>
        /// 
        /// </summary>
        public double Maxcurrent => GetVariable(Variable.MAXCURRENT) / 17.1;

        /// <summary>
        /// 
        /// </summary>
        public double Tension => Math.Round((0.2 + GetVariable(Variable.SUPPLYVOLTAGE) * 0.127), 2);

        /// <summary>
        /// 
        /// </summary>
        public byte ConnectorAddress => GetVariable(Variable.CONNECTORADDRESS);

        /// <summary>
        /// 
        /// </summary>
        public void GetVariableSet()
        {
            CDevicesManage.Log.Info("Lecture des variables du hopper {0}", Owner.DeviceAddress - CHopper.AddressBaseHoper);
            try
            {

                if (!Owner.IsCmdccTalkSended(Owner.DeviceAddress, CccTalk.Header.REQUESTVARIABLESET, 0, null, VariableSetToRead))
                {
                    CDevicesManage.Log.Error("Impossible de lire le -variables set- du hopper {0} ", Owner.DeviceAddress - CHopper.AddressBaseHoper);
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
        /// <bufferParam name="variable"></bufferParam>
        /// <returns></returns>
        private byte GetVariable(Variable variable)
        {
            GetVariableSet();
            CDevicesManage.Log.Debug("Variable demandée {0}", variable);
            return VariableSetToRead[(int)variable];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentLimit"></param>
        /// <param name="motorStopDelay"></param>
        /// <param name="payoutTO"></param>
        /// <param name="singleCoinMode"></param>
        public void SetVariable(double currentLimit, byte motorStopDelay, byte payoutTO, CoinMode singleCoinMode)
        {
            try
            {
                CDevicesManage.Log.Info("Enregistrement des variables du {0}", Owner.DeviceAddress);
                byte[] bufferParam = { (byte)(currentLimit * 17.1), motorStopDelay, (byte)(payoutTO * 3), (byte)singleCoinMode };
                if (!Owner.IsCmdccTalkSended(Owner.DeviceAddress, CccTalk.Header.MODIFYVARIABLESET, (byte)bufferParam.Length, bufferParam, null))
                {
                    CDevicesManage.Log.Info("Erreur durant l'écriture des variables du {0}", Owner.DeviceAddress);
                }
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Consructeur
        /// </summary>
        public CHopperVariableSet(CHopper owner)
        {
            try
            {
                Owner = owner;
                VariableSetToRead = new byte[] { 0, 0, 0, 0, 0, 0 };
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }
    }
}
