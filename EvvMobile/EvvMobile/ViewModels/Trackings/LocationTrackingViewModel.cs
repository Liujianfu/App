using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Database.Models;
using EvvMobile.Database.Repositories;
using EvvMobile.Services;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Trackings
{
    public class LocationTrackingViewModel : BaseViewModel
    {
        public LocationTrackingViewModel()
        {
            ///testing
            ClientName = new PersonNameDto
            {
                FirstName = "John",
                LastName = "Doe"
            };
            //////////////////
        }
        public PersonNameDto ClientName { get; set; }
        public TimeSpan Time
        {
            get
            {
                return _timeSpan;
            }

            set
            {
                if (_timeSpan != value)
                {
                    _timeSpan = value;
                    OnPropertyChanged("Time");
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

        public double Distance
        {
            get
            {
                return _distance;
            }

            set
            {
                if (Math.Abs(_distance - value) > 0.001)
                {
                    _distance = value;
                    OnPropertyChanged("Distance");
                }
            }
        }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime
        {
            get; set;
        }

        public int Id { get; set; }

        public int TotalNumberOfLocations { get; set; }
        public string ClientFullName
        {
            get
            {
                if (ClientName != null)
                {
                    return ClientName.FullName;

                }
                else
                {
                    return "";
                }
            }
            set
            {
                var names = value.Split(' ');
                if (names.Length >= 1)
                {
                    if (ClientName == null)
                    {
                        ClientName = new PersonNameDto();
                    }
                    ClientName.FirstName = names[0];
                    if (names.Length > 1)
                    {
                        ClientName.LastName = names[1];
                    }
                }
            }
        }
        #region commands
        Command _syncLocationTrackingCommand;

        public Command SyncLocationTrackingCommand
        {
            get { return _syncLocationTrackingCommand ?? (_syncLocationTrackingCommand = new Command(async () => await ExecuteSyncLocationTrackingCommand(),
                () => !IsBusy&& SystemViewModel.Instance.HasNetworkConnection)); }
        }

        async Task ExecuteSyncLocationTrackingCommand()
        {
            if (IsBusy && SystemViewModel.Instance.HasNetworkConnection)
                return;

            IsBusy = true;
            SyncLocationTrackingCommand.ChangeCanExecute();
            Synchronized = true;
            // Sync Tracking with Rest Service

            await Task.Run(() =>
            {
                var locationResult = _locationOperation.DeleteLocationsAsync(Id).Result;
                var trackingResult = _locationTrackingOperation.DeleteLocationTrackingAsync(Id).Result;
            });

            IsBusy = false;
            SyncLocationTrackingCommand.ChangeCanExecute();
        }

        Command _deleteOfflineLocationTrackingCommand;

        public Command DeleteOfflineLocationTrackingCommand
        {
            get
            {
                return _deleteOfflineLocationTrackingCommand ?? (_deleteOfflineLocationTrackingCommand = new Command(async () => await ExecuteDeleteOfflineLocationTrackingCommand(),
          () => !IsBusy));
            }
        }

        async Task ExecuteDeleteOfflineLocationTrackingCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            DeleteOfflineLocationTrackingCommand.ChangeCanExecute();
            await Task.Run(() =>
            {

                var locationResult = _locationOperation.DeleteLocationsAsync(Id).Result;
                var trackingResult = _locationTrackingOperation.DeleteLocationTrackingAsync(Id).Result;
            });
            IsBusy = false;
            DeleteOfflineLocationTrackingCommand.ChangeCanExecute();
        }

        Command _createLocationTrackingCommand;

        public Command CreateLocationTrackingCommand
        {
            get
            {
                return _createLocationTrackingCommand ?? (_createLocationTrackingCommand = new Command( () =>  ExecuteCreateLocationTrackingCommand(),
                        () => !IsBusy));
            }
        }

         void ExecuteCreateLocationTrackingCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            CreateLocationTrackingCommand.ChangeCanExecute();
            Task.Run(() =>
            {
                var locationTracking = new LocationTracking();
                StartTime = DateTimeOffset.Now;
                locationTracking.StartTime = StartTime;
                TotalNumberOfLocations = 0;
                Time = new TimeSpan();
                Distance = 0.0d;
                //TODO: Save to database or call RESTFul service? prefer local db

                var trackingResult = _locationTrackingOperation.SaveLocationTrackingAsync(locationTracking).Result;

                Id = locationTracking.Id;
            }).Wait();
            IsBusy = false;
            CreateLocationTrackingCommand.ChangeCanExecute();
        }
        Command _endLocationTrackingCommand;

        public Command EndLocationTrackingCommand
        {
            get
            {
                return _endLocationTrackingCommand ?? (_endLocationTrackingCommand = new Command(async () => await ExecuteEndLocationTrackingCommand(),
                        () => !IsBusy));
            }
        }

        async Task ExecuteEndLocationTrackingCommand()
        {
            IsBusy = true;
            EndLocationTrackingCommand.ChangeCanExecute();
            EndTime = DateTimeOffset.Now;
            Time = EndTime.Subtract(StartTime);

            await Task.Run(() =>
            {

                var locationTracking = new LocationTracking
                {
                    Id = Id,
                    StartTime = StartTime,
                    EndTime = EndTime

                };
                if (_totalRunningInsertTasks > 0)
                {
                    Task.Delay(5000).Wait();
                }
                //calculate distance
                var allPositions = _locationOperation.GetLocationsAsync(Id).Result;
                Location firstPostion = null;
                double distanceInMiles = 0.0;
                foreach (var p in allPositions)
                {
                    if (firstPostion == null)
                    {
                        firstPostion = p;
                        continue;
                    }

                    var secondPosition = p;
                    distanceInMiles += RouteService.DistanceInMiles(firstPostion.Latitude, firstPostion.Longitude, secondPosition.Latitude, secondPosition.Longitude);
                    firstPostion = secondPosition;
                }
                locationTracking.Distance =Distance= distanceInMiles;
                locationTracking.TotalNumberOfLocations = TotalNumberOfLocations;

                try
                {

                    if (!SystemViewModel.Instance.HasNetworkConnection)
                    {
                        var trackingResult = _locationTrackingOperation.SaveLocationTrackingAsync(locationTracking).Result;

                    }
                    else
                    {
                        // Send Location Tracking Info to Rest Service
                        // Delete all locations adn location tracking
                        _locationOperation.DeleteLocationsAsync(Id);
                        _locationTrackingOperation.DeleteLocationTrackingAsync(locationTracking);
                    }
                }
                catch (Exception)
                {
                }
            });
            IsBusy = false;
            EndLocationTrackingCommand.ChangeCanExecute();
        }
        Command _insertLocationCommand;

        public Command InsertLocationCommand
        {
            get
            {
                return _insertLocationCommand ?? (_insertLocationCommand = new Command( (location) =>  ExecuteInsertLocationCommand(location)));
            }
        }

         void ExecuteInsertLocationCommand(object location)
        {

            IsBusy = true;
            Interlocked.Increment(ref _totalRunningInsertTasks);
            CreateLocationTrackingCommand.ChangeCanExecute();
            var locationRecord = location as Location;
            if (locationRecord != null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        //TODO: Save to database or call RESTFul service? prefer local db
                        TotalNumberOfLocations = TotalNumberOfLocations + 1;
                        var result = _locationOperation.InsertLocation(locationRecord).Result;
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {
                        Interlocked.Decrement(ref _totalRunningInsertTasks);
                    }
                });    
                            
            }

            IsBusy = false;
            CreateLocationTrackingCommand.ChangeCanExecute();
        }
        #endregion

        public List<Location> GetAllLocations()
        {
            if (!SystemViewModel.Instance.HasNetworkConnection)
            {
                return _locationOperation.GetLocationsAsync(Id).Result;

            }
            else
            {
                //TODO: get locations from RESTFul service
                return _locationOperation.GetLocationsAsync(Id).Result;
            }
            
        }
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            CreateLocationTrackingCommand.ChangeCanExecute();
            EndLocationTrackingCommand.ChangeCanExecute();
            CreateLocationTrackingCommand.ChangeCanExecute();
            DeleteOfflineLocationTrackingCommand.ChangeCanExecute();
            SyncLocationTrackingCommand.ChangeCanExecute();
        }
        private TimeSpan _timeSpan;

        private bool _synchronized;

        private double _distance;

        private long _totalRunningInsertTasks = 0;
        private readonly LocationTrackingRepository _locationTrackingOperation = new LocationTrackingRepository();
        private readonly LocationRepository _locationOperation = new LocationRepository();

    }
}
