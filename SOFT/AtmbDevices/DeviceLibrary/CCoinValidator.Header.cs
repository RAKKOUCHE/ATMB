namespace DeviceLibrary
{
    public partial class CCoinValidator : CcashReader
    {
        /// <summary>
        /// Liste des headers spécifiques au monnayeur
        /// </summary>
        protected new enum Header : byte
        {
            /// <summary>
            /// 
            /// </summary>
            REQUESTSTATUS = 248,
            /// <summary>
            /// 
            /// </summary>
            REQUESTDATABASEVER = 243,
            /// <summary>
            /// 
            /// </summary>
            TESTSOLENOID = 240,
            /// <summary>
            /// 
            /// </summary>
            TESTOUTPUTLINES = 238,
            /// <summary>
            /// 
            /// </summary>
            READINPUTLINES = 237,
            /// <summary>
            /// 
            /// </summary>
            READOPTOSTATES = 236,
            /// <summary>
            /// 
            /// </summary>
            READBUFFERCREDIT = 229,
            /// <summary>
            /// 
            /// </summary>
            MODIFYOVERRIDESTATUS = 222,
            /// <summary>
            /// 
            /// </summary>
            REQUESTOVERRIDESTATUS = 221,
            /// <summary>
            /// 
            /// </summary>
            REQUESTOPTIONFLAG = 213,
            /// <summary>
            /// 
            /// </summary>
            MODIFYSORTERPATH = 210,
            /// <summary>
            /// 
            /// </summary>
            MODIFYDEFAULTSORTERPATH = 189,
            /// <summary>
            /// 
            /// </summary>
            REQUESTDEFAULTSORTERPATH = 188,
            /// <summary>
            /// 
            /// </summary>
            SETACCEPTLIMIT = 135,
        }
    }
}