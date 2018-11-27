namespace DeviceLibrary
{

    public partial class CCoinValidator : CcashReader
    {
        /// <summary>
        /// Etat de la machine d'état du CV.
        /// </summary>
        public enum Etat : byte
        {
            /// <summary>
            /// Etat pour l'initialisation du monnayeur.
            /// </summary>
            STATE_INIT,
            /// <summary>
            /// Etat pour le reset du monnayeur.
            /// </summary>
            STATE_RESET,
            /// <summary>
            ///
            /// </summary>
            STATE_SIMPLEPOLL,
            /// <summary>
            ///
            /// </summary>
            STATE_GETPOLLINGPRIORITY,
            /// <summary>
            ///
            /// </summary>
            STATE_SETPOLLINGDELAY,
            /// <summary>
            ///
            /// </summary>
            STATE_GETSTATUS,
            /// <summary>
            ///
            /// </summary>
            STATE_GETMANUFACTURERID,
            /// <summary>
            ///
            /// </summary>
            STATE_GETEQUIPEMENTCATEGORY,
            /// <summary>
            ///
            /// </summary>
            STATE_GETPRODUCTCODE,
            /// <summary>
            ///
            /// </summary>
            STATE_GETBUILDCODE,
            /// <summary>
            ///
            /// </summary>
            STATE_GETDATABASEVERSION,
            /// <summary>
            ///
            /// </summary>
            /// <summary>
            ///
            /// </summary>
            STATE_GETSERIALNUMBER,
            /// <summary>
            ///
            /// </summary>
            STATE_GETSOFTWAREREVISION,
            /// <summary>
            ///
            /// </summary>
            STATE_TESTSOLENOID,
            /// <summary>
            ///
            /// </summary>
            STATE_TRASHEMPTY,
            /// <summary>
            ///
            /// </summary>
            STATE_SETSPEEDMOTOR,
            /// <summary>
            ///
            /// </summary>
            STATE_GETSPEEDMOTOR,
            /// <summary>
            ///
            /// </summary>
            STATE_GETPOCKET,
            /// <summary>
            ///
            /// </summary>
            STATE_CHECKTRASHDOOR,
            /// <summary>
            ///
            /// </summary>
            STATE_CHECKLOWERSENSOR,
            /// <summary>
            ///
            /// </summary>
            STATE_SELF_TEST,
            /// <summary>
            ///
            /// </summary>
            STATE_SETINHIBITSTATUS,
            /// <summary>
            ///
            /// </summary>
            STATE_GETINHIBITSTATUS,
            /// <summary>
            ///
            /// </summary>
            /// <summary>
            ///
            /// </summary>
            STATE_GETCREDITBUFFER,
            /// <summary>
            ///
            /// </summary>
            STATE_SETMASTERINHIBIT,
            /// <summary>
            ///
            /// </summary>
            /// <summary>
            ///
            /// </summary>
            STATE_DISABLEMASTER,
            /// <summary>
            ///
            /// </summary>
            STATE_ENABLEMASTER,
            /// <summary>
            ///
            /// </summary>
            STATE_GETMASTERINHIBT,
            /// <summary>
            ///
            /// </summary>
            STATE_SETOVERRIDE,
            /// <summary>
            ///
            /// </summary>
            /// <summary>
            ///
            /// </summary>
            STATE_GETOVERRIDE,
            /// <summary>
            ///
            /// </summary>
            STATE_GETOPTION,
            /// <summary>
            ///
            /// </summary>
            STATE_SETSORTERPATH,
            /// <summary>
            ///
            /// </summary>
            STATE_GETSORTERPATH,
            /// <summary>
            ///
            /// </summary>
            /// <summary>
            ///
            /// </summary>
            STATE_SETDEFAULTSORTERPATH,
            /// <summary>
            ///
            /// </summary>
            STATE_GETDEFAULTSORTERPATH,
            /// <summary>
            ///
            /// </summary>
            STATE_GETCOINID,
            /// <summary>
            ///
            /// </summary>
            STATE_ACCEPTLIMIT,
            /// <summary>
            ///
            /// </summary>
            /// <summary>
            ///
            /// </summary>
            STATE_COMMSREVISION,
            /// <summary>
            ///
            /// </summary>
            STATE_CHECKCREDIBUFFER,
            /// <summary>
            ///
            /// </summary>
            STATE_IDLE = 0XFF,
        }
    }
}