namespace DeviceLibrary
{
    /// <summary>
    /// Header core ccTalk
    /// </summary>
    public partial class CccTalk : CDevice
    {
        /// <summary>
        /// Enumération des headers communs à tous les périphériques ccTalk.
        /// </summary>
        public enum Header : byte
        {
            /// <summary>
            /// Commande d'ajustement ou de test interne.
            /// </summary>
            FACTORYSETUP = 255,
            /// <summary>
            /// Commande vérifiant la communication entre le host et le périphérique.
            /// </summary>
            SIMPLEPOLL = 254,
            /// <summary>
            /// Commande broadcast permettant de déterminer l'adresse utilisée par chaque périphérique
            /// </summary>
            ADDRESSPOLL = 253,
            /// <summary>
            /// Commande permettant de vérifier si plusieurs périphériques partagent la même adresse.
            /// </summary>
            ADDRESSCLASH = 252,
            /// <summary>
            /// Commande effectuant le chamgement d'adresse.
            /// </summary>
            ADDRESSCHANGE = 251,
            /// <summary>
            /// Détermine un adresse alátoire pour le périphérique.
            /// </summary>
            ADDRESSRANDOM = 250,
            /// <summary>
            /// Cette commande demande les variables du périphérique.
            /// </summary>
            REQUESTVARIABLESET = 247,
            /// <summary>
            /// Cette commande demande l'identification du fabricant du périphérique.
            /// </summary>
            REQUESTMANUFACTURERID = 246,
            /// <summary>
            /// Cette commande demande la catégorie à laquelle appartient le périphérique.
            /// </summary>
            REQUESTEQUIPEMENTCATEGORYID = 245,
            /// <summary>
            /// Cette commande demande le code du produit du péripherique.
            /// </summary>
            REQUESTPRODUCTCODE = 244,
            /// <summary>
            /// Cette commande demande le numéro de série du périphérique.
            /// </summary>
            REQUESTSN = 242,
            /// <summary>
            /// Cette commande demande la révision du software du périphérique.
            /// </summary>
            REQUESTSWREV = 241,
            /// <summary>
            /// Cette commande active les moteurs du périphérique.
            /// </summary>
            OPERATEMOTOR = 239,
            /// <summary>
            /// Cette commande demande l'etat des optocoupleurs du périphérique.
            /// </summary>
            READOPTOSTATES = 236,
            /// <summary>
            /// Cette commande change le code pin du périphérique.
            /// </summary>
            ENTERNEWPINNUMBER = 219,
            /// <summary>
            /// Cette commande transmet le code PIN au périphérique.
            /// </summary>
            ENTERPINNUMBER = 218,
            /// <summary>
            /// Cette commande demande les informations sur le stockage des données dans le périphérique.
            /// </summary>
            REQUESTDATASTORAGEAVAILABILITY = 216,
            /// <summary>
            /// Cette commande lit un bloc de donnée dans le périphérique.
            /// </summary>
            READDATABLOCK = 215,
            /// <summary>
            /// Cette commande lit un bloc de donnée dans le périphérique.
            /// </summary>
            WRITEDATABLOCK = 214,
            /// <summary>
            /// Cette commande demande le code de production au périphérique.
            /// </summary>
            REQUESTBUILDCODE = 192,
            /// <summary>
            /// Cette commande demande l'année de référence au périphérique.
            /// </summary>
            REQUESTBASEYEAR = 170,
            /// <summary>
            /// Cette commande demande le mode utilisé pour l'adresse du périphérique.
            /// </summary>
            REQUESTADDRESSMODE = 169,
            /// <summary>
            /// Cette commande enregistre les variables dans le périphérique.
            /// </summary>
            MODIFYVARIABLESET = 165,
            /// <summary>
            /// Cette commande change le code de chiffrage du périphérique.
            /// </summary>
            SWITCHENCRYPTIONCODE = 137,
            /// <summary>
            /// Cette commande enregistre le code de chiffrage du périphérique.
            /// </summary>
            STOREENCRYPTIONCODE=136,
            /// <summary>
            /// Réponse renvoyé par le périphérique s'il ne peut traiter la commande parce-qu'il est occupé.
            /// </summary>
            BUSY = 6,
            /// <summary>
            /// Réponse renvoyé par le périphérique s'il ne comprend pas la commande.
            /// </summary>
            NAK = 5,
            /// <summary>
            /// Cette commande demande le niveau ccTalk du périphérique.
            /// </summary>
            REQUESTCOMMSREVISION = 4,
            /// <summary>
            /// R.A.Z. des incidents de communications.
            /// </summary>
            CLEARCOMMSSTATUSVARIABLES = 3,
            /// <summary>
            /// Cette commande demande les informations sur les incidents de communication du périphérique.
            /// </summary>
            REQUESTCOMMSSTATUSVARIABLES = 2,
            /// <summary>
            /// Cette commande effectue un reset sur le périphérique.
            /// </summary>
            RESETDEVICE = 1,
        }
    }
}
