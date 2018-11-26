namespace DeviceLibrary
{
    public partial class CPelicano : CCoinValidator
    {
        private enum CcmdMotors : byte
        {
            TRASH1 = 1,
            TRASH2 = 2,
            TRASH3 = 3,
            CPR = 4,
            SETSPEED = 10,
            GETSPEED = 11,
            GETPOCKETIME = 12,
        }
    }
}
