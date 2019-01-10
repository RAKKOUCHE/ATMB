/// \file CBNR_CPI.cs
/// \brief Fichier contenant la classe CBNR_CPI
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE

using Mei.Bnr;
using Mei.Bnr.Module;

using System;
using System.Threading;
using System.Collections.Generic;

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
        /// Enumération des erreurs du BNR.
        /// </summary>
        public enum ERRORTYPE
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
            /// Un module a été retiré.
            /// </summary>
            BNRMODULEMANQUANT,

            /// <summary>
            /// Un module a été réinseré.
            /// </summary>
            BNRMODULERINSERE,

            /// <summary>
            /// Indique qu'un billet a été refusé.
            /// </summary>
            BILLREFUSED,

            /// <summary>
            /// Les dlls sont occupées.
            /// </summary>
            DLL_NOT_FREE,
        }

        /// <summary>
        /// Class des erreurs du BNR.
        /// </summary>
        public class Cerror
        {
            /// <summary>
            /// Nom du module.
            /// </summary>
            public string nameModule;

            /// <summary>
            /// Code de l'error.
            /// </summary>
            public ERRORTYPE error;
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

#pragma warning disable CS0169 // The field 'CBNR_CPI.cashOrder' is never used

        /// <summary>
        /// Instance de la classe contenant les informations sur le billet en traitenent.
        /// </summary>
        private static readonly XfsCashOrder cashOrder;

#pragma warning restore CS0169 // The field 'CBNR_CPI.cashOrder' is never used

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
        private static Etat State
        {
            get => state;
            set => state = value;
        }

        /// <summary>
        /// Thread utilisé pour le BNR
        /// </summary>
        private readonly Thread bnrTask;

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

        /// <summary>
        /// Levée pour une opération sur un billet.
        /// </summary>
        /// <param name="cashInOrder"></param>
        private static void CashOccured(XfsCashOrder cashInOrder)
        {
            Thread.Sleep(1);
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
            switch(operationId)
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
                    break;

                case OperationId.Empty:
                    break;

                case OperationId.CashInRollBack:
                {
                    Thread.Sleep(1);
                    break;
                }
                case OperationId.CashInEnd:
                    break;

                case OperationId.CashIn:
                {
                    if(result == IntermediateEvents.SubCashin)
                    {
                        denominationInserted.ValeurCent = ((XfsCashOrder)Data).Denomination.Amount;
                        denominationInserted.CVChannel = 0;
                        denominationInserted.CVPath = 0;
                        denominationInserted.TotalAmount += denominationInserted.ValeurCent;

                        lock(eventListLock)
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
                        lock(eventListLock)
                        {
                            errorInfo.nameModule = null;
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
                    break;

                case OperationId.GetDateTime:
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
            switch(status)
            {
                case StatusChanged.MaintenanceStatusChanged:
                    break;

                case StatusChanged.CashAvailable:
                    isCashPresent = true;
                    isCashTaken = false;
                    break;

                case StatusChanged.CashTaken:
                    isCashTaken = true;
                    isCashPresent = false;
                    break;

                case StatusChanged.TransportChanged:
                    break;

                case StatusChanged.DeviceStatusChanged:
                {
                    if(Data != null)
                    {
                        switch((DeviceStatus)Data)
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
                        foreach(CModulePosition modulePosition in listModulePosition)
                        {
                            if(modulePosition.name == ((Mei.Bnr.CashUnit.CashUnit)Data).PhysicalCashUnits[0].Name)
                            {
                                CEvent eventStatus = new CEvent()
                                {
                                    nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                    data = modulePosition.name,

                                };
                                if(!(modulePosition.isPresent = ((Mei.Bnr.CashUnit.CashUnit)Data).PhysicalCashUnits[0].Status != Mei.Bnr.CashUnit.CashUnitStatus.Missing))
                                {
                                    modulePosition.isReinserted = false;
                                    eventStatus.reason = CEvent.Reason.BNRMODULEMANQUANT;
                                }
                                else
                                {
                                    modulePosition.isReinserted = true;
                                    eventStatus.reason = CEvent.Reason.BNRMODULEREINSERE;
                                }
                                lock(eventListLock)
                                {
                                    eventsList.Add(eventStatus);
                                }
                            }
                        }
                    }
                    catch(Exception E)
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
            switch(operationId)
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
                    break;

                case OperationId.Empty:
                    break;

                case OperationId.CashInRollBack:
                {
                    CDevicesManager.ToPay -= ((XfsCashOrder)Data).Denomination.Amount;
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
                    break;

                case OperationId.Denominate:
                {
                    isDispensable = result == BnrXfsErrorCode.Success;
                    break;
                }
                case OperationId.Reset:
                {
                    try
                    {
                        if(result != BnrXfsErrorCode.Success)
                        {
                            HardwareError(DeviceStatus.UserError);
                        }
                        else
                        {
                            foreach(CModulePosition modulePosition in listModulePosition)
                            {
                                if(modulePosition.isReinserted == true)
                                {
                                    modulePosition.isReinserted = false;

                                    CEvent eventStatus = new CEvent()
                                    {
                                        reason = CEvent.Reason.BNRRAZMETER,
                                        nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                        data = modulePosition.name,
                                    };
                                    if(modulePosition.name == "CB")
                                    {
                                        CDevicesManager.Log.Info("Raz des compteurs de la caisse du BNR.");
                                        bnr.ResetCashboxCuContent(true);
                                        lock(eventListLock)
                                        {
                                            eventsList.Add(eventStatus);
                                        }
                                    }
                                    if(modulePosition.name == "LO1")
                                    {
                                        CDevicesManager.Log.Info("RAZ des compteurs du loader du BNR.");
                                        bnr.SetLoaderCuContent("LO1", 0);
                                        lock(eventListLock)
                                        {
                                            eventsList.Add(eventStatus);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception E)
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
            switch(deviceStatus)
            {
                case DeviceStatus.HardwareError:
                {
                    foreach(Module m in bnr.Modules)
                    {
                        if(m.Status.OperationalStatus != OperationalStatus.Operational)
                        {
                            errorInfo.nameModule = m.ToString();
                            if(bnr.SystemStatusOverview.SystemStatus.ErrorCode != SystemErrorCode.NoError)
                            {
                                errorInfo.error = ERRORTYPE.BNR_OUVERT;
                                break;
                            }
                            if(bnr.SystemStatusOverview.SystemStatus.BillTransportStatus != Mei.Bnr.BillTransportStatus.Ok)
                            {
                                errorInfo.error = ERRORTYPE.BOURRAGE;
                                break;
                            }
                            if(bnr.SystemStatusOverview.SystemStatus.CashTypeStatus != Mei.Bnr.CashTypeStatus.Ok)
                            {
                                errorInfo.error = ERRORTYPE.TYPE_BILLET_ERREUR;
                                break;
                            }
                            if(bnr.SystemStatusOverview.SystemStatus.SystemBillStorageStatus != SystemBillStorageStatus.Ok)
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
            lock(eventListLock)
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
            catch(Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Machime d'état du bnr.
        /// </summary>
        public override void Task()
        {
            while(true)
            {
                try
                {
                    switch(State)
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
                                if(bnr.TransactionStatus.CurrentTransaction == TransactionType.Cashin)
                                {
                                    ev.Reset();
                                    bnr.CashInEnd();
                                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                                }
                                if(bnr.TransactionStatus.CurrentTransaction == TransactionType.Dispense)
                                {
                                    ev.Reset();
                                    bnr.Present();
                                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                                }
                                foreach(Mei.Bnr.CashUnit.PhysicalCashUnit physicalCashUnit in bnr.CashUnit.PhysicalCashUnits)
                                {
                                    CModulePosition modulePostion = new CModulePosition(physicalCashUnit.Name,
                                        physicalCashUnit.Status != Mei.Bnr.CashUnit.CashUnitStatus.Missing);
                                    listModulePosition.Add(modulePostion);
                                }
                                State = Etat.STATE_RESET;
                            }
                            catch(Exception E)
                            {
                                IsPresent = false;
                                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                                errorInfo.error = ERRORTYPE.DLL_NOT_FREE;
                                errorInfo.nameModule = string.Empty;
                                lock(eventListLock)
                                {
                                    eventsList.Add(new CEvent
                                    {
                                        reason = CEvent.Reason.BNRERREUR,
                                        nameOfDevice = bnr.SystemConfiguration.BnrType.ToString(),
                                        data = errorInfo
                                    });
                                }
                                evReady.Set();
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
                                if(!ev.WaitOne(BnrResetTimeOutInMS))
                                {
                                    throw new Exception("Le reset du BNR a échoué!");
                                }
                                IsPresent = true;
                                State = Etat.STATE_SETDATETIME;
                            }
                            catch(Exception E)
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
                            foreach(Module m in bnr.Modules)
                            {
                                CDevicesManager.Log.Info(m.ToString() + " : " + m.Status.OperationalStatus);
                            }
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
                            if(IsBNRToBeActivated)
                            {
                                IsBNRToBeActivated = false;
                                State = Etat.STATE_ENABLE;
                            }
                            if(IsBNRToBeDeactivated)
                            {
                                IsBNRToBeDeactivated = false;
                                State = Etat.STATE_DISABLE;
                            }
                            break;
                        }
                        case Etat.STATE_STOP:
                        {
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
                                catch(Exception E)
                                {
                                    CDevicesManager.Log.Error(E.Message);
                                }
                                if(!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                {
                                    throw new Exception("L'opération CashinStart a échouée.");
                                }
                                CDevicesManager.Log.Debug("cASHIN");
                                if(bnr.CashIn(20000000) <= 0)
                                {
                                    throw new Exception("L'opération cashIn a échouée.");
                                }
                            }
                            catch(Exception E)
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
                                ev.Reset();
                                bnr.CashInEnd();
                                CDevicesManager.Log.Debug("Cashinend.");
                                if(!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                {
                                    throw new Exception();
                                }
                            }
                            catch(Exception E)
                            {
                                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
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
                catch(Exception E)
                {
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
            catch(Exception E)
            {
                CDevicesManager.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
            evReady.WaitOne(90000);
        }
    }
}
