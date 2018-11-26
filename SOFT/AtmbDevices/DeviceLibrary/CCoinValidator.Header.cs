namespace DeviceLibrary
{
    public partial class CCoinValidator : CcashReader
    {
        protected new enum Header : byte
        {
            REQUESTSTATUS = 248,
            REQUESTDATABASEVER = 243,
            TESTSOLENOID = 240,
            TESTOUTPUTLINES = 238,
            READINPUTLINES = 237,
            READOPTOSTATES = 236,
            READBUFFERCREDIT = 229,
            MODIFYOVERRIDESTATUS = 222,
            REQUESTOVERRIDESTATUS = 221,
            REQUESTOPTIONFLAG = 213,
            MODIFYSORTERPATH = 210,
            MODIFYDEFAULTSORTERPATH = 189,
            REQUESTDEFAULTSORTERPATH = 188,
        }
    }
}