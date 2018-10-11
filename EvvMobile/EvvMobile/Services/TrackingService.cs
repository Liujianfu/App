using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.ViewModels.Trackings;
using Plugin.Geolocator;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using EvvMobile.Database.Models;
namespace EvvMobile.Services
{
    public static class TrackingService
    {
        static private bool _isTracking;
        static private string _serviceVisitId;
        static private LocationTrackingViewModel TrackModel = new LocationTrackingViewModel();
        public static bool IsTracking {
            get
            {
                return _isTracking;
            }
        }
        public static string ServiceVisitId { get
            { return _serviceVisitId; }
        }
        public static async Task<bool> StartTracking(string serviceVisitId)
        {
            try
            {
                lock (locker)
                {
                    if (IsTracking||string.IsNullOrWhiteSpace(serviceVisitId))
                        return false;
                    _serviceVisitId = serviceVisitId;
                    _isTracking = true;
                    TrackModel = new LocationTrackingViewModel();
                }

                string address = "Unknown";
                var locator = CrossGeolocator.Current;
                try
                {
                    var position = await locator.GetPositionAsync(timeoutMilliseconds: 1000);
                    SystemViewModel.Instance.CurrentLatitude = position.Latitude;
                    SystemViewModel.Instance.CurrentLongitude = position.Longitude;
                    Geocoder geoCoder = new Geocoder();

                    var addressList = await geoCoder.GetAddressesForPositionAsync(new Position(SystemViewModel.Instance.CurrentLatitude, SystemViewModel.Instance.CurrentLongitude));
                    address = addressList.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    SystemViewModel.Instance.ErrorMessage = "Cannot get current location, will use default value.";
                }
                if (TrackModel.CreateLocationTrackingCommand.CanExecute(null))
                {


                    TrackModel.CreateLocationTrackingCommand.Execute(null);
                    locator.PositionChanged += LocatorPositionChanged;
                    if (!locator.IsListening)
                    {
                        try
                        {
                            if (!locator.IsListening)
                            {
                                // //minTime in Milliseconds, minDistance in Memters 
                                bool listened = await locator.StartListeningAsync(minTime: 1000, minDistance: 20,
                                    includeHeading: true);

                                if (!listened)
                                {
                                    // Show Alert Message
                                    SystemViewModel.Instance.ErrorMessage = "Starting Tracking Failed.";
                                }
                            }
                        }
                        catch
                        {
                            // Log Error
                        }
                    }
                }
                else
                {
                    SystemViewModel.Instance.ErrorMessage = "Tracking System is Running,Please Wait.";
                }
                TrackModel.CreateLocationTrackingCommand.Execute(null);
               
            }
            catch(Exception ex)
            {
                _isTracking = false;

                return false;
            }
            return true;

        }
        private static void LocatorPositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position == null)
                {
                    return;
                }

                if (e.Position.Accuracy > 50)
                {
                    return;
                }

                var location = new Location()
                {
                    Latitude = position.Latitude,
                    Longitude = position.Longitude,
                    LocationTrackingId = TrackModel.Id,
                    Accuracy = position.Accuracy,
                    Timestamp = position.Timestamp
                };
                TrackModel.InsertLocationCommand.Execute(location);
            }
            catch (Exception)
            {
                // Log the error.
            }
        }
        public static async Task<int> EndTracking(string serviceVisitId)
        {
            lock (locker)
            {
                if (!IsTracking || serviceVisitId != ServiceVisitId||string.IsNullOrWhiteSpace(serviceVisitId))
                    return -1;
                _serviceVisitId = "";
            }
            var locator = CrossGeolocator.Current;
            Plugin.Geolocator.Abstractions.Position lastPosition = null;
            try
            {
                lastPosition = await locator.GetPositionAsync(timeoutMilliseconds: 1000);
                SystemViewModel.Instance.CurrentLatitude = lastPosition.Latitude;
                SystemViewModel.Instance.CurrentLongitude = lastPosition.Longitude;
            }
            catch (Exception ex)
            {
                
            }
            try
            {
                locator.PositionChanged -= LocatorPositionChanged;
                if (lastPosition != null)
                {
                    var location = new Location()
                    {
                        Latitude = lastPosition.Latitude,
                        Longitude = lastPosition.Longitude,
                        LocationTrackingId = TrackModel.Id,
                        Accuracy = lastPosition.Accuracy,
                        Timestamp = lastPosition.Timestamp
                    };
                    TrackModel.InsertLocationCommand.Execute(location);
                }
                if (locator.IsListening)
                {

                    bool stopped = await locator.StopListeningAsync();
                    if (!stopped)
                    {
                        // Log Error;
                    }
                }
                TrackModel.EndLocationTrackingCommand.Execute(null);

            }
            catch (Exception)
            {
                SystemViewModel.Instance.ErrorMessage = "Ending Tracking Failed.";
            }
            lock (locker)
            {
                _isTracking = false;
                return TrackModel.Id;
            }
            
        }

        private static object locker = new object();
    }
}
