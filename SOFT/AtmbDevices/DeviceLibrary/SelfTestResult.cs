/// \file SelfTestResult.cs
/// \brief Fichier contenant l'énumération des commandes ccTalk spécifiques aux monnayeurs.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHEnamespace DeviceLibrary

namespace DeviceLibrary
{
    public partial class CccTalk : CDevice
    {
        /// \addtogroup Erreurs
        /// @{        
        /// <summary>
        /// Groupe des codes erreur renvoyés par les périphériques
        /// </summary>
        /// <summary>
        /// Liste des fautes possibles retournées par un self test.
        /// </summary>
        public enum SelfTestResult
    {
            /// <summary>
            /// OK
            /// </summary>
            OK = 0,
            /// <summary>
            /// Erreur de checksum
            /// </summary>
            ERRORCHECKSUM = 1,
            /// <summary>
            /// Erreur sur une bobine de mesure.
            /// </summary>
            ERRORCOILMEASUREMENT = 2,
            /// <summary>
            /// Erreur sur le senseur de crédit.
            /// </summary>
            ERRORCREDITSENSOR = 3,
            /// <summary>
            /// Erreur sur le sensor piezzo.
            /// </summary>
            ERRORPIEZOSENSOR = 4,
            /// <summary>
            /// Erreur sur le réflecteur.
            /// </summary>
            ERRORREFLECTIVESENSOR = 5,
            /// <summary>
            /// Erreur sur les senseurs de diamètre.
            /// </summary>
            ERRORDIAMETERSENSOR = 6,
            /// <summary>
            /// Erreur sur le sensor d'éveil
            /// </summary>
            ERRORONWAKEUPSENSOR = 7,
            /// <summary>
            /// Erreur sur le senseur de sortie
            /// </summary>
            ERROREXITSENSOR = 8,
            /// <summary>
            /// Erreur sur le checkcum de la mémoire flash.
            /// </summary>
            NVRAMCHECKSUM = 9,
            /// <summary>
            /// Erreur sur le clavier.
            /// </summary>
            ERRORKEYPAD = 14,
            /// <summary>
            /// Code sur un bouton de contact.
            /// </summary>
            ERRORBUTTON = 15,
            /// <summary>
            /// Erreur sur l'afficheur.
            /// </summary>
            ERRORDISPLAY = 16,
            /// <summary>
            /// Erreur sur l'audit des pièces.
            /// </summary>
            COINAUDITERROR = 17,
            /// <summary>
            /// Erreur sur le senseur de rejet.
            /// </summary>
            ERRORONREJECTSENSOR = 18,
            /// <summary>
            /// Erreur sur le système de rejet.
            /// </summary>
            ERRORONCOINRETURNMECH = 19,
            /// <summary>
            /// Erreur sur le système anti-fishing
            /// </summary>
            ERRORCOSMECH = 20,
            /// <summary>
            /// Erreur sur le senseur RIM
            /// </summary>
            ERRORRIM = 21,
            /// <summary>
            /// Erreur sur le thermistor.
            /// </summary>
            ERRORTHERMISTOR = 22,
            /// <summary>
            /// Erreur moteur
            /// </summary>
            ERRORMOTOR = 23,
            /// <summary>
            /// Erreur sur le senseur de sortie du hopper.
            /// </summary>
            ERRORPAYOUTSENSOR = 26,
            /// <summary>
            /// Erreur sur un senseur de niveau.
            /// </summary>
            ERRORLEVELSENSOR = 27,
            /// <summary>
            /// Erreur sur le checksum d'un blco de données.
            /// </summary>
            ERRORDATABLOCKCHECSUM = 30,
            /// <summary>
            /// Port série interne en défaut
            /// </summary>
            ERRORINTERNALCOM = 32,
            /// <summary>
            /// Alimentation en dehors des valeurs limits.
            /// </summary>
            ERRORPOWERSUPPLY = 33,
            /// <summary>
            /// La température externe est en dehors des valeurs limites définie dans le spécification du périphérique.
            /// </summary>
            ERRORTEMP = 34,
            /// <summary>
            /// Erreur dans le systeme 2 entrées.
            /// </summary>
            ERRORDCE = 35,
            /// <summary>
            /// Erreur senseur dans le lecteur de billets.
            /// </summary>
            ERRORBVSENSOR = 36,
            /// <summary>
            /// Erreur dans le déplacement du billet dans le lecteur de billets.
            /// </summary>
            ERRORBVTRANSPORT = 37,
            /// <summary>
            /// Defaut dans l'empileur
            /// </summary>
            ERRORSTACKER = 38,
            /// <summary>
            /// Bourrage billet
            /// </summary>
            BILLJAMMED = 39,
            /// <summary>
            /// Erreur dans la RAM.
            /// </summary>
            ERRORTESTRAM = 40,
            /// <summary>
            /// Erreur dans le système anti-fishing du lecteur de billet.
            /// </summary>
            ERRORSTRINGSENSOR = 41,
            /// <summary>
            /// Erreur la porte d'acceptation est ouverte.
            /// </summary>
            ERRORACCEPTGATEOPEN = 42,
            /// <summary>
            /// Erreur la porte d'acceptation est fermée.
            /// </summary>
            ERRORACCEPTGATECLOSE = 43,
            /// <summary>
            /// L'empileur est absent.
            /// </summary>
            STACKERMISSING = 44,
            /// <summary>
            /// L'empileur est plein.
            /// </summary>
            STACKERFULL = 45,
            /// <summary>
            /// Echec de l'effacement de la mémoire flash.
            /// </summary>
            ERRORERASEFLASHMEM = 46,
            /// <summary>
            /// Echec de l'écriture de la mémoire flash.
            /// </summary>
            ERRORWRITEFLASHMEM = 47,
            /// <summary>
            /// Defaut de communication avec le périphérique.
            /// </summary>
            ERRORSLAVEDEVICE = 48,
            /// <summary>
            /// Erreur sur un optocoupleur.
            /// </summary>
            ERROROPTOSENSOR = 49,
            /// <summary>
            /// Erreur batterie
            /// </summary>
            ERRORBATTERY = 50,
            /// <summary>
            /// La porte est restée ouverte
            /// </summary>
            ERRORDOOROPEN = 51,
            /// <summary>
            /// Erreur sur un microswitch.
            /// </summary>
            ERRORMICROSWITCH = 52,
            /// <summary>
            /// Erreur sur l'horloge en temps réel.
            /// </summary>
            ERRORRTC = 53,
            /// <summary>
            /// Erreur Firmware.
            /// </summary>
            ERRORFW = 54,
            /// <summary>
            /// Erreur d'initialisation.
            /// </summary>
            ERRORINIT = 55,
            /// <summary>
            /// Le périphérique est utilisé en dehors des limites de courant deéfinie par le fabricant.
            /// </summary>
            ERRORCURRENTSUPPLY = 56,
            /// <summary>
            /// Mode bootloader forcé.
            /// </summary>
            FORCEBOOTLOADERMODE = 57,
            /// <summary>
            /// Bourrage pièces.
            /// </summary>
            COINJAM = 253,
            /// <summary>
            /// Débloqué.
            /// </summary>
            DISKBLOCKED = 254,
            /// <summary>
            /// Erreur inconnue.
            /// </summary>
            ERRORUNKNOW = 255,
        }
        /// @}
    }
}