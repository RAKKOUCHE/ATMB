/// \file CBNR_CPI.cs
/// \brief Fichier contenant la classe CBNR_CPI
/// \date 28 11 2018
/// \version 1.0.0
/// \author Rachid AKKOUCHE 

using Mei.Bnr;
using Mei.Bnr.Module;
using System;
using System.Threading;

namespace DeviceLibrary
{

    /// <summary>
    /// Enumération des erreurs du BNR.
    /// </summary>
    public enum ERROR_BNR
    {
        /// <summary>
        /// Billet bloqué dans le BNR.
        /// </summary>
        BOURRAGE,
        /// <summary>
        /// Le bnr est ouvert ou déverrouillé.
        /// </summary>
        BNR_OPEN,
        /// <summary>
        /// Indique une erreurs sur le paramètrage d'un billet.
        /// </summary>
        TYPE_BILLET_ERREUR,
        /// <summary>
        /// Erreur de stockage des billets.
        /// </summary>
        STOCKAGE_ERREUR,
        /// <summary>
        /// Les dlls sont occupées.
        /// </summary>
        DLL_NOT_FREE,
    }

    /// <summary>
    /// Class des erreurs du BNR.
    /// </summary>
    public class CerrorBNR
    {
        /// <summary>
        /// Nom du module.
        /// </summary>
        public string nameModule;
        /// <summary>
        /// Code de l'error.
        /// </summary>
        public ERROR_BNR error;
    }

    /// <summary>
    /// Class du bnr
    /// </summary>
    public partial class CBNR_CPI : CDevice
    {
        /// <summary>
        /// Instance du bnr.
        /// </summary>
        private static Bnr bnr;

        /// <summary>
        /// Contient les informations concernant un évenement.
        /// </summary>
        private static CEvent BNREvent;

        /// <summary>
        /// Code produit du périphérique.
        /// </summary>
        public override string ProductCode
        {
            get
            {
                string result = "";
                try
                {

                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Retourne l'identifiant du fabricant.
        /// </summary>
        public override string Manufacturer
        {
            get
            {
                string result = "";
                try
                {

                }
                catch (Exception E)
                {
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
                return result;
            }
        }

        /// <summary>
        /// Instance de la classe contenant les informations sur le billet en traitenent.
        /// </summary>
        private static XfsCashOrder cashOrder;

        /// <summary>
        /// Délai maximum accordé pour effectur un reset
        /// </summary>
        private const int BnrResetTimeOutInMS = 60000;

        /// <summary>
        /// Delain maximum pour effectuer une opération.
        /// </summary>
        private const int BnrDefaultOperationTimeOutInMS = 10000;

        /// <summary>
        /// Etat de la machine d'état du BNR.
        /// </summary>
        private static Etat state;

        /// <summary>
        /// Thread utilisé pour le BNR
        /// </summary>
        private Thread bnrTask;

        /// <summary>
        /// Evénement permettant de gérer les function asynchrone.
        /// </summary>
        private static AutoResetEvent ev;

        /// <summary>
        /// Indique si un bnr est connecté.
        /// </summary>
        public static bool isPresent;

        /// <summary>
        /// Instance de la class CerrorBNR.
        /// </summary>
        private static CerrorBNR errorInfo;

        private bool isBNRToBeDeactivated;
        /// <summary>
        /// Indique si le monnayeur doit être adtivé.
        /// </summary>
        public bool IsBNRToBeDeactivated
        {
            get => isBNRToBeDeactivated;
            set => isBNRToBeDeactivated = value;
        }

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
            ev.Set();
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
                    break;
                case OperationId.Empty:
                    break;
                case OperationId.CashInRollBack:
                    break;
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
                    break;
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
            switch (status)
            {
                case StatusChanged.MaintenanceStatusChanged:
                    break;
                case StatusChanged.CashAvailable:
                    break;
                case StatusChanged.CashTaken:
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
                                state = Etat.STATE_RESET;
                                break;
                            }
                            case DeviceStatus.OnLine:
                            {
                                break;
                            }
                            case DeviceStatus.HardwareError:
                            case DeviceStatus.UserError:
                            {
                                HardwareError();
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
                    break;
                case StatusChanged.CashUnitChanged:
                    break;
                default:
                    break;
            }
            ev.Set();
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
                    break;
                case OperationId.Empty:
                    break;
                case OperationId.CashInRollBack:
                    break;
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
                    break;
                case OperationId.Reset:
                {
                    if (result != BnrXfsErrorCode.Success)
                    {
                        HardwareError();
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
        private static void HardwareError()
        {
            BNREvent.reason = Reason.BNRERREUR;
            BNREvent.deviceId = bnr.SystemConfiguration.BnrType.ToString();
            foreach (Module m in bnr.Modules)
            {
                if (m.Status.OperationalStatus != OperationalStatus.Operational)
                {
                    errorInfo.nameModule = m.ToString();
                    if (bnr.SystemStatusOverview.SystemStatus.BillTransportStatus != Mei.Bnr.BillTransportStatus.Ok)
                    {
                        errorInfo.error = ERROR_BNR.BOURRAGE;
                    }
                    else
                    {
                        if (bnr.SystemStatusOverview.SystemStatus.ErrorCode != SystemErrorCode.NoError)
                        {
                            errorInfo.error = ERROR_BNR.BNR_OPEN;
                        }
                        else
                        {
                            if (bnr.SystemStatusOverview.SystemStatus.CashTypeStatus != Mei.Bnr.CashTypeStatus.Ok)
                            {
                                errorInfo.error = ERROR_BNR.TYPE_BILLET_ERREUR;
                            }
                            else
                            {
                                if (bnr.SystemStatusOverview.SystemStatus.SystemBillStorageStatus != SystemBillStorageStatus.Ok)
                                {
                                    errorInfo.error = ERROR_BNR.STOCKAGE_ERREUR;
                                }
                            }
                        }
                    }
                    break;
                }
            }
            BNREvent.data = errorInfo;
            isPresent = false;
            lock (eventListLock)
            {
                eventsList.Add(BNREvent);
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
                errorInfo = new CerrorBNR();
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        /// <summary>
        /// Machime d'état du bnr.
        /// </summary>
        protected override void Task()
        {
            while (true)
            {
                try
                {
                    switch (state)
                    {
                        case Etat.STATE_INIT:
                        {
                            Init();

                            state = Etat.STATE_OPEN;
                            break;
                        }
                        case Etat.STATE_OPEN:
                        {
                            try
                            {
                                bnr.Open();
                                try
                                {
                                    bnr.Cancel();
                                    ev.Reset();
                                    bnr.CashInEnd();
                                    ev.WaitOne(BnrDefaultOperationTimeOutInMS);
                                }
                                catch
                                {

                                }
                                state = Etat.STATE_RESET;
                            }
                            catch (Exception E)
                            {
                                isPresent = false;
                                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                                BNREvent.reason = Reason.BNRERREUR;
                                BNREvent.deviceId = "BNR";
                                errorInfo.error = ERROR_BNR.DLL_NOT_FREE;
                                errorInfo.nameModule = string.Empty;
                                BNREvent.data = errorInfo;
                                eventsList.Add(BNREvent);
                                state = Etat.STATE_STOP;
                            }
                            break;
                        }
                        case Etat.STATE_RESET:
                        {
                            try
                            {
                                if (bnr.Status.DeviceStatus != DeviceStatus.OnLine)
                                {
                                    ev.Reset();
                                    bnr.Reset();
                                    if (!ev.WaitOne(BnrResetTimeOutInMS))
                                    {
                                        throw new Exception("Le reset du BNR a échoué!");
                                    }
                                }
                                isPresent = true;
                                state = Etat.STATE_SETDATETIME;
                            }
                            catch (Exception E)
                            {
                                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                                Thread.Sleep(BnrResetTimeOutInMS * 2);
                            }
                            break;
                        }
                        case Etat.STATE_SETDATETIME:
                        {
                            //NECESSAIRE POUR LES BNR D'ANCIENNE GENERATION.
                            bnr.DateTime = DateTime.Now;
                            state = Etat.STATE_GETMODULE;
                            break;
                        }
                        case Etat.STATE_GETMODULE:
                        {
                            foreach (Module m in bnr.Modules)
                            {
                                CDevicesManage.Log.Debug(m.ToString() + " " + m.Status.OperationalStatus);
                            }
                            state = Etat.STATE_GETSTATUS;
                            break;
                        }
                        case Etat.STATE_GETSTATUS:
                        {

                            CDevicesManage.Log.Debug(bnr.Status.PositionStatusList[0].ShutterStatus.ToString());
                            CDevicesManage.Log.Debug(bnr.Status.PositionStatusList[1].ShutterStatus.ToString());
                            CDevicesManage.Log.Debug(bnr.Status.ToString);
                            state = Etat.STATE_IDLE;
                            break;
                        }

                        case Etat.STATE_IDLE:
                        {
                            if (IsBNRToBeActivated)
                            {
                                IsBNRToBeActivated = false;
                                state = Etat.STATE_ENABLE;
                            }
                            if (IsBNRToBeDeactivated)
                            {
                                IsBNRToBeDeactivated = false;
                                state = Etat.STATE_DISABLE;
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
                                bnr.CashInStart();
                                if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                {
                                    throw new Exception("L'opération CashinStart a échouée.");
                                }
                                if(bnr.CashIn(65000) <= 0)
                                {
                                    throw new Exception("L'opération cashIn a échouée.");
                                }
                            }
                            catch (Exception E)
                            {
                                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            }
                            state = Etat.STATE_IDLE;
                            break;
                        }
                        case Etat.STATE_DISABLE:
                        {
                            try
                            {
                                CDevicesManage.Log.Debug("cancel.");
                                bnr.Cancel();
                                ev.Reset();
                                bnr.CashInEnd();
                                CDevicesManage.Log.Debug("Cashinend.");
                                if (!ev.WaitOne(BnrDefaultOperationTimeOutInMS))
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception E)
                            {
                                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            }
                            state = Etat.STATE_IDLE;
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
                    CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                }
            }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public CBNR_CPI()
        {
            isPresent = false;
            try
            {
                bnr = new Bnr();
                state = Etat.STATE_INIT;
                ev = new AutoResetEvent(false);
                BNREvent = new CEvent();
                bnrTask = new Thread(Task);
                bnrTask.Start();
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }
    }
}