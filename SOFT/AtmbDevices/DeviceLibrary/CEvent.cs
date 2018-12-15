/// \file CDevice.cs
/// \brief Fichier contenant la classe CDevice.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE


namespace DeviceLibrary
{
    public abstract partial class CDevice
    {
        /// <summary>
        /// Class contenant les informations sur u évenement.
        /// </summary>
        public class CEvent
        {
            /// <summary>
            /// Raison de l'évenement.
            /// </summary>
            public Reason reason;
            /// <summary>
            /// Nom du périphérique
            /// </summary>
            public string deviceId;
            /// <summary>
            /// donnée concernant l'évenenement.
            /// </summary>
            ///<remarks>Dépend du périphérique et de l'évenement.</remarks>
            public object data;
        }
    }
}