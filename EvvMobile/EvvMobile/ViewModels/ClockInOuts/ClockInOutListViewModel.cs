using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using EvvMobile.Database.Repositories;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.ClockInOuts
{
    public class ClockInOutListViewModel : BaseViewModel
    {
        public ClockInOutListViewModel()
        {
            _clockInOuts = new ObservableCollection<ClockInOutViewModel>();
        }

        #region properties
        ObservableCollection<ClockInOutViewModel> _clockInOuts;
        private const string ClockInOutsPropertyName = "ClockInOuts";
        public ObservableCollection<ClockInOutViewModel> ClockInOuts
        {
            get { return _clockInOuts; }
            set
            {
                SetProperty(ref _clockInOuts, value, ClockInOutsPropertyName);
            }
        }

        private ClockInOutSearchViewModel _clockInOutSearchViewModel;
        private const string ClockInOutSearchViewModelPropertyName = "ClockInOutSearchViewModel";
        public ClockInOutSearchViewModel ClockInOutSearchViewModel
        {
            get { return _clockInOutSearchViewModel; }
            set
            {
                 SetProperty(ref _clockInOutSearchViewModel, value, ClockInOutSearchViewModelPropertyName); 
                 OnPropertyChanged("EnableSyncButton");
            }
        }

        public bool EnableSyncButton
        {
            get { return !IsBusy && SystemViewModel.Instance.HasNetworkConnection && ClockInOuts != null && ClockInOuts.Count > 0; }
        }
        #endregion


        #region Commands
        Command _loadCLockInOutsCommand;

        public Command LoadCLockInOutsCommand
        {
            get { return _loadCLockInOutsCommand ?? (_loadCLockInOutsCommand = new Command(async () => await ExecuteLoadCLockInOutsCommand(), () => !IsBusy&& ClockInOutSearchViewModel != null)); }
        }

        async Task ExecuteLoadCLockInOutsCommand()
        {
            if (IsBusy || ClockInOutSearchViewModel == null)
                return;
            _clockInOuts.Clear();
            IsBusy = true;

            //testing data , should get data from restful service when it is online
            //  if (SystemViewModel.Instance.HasNetworkConnection)
            // {
            // by Restfull Service

            //   }
            //  else
            {
                //waiting indicator test
                await Task.Run(async () =>
                {
                    Task.Delay(10000).Wait();
                });
                //////////////////
                var clockInOutResults = new ObservableCollection<ClockInOut>((await _clockInDbOperation.GetClockInOutsAsync()));

                clockInOutResults = new ObservableCollection<ClockInOut>
                {
                    new ClockInOut
                    {
                        ClientFullName = "Test1",
                        ClientMaNumber = "1234567788",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockIn",
                        ProcedureCode = "U:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                    new ClockInOut
                    {
                        ClientFullName = "Test2",
                        ClientMaNumber = "123456222",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockOut",
                        ProcedureCode = "H:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                                            new ClockInOut
                    {
                        ClientFullName = "Test1",
                        ClientMaNumber = "1234567788",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockIn",
                        ProcedureCode = "U:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                    new ClockInOut
                    {
                        ClientFullName = "Test2",
                        ClientMaNumber = "123456222",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockOut",
                        ProcedureCode = "H:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                                            new ClockInOut
                    {
                        ClientFullName = "Test1",
                        ClientMaNumber = "1234567788",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockIn",
                        ProcedureCode = "U:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                    new ClockInOut
                    {
                        ClientFullName = "Test2",
                        ClientMaNumber = "123456222",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockOut",
                        ProcedureCode = "H:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                                            new ClockInOut
                    {
                        ClientFullName = "Test1",
                        ClientMaNumber = "1234567788",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockIn",
                        ProcedureCode = "U:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                    new ClockInOut
                    {
                        ClientFullName = "Test2",
                        ClientMaNumber = "123456222",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockOut",
                        ProcedureCode = "H:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                                            new ClockInOut
                    {
                        ClientFullName = "Test1",
                        ClientMaNumber = "1234567788",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockIn",
                        ProcedureCode = "U:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                    new ClockInOut
                    {
                        ClientFullName = "Test2",
                        ClientMaNumber = "123456222",
                        ClockInOutTime = DateTimeOffset.Now,
                        ClockInOutType = "ClockOut",
                        ProcedureCode = "H:ABC123",
                        ServiceName = "Service A",
                        ProviderNumber = "111111111"
                    },
                };
                foreach (var clockInOut in clockInOutResults)
                {
                    ClockInOuts.Add(new ClockInOutViewModel
                    {
                        ClientFullName = clockInOut.ClientFullName,
                        ClientMaNumber = clockInOut.ClientMaNumber,
                        ClockInOutTime = clockInOut.ClockInOutTime,
                        ClockInOutType = clockInOut.ClockInOutType,
                        ProcedureCode = clockInOut.ProcedureCode,
                        ServiceName = clockInOut.ServiceName,
                        ProviderNumber = clockInOut.ProviderNumber
                    });
                }

                

            }

            IsBusy = false;

        }

        Command _loadOfflineCLockInOutsCommand;

        public Command LoadOfflineCLockInOutsCommand
        {
            get { return _loadOfflineCLockInOutsCommand ?? (_loadOfflineCLockInOutsCommand = new Command( () =>  ExecuteLoadOfflineCLockInOutsCommand(),
                             () => !IsBusy)); }
        }

        async void ExecuteLoadOfflineCLockInOutsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            //test
            await Task.Run(() =>
            {
                Task.Delay(10000).Wait();
            });

            var clockInOutResults = new ObservableCollection<ClockInOut>((await _clockInDbOperation.GetClockInOutsAsync()));
            ClockInOuts.Clear();
            //testing data
            clockInOutResults = new ObservableCollection<ClockInOut>
            {
                new ClockInOut
                {
                    ClientFullName = "Test1",
                    ClientMaNumber = "1234567788",
                    ClockInOutTime = DateTimeOffset.Now,
                    ClockInOutType = "ClockIn",
                    ProcedureCode = "U:ABC123",
                    ServiceName = "Service A",
                    ProviderNumber = "111111111"
                },
                new ClockInOut
                {
                    ClientFullName = "Test2",
                    ClientMaNumber = "123456222",
                    ClockInOutTime = DateTimeOffset.Now,
                    ClockInOutType = "ClockOut",
                    ProcedureCode = "H:ABC123",
                    ServiceName = "Service A",
                    ProviderNumber = "111111111"
                },
            };
            foreach (var clockInOut in clockInOutResults)
            {
                ClockInOuts.Add(new ClockInOutViewModel
                {
                    ClientFullName = clockInOut.ClientFullName,
                    ClientMaNumber = clockInOut.ClientMaNumber,
                    ClockInOutTime = clockInOut.ClockInOutTime,
                    ClockInOutType = clockInOut.ClockInOutType,
                    ProcedureCode = clockInOut.ProcedureCode,
                    ServiceName = clockInOut.ServiceName,
                    ProviderNumber = clockInOut.ProviderNumber
                });
            }
            IsBusy = false;
            if (ClockInOuts.Count <= 0)
            {
                SystemViewModel.Instance.ErrorMessage = "No Records Found!";
            }
        }
        
        Command _syncAllOfflineCLockInOutsCommand;

        public Command SyncAllOfflineCLockInOutsCommand
        {
            get { return _syncAllOfflineCLockInOutsCommand ?? (_syncAllOfflineCLockInOutsCommand = new Command(async (param) => await ExecuteSyncAllOfflineCLockInOutsCommand(param),
                             canExecute:  x=> !IsBusy&& SystemViewModel.Instance.HasNetworkConnection&& ClockInOuts != null && ClockInOuts.Count > 0)); }
        }

        async Task ExecuteSyncAllOfflineCLockInOutsCommand(object parameter)
        {
            try
            {
                if (IsBusy || !SystemViewModel.Instance.HasNetworkConnection)
                    return;
                SystemViewModel.Instance.CleanMessages();

                IsBusy = true;
                // Sync Tracking with Rest Service

                await Task.Run(() =>
                {
                    Task.Delay(1000).Wait();
                    //return;
                    if (ClockInOuts != null && ClockInOuts.Count > 0)
                    {
                        foreach (var c in ClockInOuts)
                        {
                            var result = _clockInDbOperation.DeleteClockInOutAsync(c.Id).Result;
                        }
                    }
                    ClockInOuts = null;
                });
                IsBusy = false;

            }
            catch (Exception)
            {
                //
                SystemViewModel.Instance.InfoMessage = "Sync All Failed!";
            }
        }
        #endregion

        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            SyncAllOfflineCLockInOutsCommand.ChangeCanExecute();
            LoadOfflineCLockInOutsCommand.ChangeCanExecute();
            LoadCLockInOutsCommand.ChangeCanExecute();
        }
        #region fields
        private readonly ClockInOutRepository _clockInDbOperation = new ClockInOutRepository();

        

        #endregion
    }

}
