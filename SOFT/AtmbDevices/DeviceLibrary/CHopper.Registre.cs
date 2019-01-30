/// \file CHopper.Registre.cs
/// \brief Fichier contenant la liste des états dans les registres.
/// \date 30 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    public partial class CHopper : CccTalk
    {
        /// <summary>
        /// Liste des réponses contenu dans le registre
        /// </summary>
        [System.Flags]
        public enum RegistrePos : byte
        {
            /// <summary>
            /// Pas d'erreur.
            ///</summary>
            NULL = 0,

            /// <summary>
            /// Le pièce par pièce est activé.
            /// </summary>
            SINGLEPAYOUT = 0X02,

            /// <summary>
            /// La rotation du moteur a été inversée lors de la dernière distribution pour corriger un bourrage
            /// </summary>
            MOTORREVERSED = 0X04,

            /// <summary>
            /// Le hopper n'est pas activé.
            /// </summary>
            DISABLED = 0X80,

            /// <summary>
            /// Le hopper est protéger par un code PIN
            /// </summary>
            PINNUMBER = 0X80,

            /// <summary>
            /// La consommation de courant a dépassée celle autorisée.
            /// </summary>
            MAXCURRENTEXCEEDED = 0X01,

            /// <summary>
            /// Un opto est est activé en permanence
            /// </summary>
            OPTOSHORTCIRCUIT = 0X01,

            /// <summary>
            /// Le temps alloué pour l'éjection d'une pièce a été depassé.
            /// </summary>
            /// <remarks>Utilisé pour detecté le hopper vide lors du vidage.</remarks>
            TOOCCURED = 0X02,

            /// <summary>
            /// Le checksum du bloc A de la mémoire non-volatile est erroné.
            /// </summary>
            CHECKSUMA = 0X04,

            /// <summary>
            /// L'opto coupleur de sortie est bloqué
            /// </summary>
            OPTOPATHBLOCKED = 0X08,

            /// <summary>
            /// Le checksum du bloc A de la mémoire non-volatile est erroné.
            /// </summary>
            CHECKSUMB = 0X08,

            /// <summary>
            /// L'optocoupleur est en court-circuit au repos
            /// </summary>
            OPTOSHORTCIRCUITIDLE = 0X10,

            /// <summary>
            /// Le checksum du bloc A de la mémoire non-volatile est erroné.
            /// </summary>
            CHECKSUMC = 0X10,

            /// <summary>
            /// Un optocoupleur est bloqué en permanence
            /// </summary>
            OPTOBLOCKEDPERMANENTLY = 0X20,

            /// <summary>
            /// Le checksum du bloc A de la mémoire non-volatile est erroné.
            /// </summary>
            CHECKSUMD = 0X20,

            /// <summary>
            /// Le hopper a été alimenté et le reset n'a pas encore été effectué.
            /// </summary>
            POWERUP = 0X40,

            /// <summary>
            /// L'alimentation a été coupée pendant une distribution
            /// </summary>
            POWERFAIL = 0X40,
        }
    }
}