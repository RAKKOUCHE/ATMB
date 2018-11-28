namespace DeviceLibrary
{
    public partial class CCoinValidator : CcashReader
    {
        /** \addtogroup Erreur
         * @{
         */
        /// <summary>
        /// Enumération des codes erreur du monnayeur
        /// </summary>
        public enum CVErrorCodes : byte
        {
            /// <summary>
            /// Pas d'erreur.
            /// </summary>
            NULL = 0,
            /// <summary>
            /// Pièce non reconnue.
            /// </summary>
            REJECTCOIN = 1,
            /// <summary>
            /// Pièces refusée 
            /// </summary>
            INHIBITEDCOIN = 2,
            /// <summary>
            /// Plusieurs canaux reconnaissent la pièce.
            /// </summary>
            MULTIPLEWINDOWS = 3,
            /// <summary>
            /// Delai de réveil dépassé.
            /// </summary>
            WAKEUP_TO = 4,
            /// <summary>
            /// Temps d'identification dépassé.
            /// </summary>
            VALIDATION_TO = 5,
            /// <summary>
            /// Délai de passage devant le sensor de crédit dépassé.
            /// </summary>
            CREDITSENSOR_TO = 6,
            /// <summary>
            /// Délaide passage devant le sensor du trieur dépassé.
            /// </summary>
            SORTEROPTO_TO = 7,
            /// <summary>
            /// 2 pièces trop rapprochées pour l'dentification.
            /// </summary>
            TOOCLOSECOIN = 8,
            /// <summary>
            /// La porte d'acceptation n'est pas prête.
            /// </summary>
            ACCEPTGATENOTREADY = 9,
            /// <summary>
            /// Le sensor de crédit n'est pas prêt.
            /// </summary>
            CREDITSENSORNOTREADY = 10,
            /// <summary>
            /// Le trieur n'est pas prêt.
            /// </summary>
            SORTERNOTREADY = 11,
            /// <summary>
            /// Le passage de rejet est occupé.
            /// </summary>
            REJECTCOINNOTCLEARED = 12,
            /// <summary>
            /// Le sensor de validation n'est pas prêt.
            /// </summary>
            VALIDATORSENSORNOTREADY = 13,
            /// <summary>
            /// Le sensor de crédit est bloqué.
            /// </summary>
            CREDITSENSORBLOCKED = 14,
            /// <summary>
            /// Le sensor optique du trieur est bloqué.
            /// </summary>
            SORTEROPTOBLOCKED = 15,
            /// <summary>
            /// Séquence de crédit erronée.
            /// </summary>
            CREDITSEQERROR = 16,
            /// <summary>
            /// Retour en arrière de la pièce.
            /// </summary>
            COINGOINGBACK = 17,
            /// <summary>
            /// Passage trop rapide.
            /// </summary>
            COINTOOFAST = 18,
            /// <summary>
            /// Passage trop lent.
            /// </summary>
            COINTOOSLOW = 19,
            /// <summary>
            /// Le mechanisme anti pêche activé.
            /// </summary>
            COSMECHACTIVED = 20,
            /// <summary>
            /// Dual coin entry opto delai dépassé.
            /// </summary>
            DCEOPTO_TO = 21,
            /// <summary>
            /// Opto DCE non vu.
            /// </summary>
            DCEOPTONOTSEE = 22,
            /// <summary>
            /// Credit sensor atteint trop tôt.
            /// </summary>
            CREDITSENSORREACHEDEARLY = 23,
            /// <summary>
            /// Pièce rejetée.
            /// </summary>
            REJECTEDCOIN2 = 24,
            /// <summary>
            /// Fausse pièce rejetée
            /// </summary>
            REJECTSLUG = 25,
            /// <summary>
            /// Sensor de rejet bloqué.
            /// </summary>
            REJECTSENSORBLOCKED = 26,
            /// <summary>
            /// Montant nécessaire introduit trop lentement.
            /// </summary>
            GAMESOVERLOAD = 27,
            /// <summary>
            /// Nombre maximum d'impulsions dépassé
            /// </summary>
            MAXCOINMETERPULSESEXCEEDED = 28,
            /// <summary>
            /// Porte d'acceptation ouverte.
            /// </summary>
            ACCEPTGATEOPENNOTCLOSED = 29,
            /// <summary>
            /// Porte d'acceptation non ouverte.
            /// </summary>
            ACCEPTGATECLOSEDNOTOPEN = 30,
            /// <summary>
            /// Délai dans le séparateur trop long.
            /// </summary>
            MANIFOLDTIMEOUT = 31,
            /// <summary>
            /// Opto coupleur du séparateur bloqué.
            /// </summary>
            MANIFOLDOPTOBLOCKED = 32,
            /// <summary>
            /// Séparateur pas prêt.
            /// </summary>
            MANIFOLDNOTREADY = 33,
            /// <summary>
            /// Les critères de sécurité ont changé.
            /// </summary>
            SECURITYSTATUSCHANGED = 34,
            /// <summary>
            /// Problem sur le moteur.
            /// </summary>
            MOTOREXCEPTION = 35,
            /// <summary>
            /// Pièce avalée.
            /// </summary>
            SWALLOWEDCOIN = 36,
            /// <summary>
            /// Passage trop rapide.
            /// </summary>
            COIN2FAST = 37,
            /// <summary>
            /// Passage trop lent.
            /// </summary>
            COIN2SLOW = 38,
            /// <summary>
            /// Tri incorrecte.
            /// </summary>
            COININCORRECTLYSORTED = 39,
            /// <summary>
            /// Tentative de fraude à la lumiere
            /// </summary>
            EXTERNALLIGHTATTACK = 40,
            /// <summary>
            /// Pièce 1 inhibée
            /// </summary>
            INHIBITEDCOIN_1 = 128,
            /// <summary>
            /// Pièce 2 inhibée
            /// </summary>
            INHIBITEDCOIN_2 = 129,
            /// <summary>
            /// Pièce 3 inhibée
            /// </summary>
            INHIBITEDCOIN_3 = 130,
            /// <summary>
            /// Pièce 4 inhibée
            /// </summary>
            INHIBITEDCOIN_4 = 131,
            /// <summary>
            /// Pièce 5 inhibée
            /// </summary>
            INHIBITEDCOIN_5 = 132,
            /// <summary>
            /// Pièce 6 inhibée
            /// </summary>
            INHIBITEDCOIN_6 = 133,
            /// <summary>
            /// Pièce 7 inhibée
            /// </summary>
            INHIBITEDCOIN_7 = 134,
            /// <summary>
            /// Pièce 8 inhibée
            /// </summary>
            INHIBITEDCOIN_8 = 135,
            /// <summary>
            /// Pièce 9 inhibée
            /// </summary>
            INHIBITEDCOIN_9 = 136,
            /// <summary>
            /// Pièce 10 inhibée
            /// </summary>
            INHIBITEDCOIN_10 = 137,
            /// <summary>
            /// Pièce 11 inhibée
            /// </summary>
            INHIBITEDCOIN_11 = 138,
            /// <summary>
            /// Pièce 12 inhibée
            /// </summary>
            INHIBITEDCOIN_12 = 139,
            /// <summary>
            /// Pièce 13 inhibée
            /// </summary>
            INHIBITEDCOIN_13 = 140,
            /// <summary>
            /// Pièce 14 inhibée
            /// </summary>
            INHIBITEDCOIN_14 = 141,
            /// <summary>
            /// Pièce 15 inhibée
            /// </summary>
            INHIBITEDCOIN_15 = 142,
            /// <summary>
            /// Pièce 16 inhibée
            /// </summary>
            INHIBITEDCOIN_16 = 143,
            /// <summary>
            /// Pièce 17 inhibée
            /// </summary>
            INHIBITEDCOIN_17 = 144,
            /// <summary>
            /// Pièce 18 inhibée
            /// </summary>
            INHIBITEDCOIN_18 = 145,
            /// <summary>
            /// Pièce 19 inhibée
            /// </summary>
            INHIBITEDCOIN_19 = 146,
            /// <summary>
            /// Pièce 20 inhibée
            /// </summary>
            INHIBITEDCOIN_20 = 147,
            /// <summary>
            /// Pièce 21 inhibée
            /// </summary>
            INHIBITEDCOIN_21 = 148,
            /// <summary>
            /// Pièce 22 inhibée
            /// </summary>
            INHIBITEDCOIN_22 = 149,
            /// <summary>
            /// Pièce 23 inhibée
            /// </summary>
            INHIBITEDCOIN_23 = 150,
            /// <summary>
            /// Pièce 24 inhibée
            /// </summary>
            INHIBITEDCOIN_24 = 151,
            /// <summary>
            /// Pièce 25 inhibée
            /// </summary>
            INHIBITEDCOIN_25 = 152,
            /// <summary>
            /// Pièce 26 inhibée
            /// </summary>
            INHIBITEDCOIN_26 = 153,
            /// <summary>
            /// Pièce 27 inhibée
            /// </summary>
            INHIBITEDCOIN_27 = 154,
            /// <summary>
            /// Pièce 28 inhibée
            /// </summary>
            INHIBITEDCOIN_28 = 155,
            /// <summary>
            /// Pièce 29 inhibée
            /// </summary>
            INHIBITEDCOIN_29 = 156,
            /// <summary>
            /// Pièce 30 inhibée
            /// </summary>
            INHIBITEDCOIN_30 = 157,
            /// <summary>
            /// Pièce 31 inhibée
            /// </summary>
            INHIBITEDCOIN_31 = 158,
            /// <summary>
            /// Pièce 32 inhibée
            /// </summary>
            INHIBITEDCOIN_32 = 159,
            /// <summary>
            /// Bloc de données requis
            /// </summary>
            DATABLOCKREQUESTED = 253,
            /// <summary>
            /// Mechanisme de rejet activé.
            /// </summary>
            COINRETURNMECHACTIVATED = 254,
            /// <summary>
            /// Alarme inconnue.
            /// </summary>
            UNSPECIFIEDALARM = 255,
        }
    }
}