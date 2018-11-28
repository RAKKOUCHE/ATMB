﻿namespace DeviceLibrary
{
    public partial class CPelicano: CCoinValidator
    {
        /** \addtogroup Header
         * @{
         */
        /// <summary>
        /// Liste des commandes spécifiques au Pelicano
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
