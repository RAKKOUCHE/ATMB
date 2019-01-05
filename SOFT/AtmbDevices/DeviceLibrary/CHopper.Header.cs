/// \file CHopper.Header.cs
/// \brief Fichier contenant l'énumération des commandes ccTalk pour les hoppers.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    public partial class CHopper : CccTalk
    {
        /// \addtogroup Headers
        /// @{
        /// <summary>
        /// Groupe des commandes ccTalk.
        /// </summary>
        /// <summary>
        /// Enumération des commandes spécifiques aux hoppers
        /// </summary>
        public new enum Header : byte
        {
            /// <summary>
            /// Cette commande demande l'état des sondes de niveau du hopper
            /// </summary>
            REQUESTHIGHLOWSTATUS = 217,

            /// <summary>
            /// Cette commande modifie le compteur interne du hopper.
            /// </summary>
            MODIFYPAYOUTABSOLUTECOUNT = 208,

            /// <summary>
            /// Cette commande demande le compteur interne du hopper.
            /// </summary>
            REQUESTPAYOUTABSOLUTECOUNT = 207,

            /// <summary>
            /// Cette commande modifie la capacité maximum du hopper.
            /// </summary>
            MODIFYPAYOUTCAPACITY = 187,

            /// <summary>
            /// Cette commande demande la capacité maximum du hopper.
            /// </summary>
            REQUESTPAYOUTCAPACITY = 186,

            /// <summary>
            /// Cette commande modifie le niveau des pièces devant rester dans le hopper.
            /// </summary>
            MODIFYPAYOUTFLOAT = 175,

            /// <summary>
            /// Cette commande demande le niveau des pièces devant rester dans le hopper.
            /// </summary>
            REQUESTPAYOUTFLOAT = 174,

            /// <summary>
            /// Cette commande arrête immédiatement la distribution et renvoie le nombre de pièce restant à payer.
            /// </summary>
            EMERGENCYSTOP = 172,

            /// <summary>
            /// Cette commande demande le nom des pièces que le hopper gère.
            /// </summary>
            REQUESTHOPPERCOIN = 171,

            /// <summary>
            /// Cette commande demande le nombre de pièces distribuées par le hopper
            /// </summary>
            REQUESTHOPPERDISPENSECOUNT = 168,

            /// <summary>
            /// Cette commande lance la distribution des pièces.
            /// </summary>
            /// <remarks>La distribution ne peut s'effectuer que par la séquence décrite dans la documentation du hopper.</remarks>
            DISPENSEHOPPERCOINS = 167,

            /// <summary>
            /// Cette commande demande les informations sur les pièces distribuées dans l'opération en cours ou venant de se terminer.
            /// </summary>
            REQUESTHOPPERSTATUS = 166,

            /// <summary>
            /// Cette commande enregistre les données variables dans le hopper
            /// </summary>
            MODIFYVARIABLESET = 165,

            /// <summary>
            /// Active le hopper.
            /// </summary>
            /// <remarks>Le hopper ne peut être activé que par le code 165</remarks>
            ENABLEHOPPER = 164,

            /// <summary>
            /// Cette commande demande les informations sur l'état du hopper.
            /// </summary>
            TESTHOPPER = 163,

            /// <summary>
            /// Cette commande prépare en interne le chiffrage du hopper.
            /// </summary>
            PUMPRNG = 161,

            /// <summary>
            /// Cette commande demande la clé de chiffrage.
            /// </summary>
            REQUESTCIPHERKEY = 160,

            /// <summary>
            /// Cette commande demande une distribution par valeur.
            /// </summary>
            DISPENSEHOPPERVALUE = 134,

            /// <summary>
            /// Cette commande demande les informations sur les montants distribués dans l'opération en cours ou venant de se terminer.
            /// </summary>
            REQUESTHOPPERPOLLINGVALUE = 133,

            /// <summary>
            /// Cette commande arrête immédiatement la distribution et renvoie le montant restant à payer.
            /// </summary>
            EMERGENCYSTOPVALUE = 132,

            /// <summary>
            /// Cette commande demande le nom de la pièce et la valeur.
            /// </summary>
            /// <remarks>Commande fonctionnant sur les hoppers multiple.</remarks>
            REQUESTHOPPERCOINVALUE = 131,

            /// <summary>
            /// Cette commande demande le nombre de pièce d'un type distribué
            /// </summary>
            /// <remarks>Cette commande est reservée aux hopperx multiple</remarks>
            REQUESTINDEXEDDHOPPERDISPENSECOUNT = 130,

            /// <summary>
            /// Cette commande demande l'état du chiffrage du hopper
            /// </summary>
            /// <remarks>Non documenté pour des raisons de sécurité.</remarks>
            REQUESTENCRYPTEDHOPPERSTATUS = 109,
        }
    }
}