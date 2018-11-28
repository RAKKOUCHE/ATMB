namespace DeviceLibrary
{
    public abstract partial class CDevice
    {
        /// <summary>
        ///  Classe des pièces insérées
        /// </summary>
        public class CInserted
        {
            /// <summary>
            /// Montant nominal en centimes de la pièce insérée
            /// </summary>
            public int ValeurCent;
            /// <summary>
            /// Canal correpsondant à la pièce insérée
            /// </summary>
            public byte CVChannel;
            /// <summary>
            /// Chemin de tri utilisé 
            /// </summary>
            public byte CVPath;
            /// <summary>
            /// Montant inséré depuis le début de la transaction.
            /// </summary>
            public int TotalAmount;
            /// <summary>
            /// Sauvegarde du montant total inséré depuis le début de la transaction.
            /// </summary>
            public int BackTotalAmount;
        }
    }
}