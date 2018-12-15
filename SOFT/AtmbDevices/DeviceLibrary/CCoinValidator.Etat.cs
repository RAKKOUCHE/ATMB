/// \file CCoinValidator.Etat.cs
/// \brief Fichier contenant l'énumération des états de la machine d'état du monnayeurs.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;                        
namespace DeviceLibrary
{
    public partial class CCoinValidator : CcashReader
    {
        /// \addtogroup Etats
        /// @{
        /// <summary>
        /// Groupe des états des machines d'états des périphériques.
        /// </summary>
        /// <summary>
        /// Etat de la machine d'état du CV.
        /// </summary>
        public enum Etat : byte
        {
            /// <summary>
            /// Etat pour l'initialisation du monnayeur.
            /// </summary>
            STATE_INIT,
            /// <summary>
            /// Etat pour le reset du monnayeur.
            /// </summary>
            STATE_RESET,
            /// <summary>
            /// Etat pour un polling de vérification.
            /// </summary>
            STATE_SIMPLEPOLL,
            /// <summary>
            /// Etat pour obtenir le delai maximum entre 2 lectures du buffer des crédits et des codes erreurs.
            /// </summary>
            STATE_GETPOLLINGPRIORITY,
            /// <summary>
            /// Etat pour fixer le délai de lectures entre 2 lectures du buffer des crédits et des codes erreurs.
            /// </summary>
            STATE_SETPOLLINGDELAY,
            /// <summary>
            /// Etat pour la lecture du status du périphérique.
            /// </summary>
            STATE_GETSTATUS,
            /// <summary>
            /// Etat pour la lecture de l'identification du fabricant du périphérique.
            /// </summary>
            STATE_GETMANUFACTURERID,
            /// <summary>
            /// Etat pour la lecture de la catégorie du périphérique.
            /// </summary>
            STATE_GETEQUIPEMENTCATEGORY,
            /// <summary>
            /// Etat pour la lecture du code produit du périphérique.
            /// </summary>
            STATE_GETPRODUCTCODE,
            /// <summary>
            /// Etat pour la lecture du code de production du périphérique.
            /// </summary>
            STATE_GETBUILDCODE,
            /// <summary>
            /// Etat pour la lecture de version des données du périphérique.
            /// </summary>
            STATE_GETDATABASEVERSION,
            /// <summary>
            /// Etat pour la lecture du numéro de série du périphérique.
            /// </summary>
            STATE_GETSERIALNUMBER,
            /// <summary>
            /// Etat pour la lecture de la révision software du périphérique.
            /// </summary>
            STATE_GETSOFTWAREREVISION,
            /// <summary>
            /// Etat pour le test des activateurs du périphérique.
            /// </summary>
            STATE_TESTSOLENOID,
            /// <summary>
            /// Etat pour vider le container du Pelicano.
            /// </summary>
            STATE_TRASHEMPTY,
            /// <summary>
            /// Etat pour définir la vitesse de rotation du disque du pelicano.
            /// </summary>
            STATE_SETSPEEDMOTOR,
            /// <summary>
            /// Etat pour la lecture de la vitesse de rotation du disque du Pelciano.
            /// </summary>
            STATE_GETSPEEDMOTOR,
            /// <summary>
            /// Etat pour la lecture du temps entre 2 trous du disque du Pelciano.
            /// </summary>
            STATE_GETPOCKET,
            /// <summary>
            /// Etat pour lire l'état de la trappe du container du Pelicano.
            /// </summary>
            STATE_CHECKTRASHDOOR,
            /// <summary>
            /// Etat pour vérifier l'état des senseurs de sortie du Pelicano.
            /// </summary>
            STATE_CHECKLOWERSENSOR,
            /// <summary>
            /// Etat pour effectuer un test interne du périphérique.
            /// </summary>
            STATE_SELF_TEST,
            /// <summary>
            /// Etat pour fixer l'inhibition des canaux du périphérique.
            /// </summary>
            STATE_SETINHIBITSTATUS,
            /// <summary>
            /// Etat pour la lecture de l'inhibiton des canaux périphérique.
            /// </summary>
            STATE_GETINHIBITSTATUS,
            /// <summary>
            /// Etat pour la lecture du buffer de credits ou des codes d'erreur du périphérique.
            /// </summary>
            STATE_GETCREDITBUFFER,
            /// <summary>
            /// Etat pour activer ou désactiver le périphérique.
            /// </summary>
            STATE_SETMASTERINHIBIT,
            /// <summary>
            /// Etat pour désactiver le périphérique.
            /// </summary>
            STATE_DISABLEMASTER,
            /// <summary>
            /// Etat pour activer le périphérique.
            /// </summary>
            STATE_ENABLEMASTER,
            /// <summary>
            /// Etat pour la lecture de l'inhibition générale du périphérique.
            /// </summary>
            STATE_GETMASTERINHIBT,
            /// <summary>
            /// Etat pour enregistre l'octet d'override du périphérique.
            /// </summary>
            STATE_SETOVERRIDE,
            /// <summary>
            /// Etat pour la lecture de l'octet d'override du périphérique.
            /// </summary>
            STATE_GETOVERRIDE,
            /// <summary>
            /// Etat pour la lecture des options du périphérique.
            /// </summary>
            STATE_GETOPTION,
            /// <summary>
            /// Etat pour l'enregistrement des chemins de tri pour un canal du périphérique.
            /// </summary>
            STATE_SETSORTERPATH,
            /// <summary>
            /// Etat pour la lecture  des chemins de tri pour un canal du périphérique.
            /// </summary>
            STATE_GETSORTERPATH,
            /// <summary>
            /// Etat pour l'enregistrement du chemin de tri par defaut pour le périphérique.
            /// </summary>
            STATE_SETDEFAULTSORTERPATH,
            /// <summary>
            /// Etat pour la lecture du chemin de tri par defaut pour le périphérique.
            /// </summary>
            STATE_GETDEFAULTSORTERPATH,
            /// <summary>
            /// Etat pour la lecture de l'identification de la pièce reconnue dans un canal du périphérique.
            /// </summary>
            STATE_GETCOINID,
            /// <summary>
            /// Etat pour fixer les limites de pièces à accepter pour une transaction dans un périphérique.
            /// </summary>
            STATE_ACCEPTLIMIT,
            /// <summary>
            /// Etat pour lire la version ccTalk utiliser par le périphérique.
            /// </summary>
            STATE_COMMSREVISION,
            /// <summary>
            /// Etat pour analyser le buffer de crédit.
            /// </summary>
            STATE_CHECKCREDIBUFFER,
            /// <summary>
            /// Etat quand le périphérique est inactif.
            /// </summary>
            STATE_IDLE,
            /// <summary>
            /// Arrêt de la tâche.
            /// </summary>
            STATE_STOP,
        }
        /// @}
    }
}