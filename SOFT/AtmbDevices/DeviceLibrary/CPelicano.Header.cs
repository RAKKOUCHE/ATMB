/// \file CPelicano.Header.cs
/// \brief Fichier contenant l'énumération des commandes ccTalk spécifiques au Pelicano.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    /// <summary>
    /// Classe du Pelicano.
    /// </summary>
    public partial class CPelicano : CCoinValidator
    {
        /// \addtogroup Headers
        /// @{
        /// <summary>
        /// Groupe des commandes ccTalk.
        /// </summary>
        /// <summary>
        /// Enumération des commandes spécifiques au Pelicano
        /// </summary>
        private new enum Header : byte
        {
            /// <summary>
            /// Lecture des optocoupleurs à la sortie des pièces.
            /// </summary>
            READOPTOSTATES = 236,

            /// <summary>
            /// Lecture des caractèristiques d'acceptation.
            /// </summary>
            REQUESTOPTIONFLAG = 213,

            /// <summary>
            /// Fixe ou lit les limites d'acceptation.
            /// </summary>
            SETGETACCEPTLIMIT = 135,
        }
    }
}