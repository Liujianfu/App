using System;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using EvvMobile.Database.Repositories;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.ClockInOuts
{
    public class ClockInOutViewModel : BaseViewModel
    {
        #region Properties

        


        public int Id { get; set; }

        private const string ProviderNumberPropertyName = "ProviderNumber";
        public string ProviderNumber
        {
            get
            {
                return _providerNumber;
            }

            set
            {
                if (_providerNumber != value)
                {
                    SetProperty(ref _providerNumber, value, ProviderNumberPropertyName);
                }
            }
        }
        public string ProviderId { get; set; }

        public string StaffIdentifier { get; set; }

        public string StaffId { get; set; }
        private const string ClientMaNumberPropertyName = "ClientMaNumber";
        public string ClientMaNumber
        {
            get
            {
                return _clientMaNumber;
            }

            set
            {
                if (_clientMaNumber != value)
                {
                    SetProperty(ref _clientMaNumber, value, ClientMaNumberPropertyName);
                }
            }
        }
        private const string ClientFullNamePropertyName = "ClientFullName";
        public string ClientFullName
        {
            get
            {
                return _clientFullName;
            }

            set
            {
                if (_clientFullName != value)
                {
                    SetProperty(ref _clientFullName, value, ClientFullNamePropertyName);
                }
            }
        }
        private const string ClockInOutTimePropertyName = "ClockInOutTime";
        public DateTimeOffset ClockInOutTime
        {
            get
            {
                return _clockInOutTime;
            }

            set
            {
                if (_clockInOutTime != value)
                {
                    SetProperty(ref _clockInOutTime, value, ClockInOutTimePropertyName);
                }
            }
        }

        public string ClientId { get; set; }

        private const string ProcedureCodePropertyName = "ProcedureCode";

        public string ProcedureCode
        {
            get
            {
                return _procedureCode;
            }

            set
            {
                if (_procedureCode != value)
                {
                    SetProperty(ref _procedureCode, value, ProcedureCodePropertyName);
                }
            }
        }
        private const string ServiceNamePropertyName = "ServiceName";
        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
            set
            {
                if (_serviceName != value)
                {
                    SetProperty(ref _serviceName, value, ServiceNamePropertyName);
                }
                
            }
        }
        private const string ClockInOutTypePropertyName = "ClockInOutType";
        public string ClockInOutType
        {

            get
            {
                return _clockInOutType;
            }

            set
            {
                if (_clockInOutType != value)
                {
                    SetProperty(ref _clockInOutType, value, ClockInOutTypePropertyName);
                }
            }
        }
        private const string SynchronizedPropertyName = "Synchronized";
        public bool Synchronized
        {
            get
            {
                return _synchronized;
            }

            set
            {
                if (_synchronized != value)
                {
                    SetProperty(ref _synchronized, value, SynchronizedPropertyName);
                }
            }
        }
        #endregion
        #region Commands
        Command _saveClockinCommand;
        public Command SaveClockinCommand
        {
            get
            {
                return _saveClockinCommand ?? (_saveClockinCommand = new Command(async () => await ExecuteSaveClockinCommand(),()=>!IsBusy));
            }
        }

        async Task ExecuteSaveClockinCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            var callRecord = new ClockInOut()
            {
                ClientMaNumber = _clientMaNumber,
                ClockInOutTime = DateTimeOffset.Now,
                ClockInOutType = ClockInOutType,
                ProcedureCode = _procedureCode,
                ProviderNumber = _providerNumber,
                StaffId = StaffIdentifier
            };
            if (SystemViewModel.Instance.HasNetworkConnection)
            {
                // Save Clock In Out by Restfull Service

            }
            else
            {
                // Save Clock In Out to Local DB.
                await ClockInOutRepository.SaveClockInOutAsync(callRecord);
            }


            MessagingCenter.Send(callRecord, MessagingServiceConstants.SAVE_CLOCKINOUT);
            IsBusy = false;
            await PopAsync();
        }

        Command _syncClockinCommand;
        public Command SyncClockinCommand
        {
            get
            {
                return _syncClockinCommand ?? (_syncClockinCommand = new Command(async () => await ExecuteSyncClockinCommand(),()=>!IsBusy && SystemViewModel.Instance.HasNetworkConnection));
            }
        }

        async Task ExecuteSyncClockinCommand()
        {
            if (IsBusy ||!SystemViewModel.Instance.HasNetworkConnection)
                return;

            IsBusy = true;
            var callRecord = new ClockInOut()
            {
                ClientMaNumber = _clientMaNumber,
                ClockInOutTime = DateTimeOffset.Now,
                ClockInOutType = ClockInOutType,
                ProcedureCode = _procedureCode,
                ProviderNumber = _providerNumber,
                StaffId = StaffIdentifier
            };
            //TODO:
            // Sync Clock In Out by Restfull Service
            Synchronized = true;

            //delete from local database
            DeleteClockInOut();
            IsBusy = false;
            await PopAsync();
        }

        Command _deleteOfflineClockinCommand;
        public Command DeleteOfflineClockinCommand
        {
            get
            {
                return _deleteOfflineClockinCommand ?? (_deleteOfflineClockinCommand = new Command(async () => await ExecuteDeleteOfflineClockinCommand(), () => !IsBusy ));
            }
        }

        async Task ExecuteDeleteOfflineClockinCommand()
        {
            if (IsBusy )
                return;

            IsBusy = true;
            //delete from local database
            DeleteClockInOut();
            IsBusy = false;
            await PopAsync();
        }

        #endregion
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            DeleteOfflineClockinCommand.ChangeCanExecute();
            SyncClockinCommand.ChangeCanExecute();
            SaveClockinCommand.ChangeCanExecute();
        }
        #region Private methods
        private void DeleteClockInOut()
        {
            var result = ClockInOutRepository.DeleteClockInOutAsync(Id).Result;
        }

        private ClockInOutRepository ClockInOutRepository
        {
            get
            {
                if (_clockInDbOperation == null)
                {
                    _clockInDbOperation = new ClockInOutRepository();
                }
                return _clockInDbOperation;
            }
        }
        

        #endregion

        private string _providerNumber;
        private string _clientMaNumber;
        private string _procedureCode;
        private string _serviceName;
        private string _clientFullName;
        private bool _synchronized;
        private string _clockInOutType;
        private DateTimeOffset _clockInOutTime;
        private ClockInOutRepository _clockInDbOperation ;
    }
}
