namespace DeviceLibrary
{
    public abstract partial class CDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public class CInserted
        {
            public string IdDevice;
            public int ValeurCent;
            public byte CVChannel;
            public byte CVPath;
            public int TotalAmount;
            public int BackTotalAmount;
        }
    }
}