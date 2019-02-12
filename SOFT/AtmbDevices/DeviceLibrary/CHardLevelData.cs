/// \file CHopper.cs
/// \brief Fichier contenant la classe CHopper.
/// \date 28 11 2018
/// \version 1.1
/// Modification du traitements des erreurs du hopper.
/// Création de sous fichiers contenant les classes.
/// \author Rachid AKKOUCHE



namespace DeviceLibrary
{
    /// <summary>
    /// Class contenant les informations du message envoyé lors du changement d'état des niveaux soft.
    /// </summary>
    public class CHardLevelData
    {
        /// <summary>
        /// Indique le nombre de pièces dans le hopper.
        /// </summary>
        public long coinsNumber;

        /// <summary>
        /// Indique si le hopper est critique.
        /// </summary>
        public bool isHCritical;

        /// <summary>
        /// Seuil atteint.
        /// </summary>
        public CLevel.HardLevel level;

        /// <summary>
        /// Nom du hopper.
        /// </summary>
        public string nameOfHopper;
    }
}