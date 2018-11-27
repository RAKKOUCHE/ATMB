namespace DeviceLibrary
{
    /// <summary>
    /// Class 
    /// </summary>
    public partial class CCanal
    {
        /// <summary>
        /// Class contenant les informations sur un canal
        /// </summary>
        private CCoinValidator CVOwner;

        /// <summary>
        /// Identification de la pièce reconnue dans le canal
        /// </summary>
        public CCoindID coinId;

        /// <summary>
        /// Instance des chemins utilisés pour trier la pièce reconnue dans le canal
        /// </summary>
        public CSorter sorter;
        
        private byte hopperToLoad;
        /// <summary>
        /// Hopper vers lequel sera dirigé la pièce reconnue
        /// </summary>
        public byte HopperToLoad
        {
            get => hopperToLoad;
            set => hopperToLoad = value;
        }

        /// <summary>
        /// Numéro du canal
        /// </summary>
        public byte Number;

        /// <summary>
        /// Nombre de pièces reconnues par le canal.
        /// </summary>
        public long CoinIn
        {
            get => CccTalk.counters.coinsInAccepted[Number - 1];
            set => CccTalk.counters.coinsInAccepted[Number - 1] = value;
        }

        /// <summary>
        /// Montant introduit dans le canal
        /// </summary>
        public long MontantIn
        {
            get => CccTalk.counters.amountCoinInAccepted[Number - 1];
            set => CccTalk.counters.amountCoinInAccepted[Number - 1] = value;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="number">Numéro du canal</param>
        /// <param name="owner">Nécessaire pour effectuer les commandes</param>
        public CCanal(byte number, CCoinValidator owner)
        {
            Number = number;
            CVOwner = owner;
            coinId = new CCoindID(this);
            sorter = new CSorter(this);
        }
    }
}
