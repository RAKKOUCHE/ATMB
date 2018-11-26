namespace DeviceLibrary
{
    public partial class CPelicano: CCoinValidator
    {
        private new enum Header : byte
        {
            READOPTOSTATES = 236,
            REQUESTOPTIONFLAG = 213,
            SETGETACCEPTLIMIT = 135,
        }
    }
}
