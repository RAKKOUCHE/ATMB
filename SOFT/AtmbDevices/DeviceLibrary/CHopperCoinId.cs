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
    /// Identification de la dénomination de la pièce gerée par le hopper
    /// </summary>
    public class CHopperCoinId
    {
        /// <summary>
        /// Région sur 2 caractères
        /// </summary>
        public string CountryCode;

        /// <summary>
        /// Révision du data set
        /// </summary>
        public char Issue;

        /// <summary>
        /// Valeur en centimes.
        /// </summary>
        public long ValeurCent;
    }
}