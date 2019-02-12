/// \file CHopper.Status.cs
/// \brief Fichier contenant la classe CHopperStatus
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE


namespace DeviceLibrary
{
            /// <summary>
            /// Classe des resultats d'une distribution.
            /// </summary>
            public class CHopperDispensedResult
            {
                /// <summary>
                /// Montant
                /// </summary>
                public int AmountToDispense;

                /// <summary>
                /// Nombre de pièces distribuées.
                /// </summary>
                public byte CoinsPaid;

                /// <summary>
                /// Nombre de pièce restant à payer.
                /// </summary>
                ///<remarks>Passe à zéro à la fin de la distribtion quelque soit la raison.</remarks>
                public byte CoinsRemaining;

                /// <summary>
                /// Nombre de pièces non distribuées
                /// </summary>
                public byte CoinsUnpaid;

                /// <summary>
                /// Nombre de pièces devant être distribuées
                /// </summary>
                public byte CoinToDispense;

                /// <summary>
                /// Montant distribué
                /// </summary>
                public int MontantPaid;

                /// <summary>
                /// Montant non distribué
                /// </summary>
                public int MontantUnpaid;

                /// <summary>
                /// Numéro du hopper.
                /// </summary>
                public string nameOfHopper;

                /// <summary>
                /// Constructeur
                /// </summary>
                /// <param name="hopperNumber"></param>
            }
}