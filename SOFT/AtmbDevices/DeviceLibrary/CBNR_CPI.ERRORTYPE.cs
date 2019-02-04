/// \file CBNR_CPI.ERRORTYPE.cs
/// \brief Fichier contenant la classe ERRORTYPE
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

namespace DeviceLibrary
{
    public partial class CBNR_CPI
    {
        /// <summary>
        /// Enumération des erreurs du BNR.
        /// </summary>
        public enum Errortype
        {
            /// <summary>
            /// Billet bloqué dans le BNR.
            /// </summary>
            BOURRAGE,

            /// <summary>
            /// Le bnr est ouvert ou déverrouillé.
            /// </summary>
            BNR_OUVERT,

            /// <summary>
            /// Indique une erreurs sur le paramètrage d'un billet.
            /// </summary>
            TYPE_BILLET_ERREUR,

            /// <summary>
            /// Erreur de stockage des billets.
            /// </summary>
            STOCKAGE_ERREUR,

            /// <summary>
            /// Les compteurs du loader n'ont pas été mis à jour.
            /// </summary>
            BNRLOADERMETERMISING,

            /// <summary>
            /// Indique qu'un billet a été refusé.
            /// </summary>
            BILLREFUSED,

            /// <summary>
            /// Indique que le montant n'est pas distribuable.
            /// </summary>
            NOTDISTRIBUABLE,

            /// <summary>
            /// Indique que le montant n'est pas distribué.
            /// </summary>
            NOTDISPENSED,

            /// <summary>
            /// Les dlls sont occupées.
            /// </summary>
            DLL_BNR_NOT_FREE,
        }
    }
        /// <summary>
        /// Class des erreurs du BNR.
        /// </summary>
        public class Cerror
        {
            /// <summary>
            /// Code de l'error.
            /// </summary>
            public CBNR_CPI.Errortype error;

            /// <summary>
            /// Nom du module.
            /// </summary>
            public string nameModule;
    }
}