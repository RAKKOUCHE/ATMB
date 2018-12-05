/// \file CDevice.cs
/// \brief Fichier contenant la classe CDevice.
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceLibrary
{
    /// <summary>
    /// Classe abstraite parent de tous les périphériques
    /// </summary>
    public abstract partial class CDevice
    {
        /// <summary>
        /// Niveaux des périphériques.
        /// </summary>
        public CLevel deviceLevel;

        /// <summary>
        /// 
        /// </summary>
        public static CInserted denominationInserted;

        /// <summary>
        /// Numéro de série du périphérique
        /// </summary>
        public virtual int SerialNumber
        {
            get;
        }

        /// <summary>
        /// Identifiant du  fabricant
        /// </summary>
        public abstract string Manufacturer
        {
            get;
        }

        /// <summary>
        /// Fonction demandant le code produit
        /// </summary>
        /// <returns>Une chaîne de caractères contenant le code produit</returns>
        public abstract string ProductCode
        {
            get;
        }

        private bool isPresent;
        /// <summary>
        /// Flag indiquant si le hopper est detecté.
        /// </summary>
        public bool IsPresent
        {
            get => isPresent;
            set => isPresent = value;
        }

        /// <summary>
        /// Initialisation des périphériques.
        /// </summary>
        public abstract void Init();


        /// <summary>
        /// Constructeur
        /// </summary>
        public CDevice()
        {
            if (denominationInserted == null)
            {
                denominationInserted = new CInserted();
            }
        }
    }
}