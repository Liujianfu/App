using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Customizations.CustomControls;
using EvvMobile.Database.Repositories;
using EvvMobile.Localization;
using EvvMobile.Pages.ClockInOuts;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Base;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.ClockInOuts
{
    public class ClockInOutSearchViewModel : BaseViewModel
    {
        public ClockInOutSearchViewModel()
        {
            _clockInOutTypes = new Dictionary<int, string>();
            ClockInOutTypes.Add(0, "Clock In");
            ClockInOutTypes.Add(1, "Clock Out");
        }
        #region Properties

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

        private DateTimeOffset _startTime;
        private const string StartTimePropertyName = "StartTime";
        public DateTimeOffset StartTime
        {
            get
            {
                return _startTime;
            }

            set
            {
                if (_startTime != value)
                {
                    SetProperty(ref _startTime, value, StartTimePropertyName);
                }
            }
        }
        private DateTimeOffset _endTime;
        private const string EndTimePropertyName = "EndTime";
        public DateTimeOffset EndTime
        {
            get
            {
                return _endTime;
            }

            set
            {
                if (_endTime != value)
                {
                    SetProperty(ref _endTime, value, EndTimePropertyName);
                }
            }
        }
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

        private Dictionary<int, string> _clockInOutTypes;
        public Dictionary<int, string> ClockInOutTypes
        {
            get { return _clockInOutTypes; }
            set
            {
                _clockInOutTypes = value;
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

        private int _selectedTypeIndex;
        private const string TypeSelectedPropertyName = "TypeSelected";
        public int TypeSelected
        {
            get { return _selectedTypeIndex; }
            set
            {
                if (value == _selectedTypeIndex) return;
                SetProperty(ref _selectedTypeIndex,value, TypeSelectedPropertyName);
                if (_selectedTypeIndex == 0)
                {
                    ClockInOutType =  RecordTypes.ClockIn;
                }
                else
                {
                    ClockInOutType = RecordTypes.ClockOut;
                }
            }
        }
        #endregion
        #region Commands
        Command _initSearchModelCommand;
        public Command InitSearchModelCommand
        {
            get { return _initSearchModelCommand ?? (_initSearchModelCommand = new Command(() => ExecuteInitSearchModelCommand(), () => !IsBusy && !IsInitialized)); }
        }

        async void ExecuteInitSearchModelCommand()
        {
            if (IsBusy || IsInitialized)
                return;

            IsBusy = true;
            await Task.Run(() =>
            {
                _clockInOutType =  RecordTypes.ClockIn;
                //StaffPhoneNumber = "12345678"; //get from device
                StaffIdentifier = "11111111"; //get it from restful service
                StaffId = "01110000";
                //TODO: need to get staff id , provider number from DB or Back end service?
                // should get data from restful service when it is online
                //  if (SystemViewModel.Instance.HasNetworkConnection)
                // {


                // }

                /////////////////////////
            });
            IsBusy = false;
            IsInitialized = true;
        }
        Command _searchCommand;
        public Command SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new Command(() => ExecuteSearchCommand(), () => !IsBusy && IsInitialized)); }
        }

        async void ExecuteSearchCommand()
        {
            if (IsBusy || !IsInitialized)
                return;

            IsBusy = true;

            var clockInOutListModel = new ClockInOutListViewModel
            {
                ClockInOutSearchViewModel = this
            };

            if (clockInOutListModel.LoadCLockInOutsCommand.CanExecute(null))
            {

                clockInOutListModel.LoadCLockInOutsCommand.Execute(null);


                await Navigation.PushAsync(new ClockInOutListPage { Title = TextResources.ClockInOut_List, BindingContext = clockInOutListModel });
            }

            IsBusy = false;

        }
        #endregion

        #region Private methods
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
             InitSearchModelCommand.ChangeCanExecute();
             SearchCommand.ChangeCanExecute();
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
        private string _clockInOutType;
        private ClockInOutRepository _clockInDbOperation ;
    }
}
