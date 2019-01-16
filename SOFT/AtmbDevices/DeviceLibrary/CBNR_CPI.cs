/// \file CBNR_CPI.cs
/// \brief Fichier contenant la classe CBNR_CPI
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using Mei.Bnr;
using Mei.Bnr.Module;

using System;
using System.Collections.Generic;
using System.Threading;

namespace DeviceLibrary
{
    /// <summary>
    /// Class du bnr
    /// </summary>
    public partial class CBNR_CPI : CDevice
    {
        /// <summary>
        /// Instance du bnr.
        /// </summary>
        public static Bnr bnr;

        /// <summary>
        /// 
        /// </summary>
        public class CitemsDispensed
        {
            public int numberBills;
            public int BillValue;
            public int amount;
        }

        /// <summary>
        /// Class contenant le resultat d'une opération de distribution.
        /// </summary>
        public class CResultDispense
        {
            /// <summary>
            /// List contenant les montants par devise distribuée.
            /// </summary>
            public List<CitemsDispensed> listValue;

            public int  Montant;

            /// <summary>
            /// 
            /// </summary>
            public CResultDispense()
            {
                listValue = new List<CitemsDispensed>();
            }
        }

        /// <summary>
        /// Class indiquant la présence ou non d'un module.
        /// </summary>
        /// <remarks>Cette classe est utlisée pour les retraits et les insertions des modles.</remarks>
        private class CModulePosition
        {
            /// <summary>
            /// Nom du module.
            /// </summary>
            public string name;

            /// <summary>
            /// Flag indiquant
            /// </summary>
            public bool isPresent;

            /// <summary>
            /// Indique si un module a été reinséré.
            /// </summary>
            public bool isReinserted;

            /// <summary>
            /// Renvoi le nom du module.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return name;
            }

            /// <summary>
            /// Constructeur
            /// </summary>
            /// <param name="name">Indique le nom du module contenu dans l'instance de la classe.</param>
            /// <param name="isPresent">Initialise la présence du module.</param>
            public CModulePosition(string name, bool isPresent)
            {
                this.name = name;
                this.isPresent = isPresent;
                isReinserted = false;
            }
        }

        /// <summary>
        /// Liste contenant l'état des modules.
        /// </summary>
        private static List<CModulePosition> listModulePosition;

        /// <summary>
        /// Liste contenant la liste des unités logiques.
        /// </summary>
        private static Mei.Bnr.CashUnit.LogicalCashUnit[] listLU;

        /// <summary>
        /// Indique si la demande de denomination est possible.
        /// </summary>
        public static bool isDispensable;

        /// <summary>
        /// Code produit du périphérique.
        /// </summary>
        public override string ProductCode
        {
            get
            {
                return bnr.SystemConfiguration.BnrType.ToString();
            }
        }

        /// <summary>
        /// Retourne l'identifiant du fabricant.
        /// </summary>
        public override string Manufacturer
        {
            get
            {
                return "CPI";
            }
        }

        /// <summary>
        /// Délai maximum accordé pour effectur un reset
        /// </summary>
        private const int BnrResetTimeOutInMS = 60000;

        /// <summary>
        /// Delain maximum pour effectuer une opération.
        /// </summary>
        public const int BnrDefaultOperationTimeOutInMS = 10000;

        private static Etat state;

        /// <summary>
        /// Etat de la machine d'état du BNR.
        /// </summary>
        public static Etat State
        {
            get => state;
            set => state = value;
        }

        private int amountToDispense;

        /// <summary>
        /// Montant à distribuer ou dont on veur vérifier la disponibilité.
        /// </summary>
        public int AmountToDispense
        {
            get => amountToDispense;
            set => amountToDispense = value;
        }

        /// <summary>
        /// Thread utilisé pour le BNR
        /// </summary>
        public readonly Thread bnrTask;

        /// <summary>
        /// Evénement permettant de gérer les function asynchrone.
        /// </summary>
        public static AutoResetEvent ev;

        /// <summary>
        /// Instance de la class Cerror.
        /// </summary>
        private static Cerror errorInfo;

        private bool isBNRToBeDeactivated;

        /// <summary>
        /// Indique si le monnayeur doit être adtivé.
        /// </summary>
        public bool IsBNRToBeDeactivated
        {
            get => isBNRToBeDeactivated;
            set => isBNRToBeDeactivated = value;
        }

        /// <summary>
        /// Flag indiquant si les billets sont présentés à l'usager.
        /// </summary>
        public static bool isCashPresent;

        /// <summary>
        /// Flag indiquant si les billets ont été repris.
        /// </summary>
        public static bool isCashTaken;

        private bool isBNRToBeActivated;

        /// <summary>
        /// Indique si le monnayeur doit être adtivé.
        /// </summary>
        public bool IsBNRToBeActivated
        {
            get => isBNRToBeActivated;
            set => isBNRToBeActivated = value;
        }

        private int divider;

        /// <summary>
        /// Montant du billet de plus faible valeur;
        /// </summary>
        public int Divider
        {
            get => divider;
            set => divider = value;
        }

        /// <summary>
        /// Tableau contenant la liste des unités logiques.
        /// </summary>
        private Mei.Bnr.CashUnit.LogicalCashUnit[] logicalCashUnits;

        /// <summary>
        ///Vérifie la possibilité de distribuer le montant indiqué en paramètre et le distribue le cas échéant.
        /// </summary>
        /// <param name="amount">Montant à distribuer.</param>
        public void CheckAndDispense(int amount)
        {
            try
            {
                if ((amount > 0) && (bnr != null) && IsPresent)
                {
                    AmountToDispense = amount;
                    isDispensable = false;
                    while (State != Etat.STATE_IDLE)
                        ;
                    State = Etat.STATE_DENOMINATE;
                    while (State != Etat.STATE_IDLE)
                        ;
                    if (isDispensable)
                    {
                        State = Etat.STATE_DISPENSE;
                        while (State != Etat.STATE_IDLE)
                            ;
                    }
                }
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Recherche la plus petite denomination disponible.
        /// </summary>
        /// <returns></returns>
        public int SearchDivider
        {
            get
            {
                int result = int.MaxValue;
                try
                {
                    isDispensable = false;
                    while (bnr.TransactionStatus.CurrentTransaction != TransactionType.None)
                    {
                        Thread.Sleep(200);
                    }
                    foreach (Mei.Bnr.CashUnit.LogicalCashUnit logicalCashUnit in bnr.CashUnit.LogicalCashUnits)
                    {
                        if ((logicalCashUnit.Count > 0) &&
                            ((logicalCashUnit.CuKind == Mei.Bnr.CashUnit.CashUnitKind.Dispense) || (logicalCashUnit.CuKind == Mei.Bnr.CashUnit.CashUnitKind.Recycle)) &&
                            (logicalCashUnit.Status == Mei.Bnr.CashUnit.CashUnitStatus.Ok) && (logicalCashUnit.CashType.Value < result))
                        {
                            result = logicalCashUnit.CashType.Value;
                        }
                    }
                }
                catch (Exception E)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType().ToString(), E.Message, E.StackTrace.ToString());
                }
                return result;
            }
        }

        /// <summary>
        /// Levée pour une opération sur un billet.
        /// </summary>
        /// <param name="cashInOrder"></param>
        private static void CashOccured(XfsCashOrder cashInOrder)
        {
            CDevicesManager.Log.Debug("Cash occured {0}", cashInOrder.Denomination.Amount);
        }

        /// <summary>
        ///  Levée pour un évenment intermédiaure durant une opération .
        /// </summary>
        /// <param name="identificationId"></param>
        /// <param name="operationId"></param>
        /// <param name="result"></param>
        /// <param name="Data"></param>
        private static void IntermediateOccured(int identificationId, OperationId operationId, IntermediateEvents result, object Data)
        {
            CDevicesManager.Log.Debug("Intermediate - IdentificationId : {0} OperationId : {1} IntermediateEvents : {2} Data : {3}",
                identificationId, operationId.ToString(), result.ToString(), Data.ToString());
            switch (operationId)
            {
                case OperationId.BnrAutomaticBillTransfer:
                    break;

                case OperationId.BnrEject:
                    break;

                case OperationId.BnrConfigureCashUnit:
                    break;

                case OperationId.BnrPark:
                    break;

                case OperationId.BnrModulePark:
                    break;

                case OperationId.BnrSelfTest:
                    break;

                case OperationId.BnrCalibrateWithCoupon:
                    break;

                case OperationId.BnrDeleteDenomination:
                    break;

                case OperationId.BnrAddDenomination:
                    break;

                case OperationId.BnrModuleUpdateFirmware:
                    break;

                case OperationId.BnrNoOperation:
                    break;

                case OperationId.Retract:
                    break;

                case OperationId.Reject:
                    break;

                case OperationId.Present:
                {
                    Thread.Sleep(1);
                    break;
                }
                case OperationId.Empty:
                    break;

                case OperationId.CashInRollBack:
                    break;

                case OperationId.CashInEnd:
                    break;

                case OperationId.CashIn:
                {
                    if (result == IntermediateEvents.SubCashin)
                    {
                        denominationInserted.ValeurCent = ((XfsCashOrder)Data).Denomination.Amount;
                        denominationInserted.CVChannel = 0;
                        denominationInserted.CVPath = 0;
                        denominationInserted.TotalAmount += denominationInserted.ValeurCent;

                        lock (eventListLock)
                        {
                            eventsList.Add(new CEvent
                            {
                                reason = CEvent.Reason.MONEYINTRODUCTED,
                                nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                data = denominationInserted,
                            });
                        }
                        CDevicesManager.Log.Debug("Un billet de {0:C2} a été reconnue", (decimal)denominationInserted.ValeurCent / 100);
                    }
                    else
                    {
                        lock (eventListLock)
                        {
                            errorInfo.nameModule = string.Empty;
                            errorInfo.error = ERRORTYPE.BILLREFUSED;
                            eventsList.Add(new CEvent
                            {
                                reason = CEvent.Reason.BNRERREUR,
                                nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                data = errorInfo,
                            });
                        }
                    }
                    break;
                }
                case OperationId.CashInStart:
                    break;

                case OperationId.UpdateDenomination:
                    break;

                case OperationId.QueryDenomination:
                    break;

                case OperationId.SetDateTime:
                    CDevicesManager.Log.Debug("Intermédiaire setDateTime");
                    break;

                case OperationId.GetDateTime:
                    CDevicesManager.Log.Debug("Intermédiaire getDateTime");
                    break;

                case OperationId.UpdateCashUnit:
                    break;

                case OperationId.QueryCashUnit:
                    break;

                case OperationId.Dispense:
                    break;

                case OperationId.Denominate:
                {
                    break;
                }
                case OperationId.Reset:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Levée lors d'un changement d'état à eu lieu.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <param name="extendedResult"></param>
        /// <param name="Data"></param>
        private static void StatusOccured(StatusChanged status, BnrXfsErrorCode result, BnrXfsErrorCode extendedResult, object Data)
        {
            CDevicesManager.Log.Debug("StatusOccured - status : {0} BnrXfsErrorCode : {1} BnrXfsErrorCode : {2} Data : {3}", status.ToString(),
                result.ToString(), extendedResult.ToString(), Data.ToString());
            switch (status)
            {
                case StatusChanged.MaintenanceStatusChanged:
                    break;

                case StatusChanged.CashAvailable:
                    isCashPresent = true;
                    isCashTaken = false;
                    CResultDispense resultDispense = new CResultDispense();
                    
                    foreach (XfsDenominationItem denomination in ((XfsCashOrder)Data).Denomination.Items)
                    {
                        CitemsDispensed itemsDispensed = new CitemsDispensed();
                        itemsDispensed.numberBills =  denomination.Count;
                        itemsDispensed.BillValue = listLU[denomination.Unit - 1].CashType.Value;
                        itemsDispensed.amount = itemsDispensed.BillValue * itemsDispensed.numberBills;
                        resultDispense.listValue.Add(itemsDispensed);
                    }
                    resultDispense.Montant = ((XfsCashOrder)Data).Denomination.Amount;
                    lock (eventListLock)
                    {
                        eventsList.Add(new CEvent
                        {
                            reason = CEvent.Reason.BNRDISPENSE,
                            nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                            data = resultDispense,
                        });
                    }
                    CDevicesManager.Log.Debug("Un billet de {0:C2} a été reconnue", (decimal)denominationInserted.ValeurCent / 100);

                    break;

                case StatusChanged.CashTaken:
                    isCashTaken = true;
                    isCashPresent = false;
                    break;

                case StatusChanged.TransportChanged:
                    break;

                case StatusChanged.DeviceStatusChanged:
                {
                    if (Data != null)
                    {
                        switch ((DeviceStatus)Data)
                        {
                            case DeviceStatus.OffLine:
                            {
                                State = Etat.STATE_RESET;
                                break;
                            }
                            case DeviceStatus.OnLine:
                            {
                                break;
                            }
                            case DeviceStatus.HardwareError:
                            case DeviceStatus.UserError:
                            {
                                HardwareError((DeviceStatus)Data);
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                    }
                    break;
                }
                case StatusChanged.CashUnitThreshold:
                    break;

                case StatusChanged.CashUnitConfigurationChanged:
                {
                    try
                    {
                        foreach (CModulePosition modulePosition in listModulePosition)
                        {
                            if (modulePosition.name == ((Mei.Bnr.CashUnit.CashUnit)Data).PhysicalCashUnits[0].Name)
                            {
                                CEvent eventStatus = new CEvent()
                                {
                                    nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                    data = modulePosition.name,
                                };
                                if (!(modulePosition.isPresent = ((Mei.Bnr.CashUnit.CashUnit)Data).PhysicalCashUnits[0].Status != Mei.Bnr.CashUnit.CashUnitStatus.Missing))
                                {
                                    modulePosition.isReinserted = false;
                                    eventStatus.reason = CEvent.Reason.BNRMODULEMANQUANT;
                                }
                                else
                                {
                                    modulePosition.isReinserted = true;
                                    eventStatus.reason = CEvent.Reason.BNRMODULEREINSERE;
                                }
                                lock (eventListLock)
                                {
                                    eventsList.Add(eventStatus);
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                    }
                    break;
                }
                case StatusChanged.CashUnitChanged:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Levée à la fin d; une opération.
        /// </summary>
        /// <param name="identificationId"></param>
        /// <param name="operationId"></param>
        /// <param name="result"></param>
        /// <param name="extendedResult"></param>
        /// <param name="Data"></param>
        private static void Bnr_OperationCompletedEvent(int identificationId, OperationId operationId, BnrXfsErrorCode result, BnrXfsErrorCode extendedResult, object Data)
        {
            CDevicesManager.Log.Debug("complete - IdentificationId : {0} OperationId : {1} BnrXfsErrorCode : {2} BnrXfsErrorCode : {3} Data : {4}",
    identificationId, operationId.ToString(), result.ToString(), extendedResult.ToString(), Data != null ? Data.ToString() : null);

            switch (operationId)
            {
                case OperationId.BnrAutomaticBillTransfer:
                    break;

                case OperationId.BnrEject:
                    break;

                case OperationId.BnrConfigureCashUnit:
                    break;

                case OperationId.BnrPark:
                    break;

                case OperationId.BnrModulePark:
                    break;

                case OperationId.BnrSelfTest:
                    break;

                case OperationId.BnrCalibrateWithCoupon:
                    break;

                case OperationId.BnrDeleteDenomination:
                    break;

                case OperationId.BnrAddDenomination:
                    break;

                case OperationId.BnrModuleUpdateFirmware:
                    break;

                case OperationId.BnrNoOperation:
                    break;

                case OperationId.Retract:
                    break;

                case OperationId.Reject:
                    break;

                case OperationId.Present:
                {

                    Thread.Sleep(1);
                    break;
                }
                case OperationId.Empty:
                    break;

                case OperationId.CashInRollBack:
                {
                    CDevicesManager.ToPay -= ((XfsCashOrder)Data).Denomination.Amount;
                    if (CDevicesManager.ToPay < 0)
                    {
                        CDevicesManager.ToPay = 0;
                    }
                    break;
                }
                case OperationId.CashInEnd:
                    break;

                case OperationId.CashIn:
                    break;

                case OperationId.CashInStart:

                    break;

                case OperationId.UpdateDenomination:
                    break;

                case OperationId.QueryDenomination:
                    break;

                case OperationId.SetDateTime:
                    break;

                case OperationId.GetDateTime:
                    break;

                case OperationId.UpdateCashUnit:
                    break;

                case OperationId.QueryCashUnit:
                    break;

                case OperationId.Dispense:
                {
                    break;
                }
                case OperationId.Denominate:
                {
                    isDispensable = result == BnrXfsErrorCode.Success;
                    break;
                }
                case OperationId.Reset:
                {
                    try
                    {
                        if (result != BnrXfsErrorCode.Success)
                        {
                            HardwareError(DeviceStatus.UserError);
                        }
                        else
                        {
                            foreach (CModulePosition modulePosition in listModulePosition)
                            {
                                if (modulePosition.isReinserted == true)
                                {
                                    modulePosition.isReinserted = false;

                                    CEvent eventStatus = new CEvent()
                                    {
                                        reason = CEvent.Reason.BNRRAZMETER,
                                        nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                        data = modulePosition.name,
                                    };
                                    if (modulePosition.name == "CB")
                                    {
                                        CDevicesManager.Log.Info("Raz des compteurs de la caisse du BNR.");
                                        bnr.ResetCashboxCuContent(true);
                                        lock (eventListLock)
                                        {
                                            eventsList.Add(eventStatus);
                                        }
                                    }
                                    if (modulePosition.name == "LO1")
                                    {
                                        CDevicesManager.Log.Info("RAZ des compteurs du loader du BNR.");
                                        bnr.SetLoaderCuContent("LO1", 0);
                                        lock (eventListLock)
                                        {
                                            eventsList.Add(eventStatus);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                    }
                    break;
                }
                default:
                {
                    break;
                }
            }
            ev.Set();
        }

        /// <summary>
        /// Recherche de l'erreur et du module en erreur
        /// </summary>
        private static void HardwareError(DeviceStatus deviceStatus)
        {
            errorInfo.nameModule = string.Empty;
            switch (deviceStatus)
            {
                case DeviceStatus.HardwareError:
                {
                    foreach (Module m in bnr.Modules)
                    {
                        if (m.Status.OperationalStatus != OperationalStatus.Operational)
                        {
                            errorInfo.nameModule = m.ToString();
                            if (bnr.SystemStatusOverview.SystemStatus.ErrorCode != SystemErrorCode.NoError)
                            {
                                errorInfo.error = ERRORTYPE.BNR_OUVERT;
                                break;
                            }
                            if (bnr.SystemStatusOverview.SystemStatus.BillTransportStatus != Mei.Bnr.BillTransportStatus.Ok)
                            {
                                errorInfo.error = ERRORTYPE.BOURRAGE;
                                break;
                            }
                            if (bnr.SystemStatusOverview.SystemStatus.CashTypeStatus != Mei.Bnr.CashTypeStatus.Ok)
                            {
                                errorInfo.error = ERRORTYPE.TYPE_BILLET_ERREUR;
                                break;
                            }
                            if (bnr.SystemStatusOverview.SystemStatus.SystemBillStorageStatus != SystemBillStorageStatus.Ok)
                            {
                                errorInfo.error = ERRORTYPE.STOCKAGE_ERREUR;
                            }
                        }
                    }
                    break;
                }
                case DeviceStatus.UserError:
                {
                    errorInfo.error = ERRORTYPE.BNR_OUVERT;
                    break;
                }
                case DeviceStatus.OffLine:
                    break;

                case DeviceStatus.OnLine:
                    break;

                default:
                    break;
            }
            lock (eventListLock)
            {
                eventsList.Add(new CEvent
                {
                    reason = CEvent.Reason.BNRERREUR,
                    nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                    data = errorInfo
                });
            }
        }

        /// <summary>
        /// Initialisation de la classe du BNR.
        /// </summary>
        public override void Init()
        {
            try
            {
                Bnr.OperationCompletedEvent += new OperationCompleted(Bnr_OperationCompletedEvent);
                Bnr.CashOccuredEvent += new CashOccured(CashOccured);
                Bnr.IntermediateOccuredEvent += new IntermediateOccured(IntermediateOccured);
                Bnr.StatusOccuredEvent += new StatusOccured(StatusOccured);
                IsBNRToBeActivated = false;
                errorInfo = new Cerror();
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Machine d'état du BNR.
        /// </summary>
        public override void Task()
        {
            while (true)
            {
                try
                {
                    switch (State)
                    {
                        case Etat.STATE_INIT:
                        {
                            Init();

                            State = Etat.STATE_OPEN_API;
                            break;
                        }
                        case Etat.STATE_OPEN_API:
                        {
                            try
                            {
                                bnr.Open();
                                bnr.Cancel();
                                if (bnr.TransactionStatus.CurrentTransaction == TransactionType.Cashin)
                                {
                                    ev.Reset();
                                    bnr.CashInEnd();
                                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                                }
                                if (bnr.TransactionStatus.CurrentTransaction == TransactionType.Dispense)
                                {
                                    ev.Reset();
                                    bnr.Present();
                                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                                }
                                foreach (Mei.Bnr.CashUnit.PhysicalCashUnit physicalCashUnit in bnr.CashUnit.PhysicalCashUnits)
                                {
                                    CModulePosition modulePostion = new CModulePosition(physicalCashUnit.Name,
                                        physicalCashUnit.Status != Mei.Bnr.CashUnit.CashUnitStatus.Missing);
                                    listModulePosition.Add(modulePostion);
                                }
                                State = Etat.STATE_RESET;
                            }
                            catch (Exception E)
                            {
                                IsPresent = false;
                                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                                errorInfo.error = ERRORTYPE.DLL_NOT_FREE;
                                errorInfo.nameModule = string.Empty;
                                lock (eventListLock)
                                {
                                    eventsList.Add(new CEvent
                                    {
                                        reason = CEvent.Reason.BNRERREUR,
                                        nameOfDevice = "BNR",
                                        data = errorInfo
                                    });
                                }
                                State = Etat.STATE_STOP;
                            }
                            break;
                        }
                        case Etat.STATE_RESET:
                        {
                            try
                            {
                                ev.Reset();
                                bnr.Reset();
                                if (!ev.WaitOne(BnrResetTimeOutInMS))
                                {
                                    throw new Exception("Le reset du BNR a échoué!");
                                }
                                IsPresent = true;
                                State = Etat.STATE_SETDATETIME;
                            }
                            catch (Exception E)
                            {
                                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                                Thread.Sleep(BnrResetTimeOutInMS * 2);
                                evReady.Set();
                            }
                            break;
                        }
                        case Etat.STATE_SETDATETIME:
                        {
                            //NECESSAIRE POUR LES BNR D'ANCIENNE GENERATION.
                            bnr.DateTime = DateTime.Now;
                            State = Etat.STATE_GETMODULE;
                            break;
                        }
                        case Etat.STATE_GETMODULE:
                        {
                            foreach (Module m in bnr.Modules)
                            {
                                CDevicesManager.Log.Info(m.ToString() + " : " + m.Status.OperationalStatus);
                            }
                            State = Etat.STATE_GETLUS;
                            break;
                        }
                        case Etat.STATE_GETLUS:
                        {
                            listLU = new Mei.Bnr.CashUnit.LogicalCashUnit[bnr.CashUnit.LogicalCashUnits.Count];
                            bnr.CashUnit.LogicalCashUnits.CopyTo(listLU);
                            State = Etat.STATE_GETSTATUS;
                            break;
                        }

                        case Etat.STATE_GETSTATUS:
                        {
                            CDevicesManager.Log.Debug(bnr.Status.PositionStatusList[0].ShutterStatus.ToString());
                            CDevicesManager.Log.Debug(bnr.Status.PositionStatusList[1].ShutterStatus.ToString());
                            CDevicesManager.Log.Debug(bnr.Status.ToString);
                            State = Etat.STATE_IDLE;
                            evReady.Set();
                            break;
                        }
                        case Etat.STATE_IDLE:
                        {
                            if (IsBNRToBeActivated)
                            {
                                IsBNRToBeActivated = false;
                                State = Etat.STATE_ENABLE;
                            }
                            if (IsBNRToBeDeactivated)
                            {
                                IsBNRToBeDeactivated = false;
                                State = Etat.STATE_DISABLE;
                            }
                            break;
                        }
                        case Etat.STATE_STOP:
                        {
                            ev.Set();
                            evReady.Set();
                            bnrTask.Abort();
                            break;
                        }
                        case Etat.STATE_ENABLE:
                        {
                            try
                            {
                                ev.Reset();
                                try
                                {
                                    bnr.CashInStart();
                                }
                                catch (Exception E)
                                {
                                    CDevicesManager.Log.Error(E.Message);
                                }
                                if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                {
                                    throw new Exception("L'opération CashinStart a échouée.");
                                }
                                CDevicesManager.Log.Debug("cASHIN");
                                if (bnr.CashIn(20000000) <= 0)
                                {
                                    throw new Exception("L'opération cashIn a échouée.");
                                }
                            }
                            catch (Exception E)
                            {
                                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            }
                            Thread.Sleep(200);
                            State = Etat.STATE_IDLE;
                            break;
                        }
                        case Etat.STATE_DISABLE:
                        {
                            try
                            {
                                CDevicesManager.Log.Debug("cancel.");
                                bnr.Cancel();
                                if (bnr.TransactionStatus.CurrentTransaction == TransactionType.Cashin)
                                {
                                    ev.Reset();
                                    bnr.CashInEnd();
                                    CDevicesManager.Log.Debug("Cashinend.");
                                    if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                    {
                                        throw new Exception();
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            }
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_DISPENSE:
                        {
                            bnr.Dispense(AmountToDispense, "AAA");
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_DENOMINATE:
                        {
                            //par defaut, le bnr ne peut pas distribuer
                            isDispensable = false;
                            Divider = SearchDivider;
                            ev.Reset();
                            //Verifie si il peut distribuer, si oui la variable isDispensable sera positionnée à true.
                            bnr.Denominate(AmountToDispense, "AAA");
                            if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS) || !isDispensable)
                            {
                                errorInfo.nameModule = string.Empty;
                                errorInfo.error = ERRORTYPE.NOTDISTRIBUABLE;
                                lock (eventListLock)
                                {
                                    eventsList.Add(new CEvent
                                    {
                                        reason = CEvent.Reason.BNRERREUR,
                                        nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                        data = errorInfo
                                    });
                                }

                                throw new Exception("Montant de {0} n'est pas distribuable.");
                            }
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        default:
                        {
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }
                catch (Exception E)
                {
                    if (CBNR_CPI.isDispensable)
                    {
                        ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                        if (!isCashPresent)
                        {
                            errorInfo.nameModule = string.Empty;
                            errorInfo.error = ERRORTYPE.NOTDISPENSED;
                            lock (eventListLock)
                            {
                                eventsList.Add(new CEvent
                                {
                                    reason = CEvent.Reason.BNRERREUR,
                                    nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                    data = errorInfo
                                });
                            }
                        }
                    }
                    State = Etat.STATE_IDLE;
                    CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public CBNR_CPI()
        {
            try
            {
                IsPresent = false;
                bnr = new Bnr();
                listModulePosition = new List<CModulePosition>();
                State = Etat.STATE_INIT;
                ev = new AutoResetEvent(true);
                bnrTask = new Thread(Task);
                bnrTask.Start();
            }
            catch (Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            evReady.WaitOne(90000);
        }
    }
}