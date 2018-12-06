using Mei.Bnr;
using Mei.Bnr.Module;
using System;
using System.Threading;


namespace DeviceLibrary
{
    /// <summary>
    /// Class du bnr
    /// </summary>
    internal class CBnrMei
    {

        public static Bnr bnr = new Bnr();
        private static BnrXfsErrorCode OCresult;
        private static readonly XfsCashOrder cashOrder;


        /// <summary>
        /// 
        /// </summary>
        private static int BnrResetTimeOutInMS = 60000;

        /// <summary>
        /// 
        /// </summary>
        private enum Etat : byte
        {
            INIT,
            OPEN,
            RESET,
            GETMODULE,
            IDLE,
            STOP,

        }

        /// <summary>
        /// 
        /// </summary>
        private Etat state;

        Thread BnrTask;

        /// <summary>
        /// 
        /// </summary>
        private static AutoResetEvent ev;

        /// <summary>
        /// Indique si un bnr est connecté.
        /// </summary>
        public bool isPresent;

        /// <summary>
        /// 
        /// </summary>
        private readonly int result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cashInOrder"></param>
        private static void CashOccured(XfsCashOrder cashInOrder)
        {
            ev.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identificationId"></param>
        /// <param name="operationId"></param>
        /// <param name="result"></param>
        /// <param name="Data"></param>
        private static void IntermediateOccured (int identificationId, OperationId operationId, IntermediateEvents result, object Data)
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

            ev.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <param name="extendedResult"></param>
        /// <param name="Data"></param>
        private static void StatusOccured(StatusChanged status, BnrXfsErrorCode result, BnrXfsErrorCode extendedResult, object Data)
        {
            DeviceStatus resulta;
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
                    Thread.Sleep(1);

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
        /// 
        /// </summary>
        /// <param name="identificationId"></param>
        /// <param name="operationId"></param>
        /// <param name="result"></param>
        /// <param name="extendedResult"></param>
        /// <param name="Data"></param>
        private static void Bnr_OperationCompletedEvent(int identificationId, OperationId operationId, BnrXfsErrorCode result, BnrXfsErrorCode extendedResult, object Data)
        {
            OCresult = result;
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

            ev.Set();
        }

        private void Task()
        {
            while (true)
            {
                switch (state)
                {
                    case Etat.INIT:
                        try
                        {
                            Bnr.OperationCompletedEvent += new OperationCompleted(Bnr_OperationCompletedEvent);
                            Bnr.CashOccuredEvent += new CashOccured(CashOccured);
                            Bnr.IntermediateOccuredEvent += new IntermediateOccured(IntermediateOccured);
                            Bnr.StatusOccuredEvent += new StatusOccured(StatusOccured);
                            isPresent = true;
                            ev = new AutoResetEvent(false);
                            state = Etat.OPEN;
                        }
                        catch (Exception E)
                        {
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                        }
                        break;
                    case Etat.OPEN:
                        try
                        {
                            bnr.Open();
                            state = Etat.RESET;
                        }
                        catch(Exception E)
                        {
                            isPresent = false;
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            state = Etat.STOP;
                        }
                        break;
                    case Etat.RESET:
                        try
                        {
                            if (bnr.Status.DeviceStatus != DeviceStatus.OnLine)
                            {
                                ev.Reset();
                                bnr.Reset();
                            }
                            if(!ev.WaitOne(BnrResetTimeOutInMS))
                            {
                                throw new Exception("Le reset du BNR a échoué!");
                            }
                            state = Etat.GETMODULE;
                        }
                        catch(Exception E)
                        {
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            Thread.Sleep(BnrResetTimeOutInMS * 2);
                        }
                        break;
                    case Etat.GETMODULE:
                    {
                        foreach(Module m in bnr.Modules)
                        {
                            CDevicesManage.Log.Debug(m.ToString() + " "+ m.Status.OperationalStatus);
                            Thread.Sleep(1);
                        }
                        state = Etat.IDLE;
                        break;
                    }
                    case Etat.IDLE:
                        break;
                    case Etat.STOP:
                    {
                        BnrTask.Suspend();
                        break;
                    }
                    default:
                        break;
                }
                Thread.Sleep(500);
            }
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        public CBnrMei()
        {
            isPresent = false;
            try
            {
                state = Etat.INIT;
                BnrTask = new Thread(Task);
                BnrTask.Start();
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }
    }
}
