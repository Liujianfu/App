using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Customizations;
using EvvMobile.Database.Models;
using EvvMobile.Database.Repositories;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Trackings
{
    public class LocationTrackingListViewModel : BaseViewModel
    {
        public LocationTrackingListViewModel()
        {
            _locationTrackings = new ObservableCollection<Grouping<LocationTrackingViewModel, string>>();
        }

        #region properties
        ObservableCollection<Grouping<LocationTrackingViewModel,string>> _locationTrackings;
        private const string LocationTrackingsPropertyName = "LocationTrackings";
        public ObservableCollection<Grouping<LocationTrackingViewModel, string>> LocationTrackings
        {
            get { return _locationTrackings; }
            set { SetProperty(ref _locationTrackings, value, LocationTrackingsPropertyName); }

        }

        #endregion


        #region commands
        Command _loadLocationTrackingsCommand;

        public Command LoadLocationTrackingsCommand
        {
            get { return _loadLocationTrackingsCommand ?? (_loadLocationTrackingsCommand = new Command(async () => await ExecuteLoadLocationTrackingsCommand(),()=>!IsBusy)); }
        }

        async Task ExecuteLoadLocationTrackingsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LoadLocationTrackingsCommand.ChangeCanExecute();

            var locationTrackingResults = await _locationTrackingDbOperation.GetLocationTrackingsAsync();
            //testing data
            locationTrackingResults = new List<LocationTracking>
            {
                new LocationTracking
                {
                    Distance = 20.1,
                    EndTime = DateTimeOffset.Now,
                    Id = 1000000,
                    StartTime = DateTimeOffset.Now.AddHours(-1),
                    TotalNumberOfLocations = 50
                },
                 new LocationTracking
                {
                    Distance = 30.1,
                    EndTime = DateTimeOffset.Now,
                    Id = 2000000,
                    StartTime = DateTimeOffset.Now.AddHours(-1),
                    TotalNumberOfLocations = 150
                }
            };
            //end
            _locationTrackings.Clear();
            var itmes = new List<LocationTrackingViewModel>();
            foreach (var t in locationTrackingResults)
            {
                var vm = new LocationTrackingViewModel
                {
                    Id = t.Id,
                    Distance = t.Distance,
                    StartTime = t.StartTime.ToLocalTime(),
                    EndTime = t.EndTime.ToLocalTime()
                };
                vm.Time = vm.EndTime.Subtract(vm.StartTime);
                vm.Synchronized = false;

                itmes.Add(vm);
            }
            ///testing group items
            var group = new Grouping<LocationTrackingViewModel, string>(itmes, DateTimeOffset.Now.ToString("D"));
            var group2 = new Grouping<LocationTrackingViewModel, string>(itmes, DateTimeOffset.Now.AddDays(1).ToString("D"));
            var group3 = new Grouping<LocationTrackingViewModel, string>(itmes, DateTimeOffset.Now.AddDays(4).ToString("D"));
            LocationTrackings.Add(group);
            LocationTrackings.Add(group2);
            LocationTrackings.Add(group3);
            /////////////////
            IsBusy = false;
            LoadLocationTrackingsCommand.ChangeCanExecute();
        }

        Command _syncAllTrackingsCommand;

        public Command SyncAllTrackingsCommand
        {
            get { return _syncAllTrackingsCommand ?? (_syncAllTrackingsCommand = new Command(async ()=>await ExecuteSyncAllTrackingsCommand(),
            ()=>!IsBusy && SystemViewModel.Instance.HasNetworkConnection)); }
        }

        async Task ExecuteSyncAllTrackingsCommand()
        {
            if (IsBusy ||!SystemViewModel.Instance.HasNetworkConnection)
                return;

            IsBusy = true;
            SyncAllTrackingsCommand.ChangeCanExecute();

            //TODO:
            //sync using RestFul service

            await Task.Run(() =>
            {

                if (LocationTrackings.Count > 0)
                {
                    foreach (var t in LocationTrackings)
                    {
                        foreach (var item in t.ToList())
                        {
                            var locationResult = _locationOperation.DeleteLocationsAsync(item.Id).Result;
                            var trackingResult = _locationTrackingDbOperation.DeleteLocationTrackingAsync(item.Id).Result;                            
                        }

                    }
                    LocationTrackings.Clear();
                }
            });
            IsBusy = false;
            SyncAllTrackingsCommand.ChangeCanExecute();
        }




        #endregion
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            SyncAllTrackingsCommand.ChangeCanExecute();
            LoadLocationTrackingsCommand.ChangeCanExecute();
        }
        #region fields
        private readonly LocationTrackingRepository _locationTrackingDbOperation = new LocationTrackingRepository();
        private LocationRepository _locationOperation = new LocationRepository();


        #endregion
    }
}
