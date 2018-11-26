namespace DeviceLibrary
{

    public abstract partial class CcashReader : CccTalk
    {
        /// <summary>
        /// Enumeration des headers concern
        /// </summary>
        protected new enum Header : byte
        {
            REQUESTPOLLINGPRIORITY = 249,
            PERFORMSELFTEST = 232,
            MODIFYINHIBITSTATUS = 231,
            REQUESTINHIBITSTATUS = 230,
            MODIFYMASTERINHIBITSTATUS = 228,
            REQUESTMASTERINHIBITSTATUS = 227,
        }
    }
}