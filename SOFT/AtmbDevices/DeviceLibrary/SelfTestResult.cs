namespace DeviceLibrary
{
    public partial class CccTalk : CDevice
    {
        /// <summary>
        /// Liste des fautes possibles retournées par un self test.
        /// </summary>
        public enum SelfTestResult
        {
            OK = 0,
            ERRORCHECKSUM = 1,
            ERRORCOILMEASUREMENT = 2,
            ERRORCREDITSENSOR = 3,
            ERRORPIEZOSENSOR =4,
            ERRORREFLECTIVESENSOR = 5,
            ERRORDIAMETERSENSOR = 6,
            ERRORONWAKEUPSENSOR = 7,
            ERROREXITSENSOR = 8,
            NVRAMCHECKSUM = 9,
            ERRORKEYPAD = 14,
            ERRORBUTTON = 15,
            ERRORDISPLAY = 16,
            COINAUDITERROR = 17,
            ERRORONREJECTSENSOR = 18,
            ERRORONCOINRETURNMECH = 19,
            ERRORCOSMECH = 20,
            ERRORRIM = 21,
            ERRORTHERMISTOR = 22,
            ERRORMOTOR = 23,
            ERRORPAYOUTSENSOR = 26,
            ERRORLEVELSENSOR = 27,
            ERRORDATABLOCKCHECSUM = 30,
            ERRORINTERNALCOM = 32,
            ERRORPOWERSUPPLY = 33,
            ERRORTEMP = 34,
            ERRORDCE = 35,
            ERRORBVSENSOR = 36,
            ERRORBVTRANSPORT = 37,
            ERRORSTACKER = 38,
            BILLJAMMED = 39,
            ERRORTESTRAM = 40,
            ERRORSTRINGSENSOR = 41,
            ERRORACCEPTGATEOPEN = 42,
            ERRORACCEPTGATECLOSE = 43,
            STACKERMISSING = 44,
            STACKERFULL = 45,
            ERRORERASEFLASHMEM = 46,
            ERRORWRITEFLASHMEM = 47,
            ERRORSLAVEDEVICE = 48,
            ERROROPTOSENSOR = 49,
            ERRORBATTERY = 50,
            ERRORDOOROPEN = 51,
            ERRORMICROSWITCH = 52,
            ERRORRTC = 53,
            ERRORFW = 54,
            ERRORINIT = 55,
            ERRORCURRENTSUPPLY = 56,
            FORCEBOOTLOADERMODE = 57,
            COINJAM = 253,
            DISKBLOCKED = 254,
            ERRORUNKNOW = 255,
        }
    }
}