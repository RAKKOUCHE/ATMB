using Mei.Bnr;
using Mei.Bnr.Module;
using System;
using System.Threading;


namespace DeviceLibrary
{
    /// <summary>
    /// Class du bnr
    /// </summary>
    internal class CBnrMei : IDisposable
    {

        /// <summary>
        /// Instanciation d'un BNR
        /// </summary>
        public static Bnr bnr = new Bnr();

        /// <summary>
        /// Code Erreur XFS
        /// </summary>
        private static BnrXfsErrorCode oCresult;

        /// <summary>
        /// 
        /// </summary>
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
            GETSTATUS,
            IDLE,
            STOP,
        }

        /// <summary>
        /// Etat de la machine d'état du BNR.
        /// </summary>
        private Etat state;

        /// <summary>
        /// Thread utilisé pour le BNR
        /// </summary>
        Thread bnrTask;

        /// <summary>
        /// Evénement permettant de gérer les function asynchrone.
        /// </summary>
        private static AutoResetEvent ev;

        /// <summary>
        /// Indique si un bnr est connecté.
        /// </summary>
        public bool isPresent;

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
        /// 
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
                            case DeviceStatus.HardwareError:
                            {
                                break;
                            }
                            case DeviceStatus.OffLine:
                            {
                                break;
                            }
                            case DeviceStatus.OnLine:
                            {
                                break;
                            }
                            case DeviceStatus.UserError:
                            {
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
        /// 
        /// </summary>
        /// <param name="identificationId"></param>
        /// <param name="operationId"></param>
        /// <param name="result"></param>
        /// <param name="extendedResult"></param>
        /// <param name="Data"></param>
        private static void Bnr_OperationCompletedEvent(int identificationId, OperationId operationId, BnrXfsErrorCode result, BnrXfsErrorCode extendedResult, object Data)
        {
            oCresult = result;
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
                    {
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
                    }
                    case Etat.OPEN:
                    {
                        try
                        {
                            bnr.Open();
                            state = Etat.RESET;
                        }
                        catch (Exception E)
                        {
                            isPresent = false;
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            state = Etat.STOP;
                        }
                        break;
                    }
                    case Etat.RESET:
                    {
                        try
                        {
                            if (bnr.Status.DeviceStatus != DeviceStatus.OnLine)
                            {
                                ev.Reset();
                                bnr.Reset();
                            }
                            if (!ev.WaitOne(BnrResetTimeOutInMS))
                            {
                                throw new Exception("Le reset du BNR a échoué!");
                            }
                            state = Etat.GETMODULE;
                        }
                        catch (Exception E)
                        {
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                            Thread.Sleep(BnrResetTimeOutInMS * 2);
                        }
                        break;
                    }
                    case Etat.GETMODULE:
                    {
                        try
                        {
                            foreach (Module m in bnr.Modules)
                            {
                                CDevicesManage.Log.Debug(m.ToString() + " " + m.Status.OperationalStatus);
                                Thread.Sleep(1);
                            }
                        }
                        catch (Exception E)
                        {
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                        }
                        state = Etat.GETSTATUS;
                        break;
                    }
                    case Etat.GETSTATUS:
                    {
                        try
                        {

                            CDevicesManage.Log.Debug(bnr.Status.PositionStatusList[0].ShutterStatus.ToString());
                            CDevicesManage.Log.Debug(bnr.Status.PositionStatusList[1].ShutterStatus.ToString());
                            CDevicesManage.Log.Debug(bnr.Status.ToString);
                        }
                        catch (Exception E)
                        {
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                        }
                        state = Etat.IDLE;
                        break;
                    }

                    case Etat.IDLE:
                    {


                        break;
                    }
                    case Etat.STOP:
                    {
                        try
                        {
                            bnrTask.Abort();
                        }
                        catch (Exception E)
                        {
                            CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
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
                bnrTask = new Thread(Task);
                bnrTask.Start();
            }
            catch (Exception E)
            {
                CDevicesManage.Log.Error(messagesText.erreur, E.GetType(), E.Message, E.StackTrace);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CBnrMei() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
