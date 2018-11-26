namespace DeviceLibrary
{

    public partial class CCoinValidator : CcashReader
    {
        /// <summary>
        /// Etat de la machine d'état du CV.
        /// </summary>
        public enum Etat : byte
        {
            STATE_INIT,
            STATE_RESET,
            STATE_SIMPLEPOLL,
            STATE_GETPOLLINGPRIORITY,
            STATE_SETPOLLINGDELAY,
            STATE_GETSTATUS,
            STATE_GETMANUFACTURERID,
            STATE_GETEQUIPEMENTCATEGORY,
            STATE_GETPRODUCTCODE,
            STATE_GETBUILDCODE,
            STATE_GETDATABASEVERSION,
            STATE_GETSERIALNUMBER,
            STATE_GETSOFTWAREREVISION,
            STATE_TESTSOLENOID,
            STATE_TRASHEMPTY,
            STATE_SETSPEEDMOTOR,
            STATE_GETSPEEDMOTOR,
            STATE_GETPOCKET,
            STATE_CHECKTRASHDOOR,
            STATE_CHECKLOWERSENSOR,
            STATE_SELF_TEST,
            STATE_SETINHIBITSTATUS,
            STATE_GETINHIBITSTATUS,
            STATE_GETCREDITBUFFER,
            STATE_SETMASTERINHIBIT,
            STATE_DISABLEMASTER,
            STATE_ENABLEMASTER,
            STATE_GETMASTERINHIBT,
            STATE_SETOVERRIDE,
            STATE_GETOVERRIDE,
            STATE_GETOPTION,
            STATE_SETSORTERPATH,
            STATE_GETSORTERPATH,
            STATE_SETDEFAULTSORTERPATH,
            STATE_GETDEFAULTSORTERPATH,
            STATE_GETCOINID,
            STATE_ACCEPTLIMIT,
            STATE_COMMSREVISION,
            STATE_CHECKCREDIBUFFER,
            STATE_IDLE = 0XFF,
        }
    }
}