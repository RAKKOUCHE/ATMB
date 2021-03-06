﻿/// \file CHopper.Error.cs
/// \brief Fichier contenant la classe et l'énumération des erreurs hopper.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    /// \addtogroup Erreurs
    /// @{
    /// <summary>
    /// Groupe des codes erreur renvoyés par les périphériques
    /// </summary>
    /// <summary>
    /// Liste des erreurs hopper
    /// </summary>
    public enum HopperError : byte
    {
        /// <summary>
        /// Aucune erreur ou erreur non identifiée.
        /// </summary>
        NON_IDENTIFIEE = 0,

        /// <summary>
        /// Moteur inversé suite à une surconsomation. Probablement du à un bourrage.
        /// </summary>
        MOTORREVERSED,

        /// <summary>
        /// Consomation de courant hors limite du périphérique.
        /// </summary>
        MAXCURRENTEXCEEDED,

        /// <summary>
        /// Optocoupleur obstrué en permanence.
        /// </summary>
        OPTOBLOCKEDPERMANENTLY,

        /// <summary>
        /// Optocoupleur en court-circuit.
        /// </summary>
        OPTOSHORTCIRCUIT,

        /// <summary>
        /// Erreur de checksum dans le bloc de donné A.
        /// </summary>
        CHECKSUMA,

        /// <summary>
        /// Erreur de checksum dans le bloc de donné B.
        /// </summary>
        CHECKSUMB,

        /// <summary>
        /// Erreur de checksum dans le bloc de donné C.
        /// </summary>
        CHECKSUMC,

        /// <summary>
        /// Erreur de checksum dans le bloc de donné D.
        /// </summary>
        CHECKSUMD,

        /// <summary>
        /// Optocoupleur obstrué.
        /// </summary>
        OPTOPATHBLOCKED,

        /// <summary>
        /// Optocoupleur en court-circuit
        /// </summary>
        OPTOSHORTCIRCUITIDLE,

        /// <summary>
        /// Chute de tension pendant l'écriture dans la mémoire.
        /// </summary>
        POWERFAIL,
    }

    /// @}

    /// <summary>
    /// Classe contenant les informations sur une erreur survenue dans le hopper
    /// </summary>
    public class CHopperError
    {
        /// <summary>
        /// Code d'erreur du hopper
        /// </summary>
        public HopperError Code;

        /// <summary>
        /// Indique si le hopper est nécessaire pour le fonctionnement du système.
        /// </summary>
        public bool isHopperCritical;

        /// <summary>
        /// Numéro du hopper
        /// </summary>
        public string nameOfHopper;
    }
}