namespace DeviceLibrary
{
    public partial class CccTalk : CDevice
    {
        /// <summary>
        /// Enumération des headers core
        /// </summary>
        public enum Header : byte
        {
            FACTORYSETUP = 255,
            SIMPLEPOLL = 254,
            ADDRESSPOLL = 253,
            ADDRESSCLASH = 252,
            ADDRESSCHANGE = 251,
            ADDRESSRANDOM = 250,
            REQUESTVARIABLESET = 247,
            REQUESTMANUFACTURERID = 246,
            REQUESTEQUIPEMENTCATEGORYID = 245,
            REQUESTPRODUCTCODE = 244,
            REQUESTSN = 242,
            REQUESTSWREV = 241,
            OPERATEMOTOR = 239,
            READOPTOSTATES = 236,
            ENTERNEWPINNUMBER = 219,
            ENTERPINNUMBER = 218,
            REQUESTDATASTORAGEAVAILABILITY = 216,
            READDATABLOCK = 215,
            WRITEDATABLOCK = 214,
            REQUESTBUILDCODE = 192,
            REQUESTBASEYEAR = 170,
            REQUESTADDRESSMODE = 169,
            MODIFYVARIABLESET = 165,
            SWITCHENCRYPTIONCODE = 137,
            STOREENCRYPTIONCODE=136,
            SETACCEPTLIMIT = 135,
            BUSY = 6,
            NAK = 5,
            REQUESTCOMMSREVISION = 4,
            CLEARCOMMSSTATUSVARIABLES = 3,
            REQUESTCOMMSSTATUSVARIABLES = 2,
            RESETDEVICE = 1,
        }
    }
}
