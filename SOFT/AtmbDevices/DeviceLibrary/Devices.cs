namespace DeviceLibrary
{
    /// <summary>
    /// Enumération des périphériques et leurs adresses.
    /// </summary>
    public enum DefaultDevicesAddress : byte
    {
        /// <summary>
        /// Adresse pour un message sur tous les périphériques
        /// </summary>
        Broadcast = 0,
        /// <summary>
        /// Adresse master
        /// </summary>
        Host = 1,
        /// <summary>
        /// Adresse par defaut du validateur
        /// </summary>
        CoinAcceptor = 2,
        /// <summary>
        /// Adresse par defaut du hopper 1
        /// </summary>
        Hopper1 = 3,
        /// <summary>
        /// Adresse par defaut du hopper 2
        /// </summary>
        Hopper2 = 4,
        /// <summary>
        /// Adresse par defaut du hopper 3
        /// </summary>
        Hopper3 = 5,
        /// <summary>
        /// Adresse par defaut du hopper 4
        /// </summary>
        Hopper4 = 6,
        /// <summary>
        /// Adresse par defaut du hopper 5
        /// </summary>
        Hopper5 = 7,
        /// <summary>
        /// Adresse par defaut du hopper 6
        /// </summary>
        Hopper6 = 8,
        /// <summary>
        /// Adresse par defaut du hopper 7
        /// </summary>
        Hopper7 = 9,
        /// <summary>
        /// Adresse par defaut du hopper 8
        /// </summary>
        Hopper8 = 10,
        /// <summary>
        /// Adresse pour un BNA
        /// </summary>
        BNA = 128,
        /// <summary>
        /// Adresse pour un BNR
        /// </summary>
        BNR = 129,
    }
}
