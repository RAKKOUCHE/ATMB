namespace DeviceLibrary
{
    public abstract partial class CDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public class CLevel
        {
            /// <summary>
            /// Enumération des niveaux soft
            /// </summary>
            public enum SoftLevel : byte
            {
                VIDE,
                BAS,
                OK,
                HAUT,
                PLEIN,
                INCONNU,
            }

            /// <summary>
            /// Enumération des niveaux soft
            /// </summary>
            public enum HardLevel : byte
            {
                VIDE,
                OK,
                PLEIN,
                INCONNU,
            }

            public byte Number;
            public string ID;
            public SoftLevel softLevel;
            public HardLevel hardLevel;
            public bool isSoftLevelChanged;
            public bool isHardLevelChanged;
            public CLevel(SoftLevel softlevel = SoftLevel.INCONNU, HardLevel hardLevel = HardLevel.INCONNU)
            {
                softLevel = softlevel;
                this.hardLevel = hardLevel;
                isHardLevelChanged =
                isSoftLevelChanged = false;
            }
        }
    }
}