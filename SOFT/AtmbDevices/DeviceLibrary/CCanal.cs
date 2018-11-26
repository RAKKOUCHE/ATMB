namespace DeviceLibrary
{
    /// <summary>
    /// Class d'un canal
    /// </summary>
    public partial class CCanal
    {
        /// <summary>
        /// 
        /// </summary>
        private CCoinValidator CVOwner;

        /// <summary>
        /// 
        /// </summary>
        public CCoindID coinId;

        /// <summary>
        /// 
        /// </summary>
        public CSorter sorter;

        private byte hopperToLoad;
        /// <summary>
        /// 
        /// </summary>
        public byte HopperToLoad
        {
            get => hopperToLoad;
            set => hopperToLoad = value;
        }

        /// <summary>
        /// 
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
