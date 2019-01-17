/// \file CHopper.cs
/// \brief Fichier contenant la classe CHopper.
/// \date 28 11 2018
/// \version 1.1
/// Modification du traitements des erreurs du hopper.
/// Création de sous fichiers contenant les classes.
/// \author Rachid AKKOUCHE

using System;

namespace DeviceLibrary
{
    public partial class CHopper
    {
        /// <summary>
        /// Classe des variables du hopper
        /// </summary>
        public class CHopperVariableSet
        {
            /// <summary>
            /// Hopper propriétaire de l'instance.
            /// </summary>
            private readonly CHopper Owner;

            private byte[] variableSetToRead;

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
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }

            /// <summary>
            /// Enumération des modes de distribution.
            /// </summary>
            public enum CoinMode : byte
            {
                /// <summary>
                /// Plusieurs pièces peuvent être distribuées en 1 opération.
                /// </summary>
                MULTICOINSMODE = 0,

                /// <summary>
                /// Une seule pièce peut être distribuée par opération.
                /// </summary>
                SINGLECOINMODE = 1,
            }

            /// <summary>
            /// Enumération des index des variables
            /// </summary>
            public enum Variable : byte
            {
                /// <summary>
                /// Limit de courant avant d'inverser la rotation du moteur
                /// </summary>
                CURRENTLIMIT = 0,

                /// <summary>
                /// Délai pour arrêter le moteur après distribution.
                /// </summary>
                MOTORSTOPDELAY = 1,

                /// <summary>
                /// Temps sans distribution pour arrêter le moteur si une pièce doit être distribuée.
                /// </summary>
                PAYOUTTIMEOUT = 2,

                /// <summary>
                /// Maximum de consomation de courant accepté.
                /// </summary>
                MAXCURRENT = 3,

                /// <summary>
                /// Mode pièce par pièce.
                /// </summary>
                SINGLECOINMODE = 3,

                /// <summary>
                /// Tension actuelle d'alimentation.
                /// </summary>
                SUPPLYVOLTAGE = 4,

                /// <summary>
                /// Adresse physique du hopper
                /// </summary>
                CONNECTORADDRESS = 5,
            }

            /// <summary>
            /// Lecture de l'adresse physique du hopper
            /// </summary>
            public byte ConnectorAddress => GetVariable(Variable.CONNECTORADDRESS);

            /// <summary>
            /// Lecture et conversion de la limite de courant
            /// </summary>
            public double CurrentLimit => Math.Round(GetVariable(Variable.CURRENTLIMIT) / 17.1, 2);

            /// <summary>
            /// Lecture et conversion du courant maximum
            /// </summary>
            public double Maxcurrent => GetVariable(Variable.MAXCURRENT) / 17.1;

            /// <summary>
            /// Lecture du delai pour arrêter le moteur.
            /// </summary>
            public byte MotorStopDelay => GetVariable(Variable.MOTORSTOPDELAY);

            /// <summary>
            /// Lecture du delay pour la distribution d'un pièce.
            /// </summary>
            public byte PayoutDelayTO => (byte)(GetVariable(Variable.PAYOUTTIMEOUT) / 3);

            /// <summary>
            /// Lecture et conversion de la tension maximum.
            /// </summary>
            public double Tension => Math.Round((0.2 + GetVariable(Variable.SUPPLYVOLTAGE) * 0.127), 2);

            /// <summary>
            /// Buffer du résultat de la commande de lecture des variables du hopper.
            /// </summary>
            public byte[] VariableSetToRead
            {
                get => variableSetToRead;
                set => variableSetToRead = value;
            }

            /// <summary>
            /// Lecture d'une variable
            /// </summary>
            /// <param name="variable">variable demandée</param>
            /// <returns>Valeur de la variable demandée</returns>
            private byte GetVariable(Variable variable)
            {
                GetVariableSet();
                CDevicesManager.Log.Debug("Variable demandée {0}", variable);
                return VariableSetToRead[(int)variable];
            }

            /// <summary>
            /// Lecture des variables.
            /// </summary>
            public void GetVariableSet()
            {
                CDevicesManager.Log.Info("Lecture des variables du hopper {0}", Owner.DeviceAddress - CHopper.AddressBaseHoper);
                try
                {
                    if (!Owner.IsCmdccTalkSended(Owner.DeviceAddress, CccTalk.Header.REQUESTVARIABLESET, 0, null, VariableSetToRead))
                    {
                        CDevicesManager.Log.Error("Impossible de lire le -variables set- du hopper {0} ", Owner.DeviceAddress - CHopper.AddressBaseHoper);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }

            /// <summary>
            /// Enregistrement des variables
            /// </summary>
            /// <param name="currentLimit">Limit de courant avant l'inversion de la rotaiton</param>
            /// <param name="motorStopDelay">Delai pour arrêter le moteur après la distribution.</param>
            /// <param name="payoutTO">Le délai maximum pour la distribution d'une pièce.</param>
            /// <param name="singleCoinMode">Single coin ou non.</param>
            public void SetVariable(double currentLimit, byte motorStopDelay, byte payoutTO, CoinMode singleCoinMode)
            {
                try
                {
                    CDevicesManager.Log.Info("Enregistrement des variables du {0}", Owner.DeviceAddress);
                    byte[] bufferParam = { (byte)(currentLimit * 17.1), motorStopDelay, (byte)(payoutTO * 3), (byte)singleCoinMode };
                    if (!Owner.IsCmdccTalkSended(Owner.DeviceAddress, CccTalk.Header.MODIFYVARIABLESET, (byte)bufferParam.Length, bufferParam, null))
                    {
                        CDevicesManager.Log.Info("Erreur durant l'écriture des variables du {0}", Owner.DeviceAddress);
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }
        }
    }
}