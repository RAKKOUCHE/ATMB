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
    /// Class contenant les informations sur les compteurs d'une unitée logique
    /// </summary>
    public class CcounterInfo
    {
        /// <summary>
        /// Nome de l'unité physique contenant la dénominations
        /// </summary>
        public string unit;

        /// <summary>
        /// Valeur de la dénomination
        /// </summary>
        public int currencyValue;

        /// <summary>
        /// Code ISO de la dénomination
        /// </summary>
        public string currencyCode;

        /// <summary>
        /// Nombre de billets dans l'unité logique
        /// </summary>
        public int count;

        /// <summary>
        /// Montant dans l'unité logique.
        /// </summary>
        public int amount;

        /// <summary>
        /// Renvoi le nom de l'unité physique contenant les billets.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return unit;
        }

        /// <summary>
        /// Contructeur
        /// </summary>
        /// <param name="unit">Unité contenant la dénomination</param>
        /// <param name="currencyValue">Valeur de la dénomination</param>
        /// <param name="currencyCode">Nom de la devise</param>
        /// <param name="count">Nombre de billets</param>
        public CcounterInfo(string unit, int currencyValue, string currencyCode, int count)
        {
            this.unit = unit;
            this.currencyValue = currencyValue;
            this.currencyCode = currencyCode;
            this.count = count;
        }
    }

    /// <summary>
    /// Class du bnr
    /// </summary>
    public partial class CBNR_CPI : CDevice
    {
        #region VARIABLES

        /// <summary>
        /// Délai maximum accordé pour effectur un reset
        /// </summary>
        private const int BnrResetTimeOutInMS = 60000;

        /// <summary>
        /// Instance de la class Cerror.
        /// </summary>
        private static Cerror errorInfo;

        /// <summary>
        /// Flag indiquant si les billets sont présentés à l'usager.
        /// </summary>
        private static bool isCashPresent;

        /// <summary>
        /// Flag indiquant si les billets ont été repris.
        /// </summary>
        private static bool isCashTaken;

        /// <summary>
        /// Flag indiauqnt si les compteurs du loader on été mis à jour.
        /// </summary>
        private static bool isLoaderMetersUpdade;

        /// <summary>
        /// Liste contenant la liste des unités logiques.
        /// </summary>
        private static Mei.Bnr.CashUnit.LogicalCashUnit[] listLCU;

        /// <summary>
        /// Liste contenant l'état des modules.
        /// </summary>
        private static List<CModulePosition> listModulePosition;

        /// <summary>
        /// Flag indiquant que le billet a été refusé et evite l'envoi d'un message.
        /// </summary>
        private static bool isBillRefused;

        /// <summary>
        /// Delain maximum pour effectuer une opération.
        /// </summary>
        internal const int BnrDefaultOperationTimeOutInMS = 10000;

        internal static CModuleEmptied moduleEmptied;

        /// <summary>
        /// Instance du bnr.
        /// </summary>
        public static Bnr bnr;

        /// <summary>
        /// Evénement permettant de gérer les function asynchrone.
        /// </summary>
        public static AutoResetEvent ev;

        /// <summary>
        /// Indique qu'une opéation d'annulation est en cours.
        /// </summary>
        public static bool isCancelInProgress;

        /// <summary>
        /// Indique si la demande de denomination est possible.
        /// </summary>
        public static bool isDispensable;

        /// <summary>
        /// Flag indiquant que la recharge des recyclers est en cours.
        /// </summary>
        public static bool isReloadInProgress;

        /// <summary>
        /// Thread utilisé pour le BNR
        /// </summary>
        public readonly Thread bnrTask;

        /// <summary>
        /// Indique le nombre de billets à enregister pour le loader.
        /// </summary>
        public uint loaderMeter;

        /// <summary>
        /// Indique si une recherche de la plus petite dénomination doit être effectuée.
        /// </summary>
        public bool isReadyToSearchDivider;

        /// <summary>
        /// List des coompteurs du BNR.
        /// </summary>
        public List<CcounterInfo> listCountersInfo;

        #endregion VARIABLES

        #region PROPRIETEES

        private static Etat state;
        private int amountToDispense;

        /// <summary>
        /// Etat de la machine d'état gérant le BNR.
        /// </summary>
        public static Etat State
        {
            get => state;
            set => state = value;
        }

        /// <summary>
        /// Montant à distribuer ou dont on veur vérifier la disponibilité.
        /// </summary>
        public int AmountToDispense
        {
            get => amountToDispense;
            set => amountToDispense = value;
        }

        private static int divider;
        /// <summary>
        /// Montant du billet de plus faible valeur;
        /// </summary>
        public static int Divider
        {
            get => divider;
            set => divider = value;
        }

        /// <summary>
        /// Chemin du dossier de destination du rapport d'audit.
        /// </summary>
        public string ReportFilePathName;

        /// <summary>
        /// Retourne l'identifiant du fabricant.
        /// </summary>
        public override string Manufacturer => "CPI";

        /// <summary>
        /// Code produit du périphérique.
        /// </summary>
        public override string ProductCode => bnr.SystemConfiguration.BnrType.ToString();

        #endregion PROPRIETEES

        #region METHODES

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
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
            evReady.WaitOne(); ;
        }

        /// <summary>
        /// Recherche la plus petite denomination disponible.
        /// </summary>
        /// <returns></returns>
        private static int SearchDivider
        {
            get
            {
                int result = int.MaxValue;
                try
                {
                    isDispensable = false;
                    foreach (Mei.Bnr.CashUnit.LogicalCashUnit logicalCashUnit in bnr.CashUnit.LogicalCashUnits)
                    {
                        if ((logicalCashUnit.Count > 0) &&
                            ((logicalCashUnit.CuKind == Mei.Bnr.CashUnit.CashUnitKind.Dispense) || (logicalCashUnit.CuKind == Mei.Bnr.CashUnit.CashUnitKind.Recycle)) &&
                            (logicalCashUnit.Status == Mei.Bnr.CashUnit.CashUnitStatus.Ok) && (logicalCashUnit.CashType.Value < result))
                        {
                            if ((result = logicalCashUnit.CashType.Value) == 500)
                            {
                                break;
                            };
                        }
                    }
                }
                catch (Exception exception)
                {
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType().ToString(), exception.Message, exception.StackTrace.ToString());
                }
                return result;
            }
        }

        /// <summary>
        /// Function delegate levée à la fin du opération asynchrone.
        /// </summary>
        /// <param name="identificationId">Numéro de l'opération.</param>
        /// <param name="operationId">Type d'opération.</param>
        /// <param name="result">Resultat de l'opération.</param>
        /// <param name="extendedResult">Compleément d'information le cas échéant.</param>
        /// <param name="data">Information retournée par l'opération, Le type dépend de l'opéation.</param>
        private static void Bnr_OperationCompletedEvent(int identificationId, OperationId operationId, BnrXfsErrorCode result, BnrXfsErrorCode extendedResult, object data)
        {
            CDevicesManager.Log.Debug("complete - IdentificationId : {0} | OperationId : {1} | BnrXfsErrorCode : {2} | BnrXfsErrorCode : {3} | Data : {4}",
    identificationId, operationId.ToString(), result.ToString(), extendedResult.ToString(), data == null ? string.Empty : data.ToString());

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
                    break;
                }
                case OperationId.Empty:
                {
                    if (result == BnrXfsErrorCode.Success)
                    {
                        moduleEmptied.amount = ((XfsCashOrder)data).Denomination.Amount;
                        moduleEmptied.count = ((XfsCashOrder)data).Denomination.Items[0].Count;
                    }
                    else
                    {
                        CDevicesManager.Log.Error("Erreur de vidage du module.");
                    }
                    break;
                }
                case OperationId.CashInRollBack:
                {
                    denominationInserted.TotalAmount -= ((XfsCashOrder)data).Denomination.Amount;
                    break;
                }
                case OperationId.CashInEnd:
                    break;

                case OperationId.CashIn:
                {
                    CDevicesManager.Log.Trace("Opération complete cashin");
                    if ((result == BnrXfsErrorCode.Success) && (((XfsCashOrder)data).Denomination.Amount > 0))
                    {
                        denominationInserted.ValeurCent = ((XfsCashOrder)data).Denomination.Amount;
                        denominationInserted.CVChannel = 0;
                        denominationInserted.CVPath = 0;
                        if (!isReloadInProgress)
                        {
                            denominationInserted.TotalAmount += denominationInserted.ValeurCent;
                        }

                        lock (eventListLock)
                        {
                            eventsList.Add(new CEvent
                            {
                                reason = CEvent.Reason.MONEYINTRODUCTED,
                                nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                data = denominationInserted,
                            });
                        }
                        CDevicesManager.Log.Debug("Un billet de {0:C2} a été reconnu", (decimal)denominationInserted.ValeurCent / 100);
                    }
                    else
                    {
                        if (state != Etat.STATE_CASHIN_END)
                        {
                            State = Etat.STATE_CANCEL;
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
                                        data = modulePosition.moduleName,
                                    };
                                    if (modulePosition.moduleName == "CB")
                                    {
                                        CDevicesManager.Log.Info("Raz des compteurs de la caisse du BNR.");
                                        bnr.ResetCashboxCuContent(true);
                                        lock (eventListLock)
                                        {
                                            eventsList.Add(eventStatus);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
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
        /// Levée pour une opération sur un billet.
        /// </summary>
        /// <param name="cashInOrder"></param>
        private static void CashOccured(XfsCashOrder cashInOrder)
        {
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
                                errorInfo.error = Errortype.BNR_OUVERT;
                                break;
                            }
                            if (bnr.SystemStatusOverview.SystemStatus.BillTransportStatus != Mei.Bnr.BillTransportStatus.Ok)
                            {
                                errorInfo.error = Errortype.BOURRAGE;
                                break;
                            }
                            if (bnr.SystemStatusOverview.SystemStatus.CashTypeStatus != Mei.Bnr.CashTypeStatus.Ok)
                            {
                                errorInfo.error = Errortype.TYPE_BILLET_ERREUR;
                                break;
                            }
                            if (bnr.SystemStatusOverview.SystemStatus.SystemBillStorageStatus != SystemBillStorageStatus.Ok)
                            {
                                errorInfo.error = Errortype.STOCKAGE_ERREUR;
                            }
                        }
                    }
                    break;
                }
                case DeviceStatus.UserError:
                {
                    errorInfo.error = Errortype.BNR_OUVERT;
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
        ///  Levée pour un évenment intermédiaure durant une opération .
        /// </summary>
        /// <param name="identificationId"></param>
        /// <param name="operationId"></param>
        /// <param name="result"></param>
        /// <param name="data"></param>
        private static void IntermediateOccured(int identificationId, OperationId operationId, IntermediateEvents result, object data)
        {
            CDevicesManager.Log.Debug("Intermediate - IdentificationId : {0} OperationId : {1} IntermediateEvents : {2} Data : {3}",
                identificationId, operationId.ToString(), result.ToString(), data == null ? string.Empty : data.ToString());
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
                    break;
                }
                case OperationId.Empty:
                    break;

                case OperationId.CashInRollBack:
                {
                    break;
                }
                case OperationId.CashInEnd:
                    break;

                case OperationId.CashIn:
                {
                    CDevicesManager.Log.Trace("Intermédiare cashIn");
                    if (result == IntermediateEvents.InputRefused)
                    {
                        isBillRefused = true;
                        lock (eventListLock)
                        {
                            errorInfo.nameModule = string.Empty;
                            errorInfo.error = Errortype.BILLREFUSED;
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
                {
                    break;
                }
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
        /// Retourne les billets en escrow.
        /// </summary>
        private static void RollBack()
        {
            try
            {
                if (bnr.TransactionStatus.CurrentTransaction == TransactionType.Cashin)
                {
                    isCashTaken = true;
                    try
                    {
                        bnr.Cancel();
                        Thread.Sleep(500);
                    }
                    catch
                    {
                    }
                    ev.Reset();
                    bnr.CashInRollback();
                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                    while (!isCashTaken)
                        ;
                    ev.Reset();
                    bnr.CashInEnd();
                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
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
                    if (!isBillRefused)
                    {
                        isCashPresent = true;
                        isCashTaken = false;
                        CResultDispense resultDispense = new CResultDispense();

                        foreach (XfsDenominationItem denomination in ((XfsCashOrder)Data).Denomination.Items)
                        {
                            CitemsDispensed itemsDispensed = new CitemsDispensed
                            {
                                count = denomination.Count,
                                BillValue = listLCU[denomination.Unit - 1].CashType.Value
                            };
                            itemsDispensed.amount = itemsDispensed.BillValue * itemsDispensed.count;
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
                    }
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
                            if (modulePosition.moduleName == ((Mei.Bnr.CashUnit.CashUnit)Data).PhysicalCashUnits[0].Name)
                            {
                                CEvent eventStatus = new CEvent()
                                {
                                    nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                    data = modulePosition.moduleName,
                                };
                                if (modulePosition.moduleName.StartsWith("LO"))
                                {
                                    isLoaderMetersUpdade = false;
                                }
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
                                break;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
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
        /// Renvoi en caisse les billets présentés non pris.
        /// </summary>
        private static void Retract()
        {
            try
            {
                if (isCashPresent)
                {
                    ev.Reset();
                    bnr.CancelWaitingCashTaken();
                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                    ev.Reset();
                    bnr.Retract();
                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                    ev.Reset();
                    bnr.Reject();
                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

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
                    //TODO cloturer l'opération en cours.
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
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Vide les recyclers et le loader dans la caisse.
        /// </summary>
        public void Empty()
        {
            if (IsPresent)
            {
                State = Etat.STATE_EMPTY;
            }
        }
        /// <summary>
        /// Active la recharge des recyclers.
        /// </summary>
        public static void Reloadrecycler()
        {
            try
            {
                isReloadInProgress = true;
                State = Etat.STATE_CASHIN_START;
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message,
                    exception.StackTrace);
            }
        }

        /// <summary>
        /// Lit les compteurs du bnr et les enregistre dans une liste;
        /// </summary>
        public void GetBNRCounters()
        {
            try
            {
                CcounterInfo counterInfo;
                listCountersInfo.Clear();
                foreach (Mei.Bnr.CashUnit.LogicalCashUnit logicalCashUnit in bnr.CashUnit.LogicalCashUnits)
                {
                    if (logicalCashUnit.Count > 0)
                    {
                        counterInfo = new CcounterInfo(logicalCashUnit.PhysicalName, logicalCashUnit.CashType.Value,
                            logicalCashUnit.CashType.CurrencyCode, logicalCashUnit.Count);
                        counterInfo.amount = counterInfo.currencyValue * counterInfo.count;
                        listCountersInfo.Add(counterInfo);
                    }
                }
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message,
                    exception.StackTrace);
            }
        }

        /// <summary>
        /// Initialisation de la classe du BNR.
        /// </summary>
        public override void Init()
        {
            try
            {
                CDevicesManager.Log.Info("Initialisation du BNR");
                Bnr.OperationCompletedEvent += new OperationCompleted(Bnr_OperationCompletedEvent);
                Bnr.CashOccuredEvent += new CashOccured(CashOccured);
                Bnr.IntermediateOccuredEvent += new IntermediateOccured(IntermediateOccured);
                Bnr.StatusOccuredEvent += new StatusOccured(StatusOccured);
                //                IsBNRToBeActivated = false;
                errorInfo = new Cerror();
                isLoaderMetersUpdade = true;
                isReloadInProgress = false;
                isCancelInProgress = false;
                isBillRefused = false;
            }
            catch (Exception exception)
            {
                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
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
          //          CDevicesManager.Log.Debug("BNR TASK {0}", State);
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
                                isCancelInProgress = true;
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
                                listCountersInfo = new List<CcounterInfo>();
                                isCancelInProgress = false;
                                State = Etat.STATE_RESET;
                            }

                            catch (Exception exception)
                            {
                                IsPresent = false;
                                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                                errorInfo.error = Errortype.DLL_BNR_NOT_FREE;
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
                                if (!isLoaderMetersUpdade)
                                {
                                    State = Etat.STATE_IDLE;
                                    errorInfo.error = Errortype.BNRLOADERMETERMISING;
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
                                else
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
                            }
                            catch (Exception exception)
                            {
                                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
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
                            State = Etat.STATE_GETLCUS;
                            break;
                        }

                        case Etat.STATE_GETLCUS:
                        {
                            try
                            {
                                listLCU = new Mei.Bnr.CashUnit.LogicalCashUnit[bnr.CashUnit.LogicalCashUnits.Count];
                                bnr.CashUnit.LogicalCashUnits.CopyTo(listLCU);
                                State = Etat.STATE_GETSTATUS;
                            }
                            catch
                            {
                            }
                            break;
                        }

                        case Etat.STATE_GETSTATUS:
                        {
                            CDevicesManager.Log.Debug(bnr.Status.PositionStatusList[0].ShutterStatus.ToString());
                            CDevicesManager.Log.Debug(bnr.Status.PositionStatusList[1].ShutterStatus.ToString());
                            CDevicesManager.Log.Debug(bnr.Status.ToString);
                            evReady.Set();
                            State = Etat.STATE_SEARCHDIVIDER;
                            break;
                        }

                        case Etat.STATE_IDLE:
                        {
                            if (isBillRefused)
                            {
                                isBillRefused = false;
                                state = Etat.STATE_CAHSIN;
                            }
                            if (isReadyToSearchDivider)
                            {
                                isReadyToSearchDivider = false;
                                State = Etat.STATE_SEARCHDIVIDER;
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

                        case Etat.STATE_DISPENSE:
                        {
                            bnr.Dispense(AmountToDispense, CDevicesManager.mainDevise);
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_SEARCHDIVIDER:
                        {
                            Divider = SearchDivider;
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_DENOMINATE:
                        {
                            //par defaut, le bnr ne peut pas distribuer
                            isDispensable = false;
                            ev.Reset();
                            //Verifie si il peut distribuer, si oui la variable isDispensable sera positionnée à true.
                            bnr.Denominate(AmountToDispense, CDevicesManager.mainDevise);
                            if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS) || !isDispensable)
                            {
                                errorInfo.nameModule = string.Empty;
                                errorInfo.error = Errortype.NOTDISTRIBUABLE;
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

                        case Etat.STATE_ROLLBACK:
                        {
                            CDevicesManager.Log.Debug("Avant la fonction rollback");
                            RollBack();
                            CDevicesManager.Log.Debug("Après la fonction rollback");
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_RETRACT:
                        {
                            Retract();
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_UPDATE_LOADER_METER:
                        {
                            try
                            {
                                foreach (CModulePosition modulePosition in listModulePosition)
                                {
                                    if (modulePosition.moduleName.StartsWith("LO"))
                                    {
                                        ev.Reset();
                                        bnr.SetLoaderCuContent(modulePosition.moduleName, loaderMeter);
                                        ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                                        isLoaderMetersUpdade = true;
                                        State = Etat.STATE_RESET;
                                        break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                            }
                            break;
                        }

                        case Etat.STATE_RELOAD_RECYCLER:
                        {
                            isReloadInProgress = true;
                            State = Etat.STATE_CASHIN_START;
                            break;
                        }

                        case Etat.STATE_CAHSIN:
                        {
                            try
                            {
                                ev.Reset();
                                bnr.CashIn();
                                if (!ev.WaitOne(60000))
                                {
                                    throw new Exception("Cashin impossible.");
                                }
                            }
                            catch (Exception exception)
                            {
                                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                            }
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_CASHIN_START:
                        {
                            try
                            {
                                if (bnr.TransactionStatus.CurrentTransaction != TransactionType.None)
                                {
                                    ev.Reset();
                                    bnr.Cancel();
                                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                                    switch (bnr.TransactionStatus.CurrentTransaction)
                                    {
                                        case TransactionType.Dispense:
                                        {
                                            ev.Reset();
                                            bnr.Retract();
                                            if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                            {
                                                throw new Exception("Opération de retrait du billet impossible.");
                                            }
                                            ev.Reset();
                                            bnr.Reset();
                                            if (!ev.WaitOne(BnrResetTimeOutInMS))
                                            {
                                                throw new Exception("Impossible d'effectuer le reset du BNR");
                                            }
                                            break;
                                        }
                                        case TransactionType.Cashin:
                                        {
                                            ev.Reset();
                                            bnr.CashInEnd();
                                            if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                            {
                                                throw new Exception("Impossible de finir la transaction en cours.");
                                            }
                                            break;
                                        }
                                        default:
                                            break;
                                    }
                                }
                                ev.Reset();
                                bnr.CashInStart();
                                if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                {
                                    throw new Exception("Impossible de lancer CashInStart");
                                }
                                State = Etat.STATE_CAHSIN;
                            }
                            catch (Exception exception)
                            {
                                State = Etat.STATE_IDLE;
                                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                            }
                            break;
                        }

                        case Etat.STATE_CANCEL:
                        {
                            try
                            {
                                bnr.Cancel();
                            }
                            catch
                            {
                            }
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_CASHIN_END:
                        {
                            try
                            {
                                ev.Reset();
                                bnr.CashInEnd();
                                if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                {
                                    throw new Exception("Impossible de clotûrer la transaction.");
                                }
                            }
                            catch
                            {
                            }
                            State = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_EMPTY:
                        {
                            try
                            {
                                if (bnr.TransactionStatus.CurrentTransaction != TransactionType.None)
                                {
                                    ev.Reset();
                                    bnr.Reset();
                                    if (!ev.WaitOne(BnrResetTimeOutInMS))
                                    {
                                        throw new Exception("Impossible d'effectuer le reset du BNR.");
                                    }
                                }
                                foreach (Mei.Bnr.CashUnit.PhysicalCashUnit physicalCashUnit in bnr.CashUnit.PhysicalCashUnits)
                                {
                                    if ((physicalCashUnit.Name.StartsWith("LO") || physicalCashUnit.Name.StartsWith("RE")) && physicalCashUnit.Count > 0)
                                    {
                                        moduleEmptied = new CModuleEmptied(physicalCashUnit.Name);
                                        ev.Reset();
                                        bnr.Empty(physicalCashUnit.Name, false);
                                        if (!ev.WaitOne(180000))
                                        {
                                            throw new Exception("Impossible de vider {0} " + physicalCashUnit.Name);
                                        }
                                        lock (eventListLock)
                                        {
                                            eventsList.Add(new CEvent
                                            {
                                                reason = CEvent.Reason.BNREMPTY,
                                                nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                                data = moduleEmptied,
                                            });
                                        }
                                    }
                                }
                                State = Etat.STATE_IDLE;
                            }
                            catch (Exception exception)
                            {
                                State = Etat.STATE_IDLE;
                                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                            }
                            break;
                        }

                        case Etat.STATE_REPORT:
                        {
                            try
                            {
                                bnr.GenerateAuditReport(ReportFilePathName);
                            }
                            catch (Exception exception)
                            {
                                State = Etat.STATE_IDLE;
                                CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                            }
                            break;
                        }

                        default:
                        {
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }
                catch (Exception exception)
                {
                    if (isDispensable)
                    {
                        ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                        if (!isCashPresent)
                        {
                            errorInfo.nameModule = string.Empty;
                            errorInfo.error = Errortype.NOTDISPENSED;
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
                    CDevicesManager.Log.Error(messagesText.erreur, exception.GetType(), exception.Message, exception.StackTrace);
                }
            }
        }

        #endregion METHODES
    }
}