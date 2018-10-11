using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EvvMobile.Customizations;
using EvvMobile.Database.Models;
using EvvMobile.Pages.Base;
using EvvMobile.Services;
using EvvMobile.ViewModels.Systems;
using EvvMobile.ViewModels.Trackings;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace EvvMobile.Pages.Trackings
{
    public partial class TrackLocationPage : TrackLocationPageXaml
    {
        public TrackLocationPage()
        {
            InitializeComponent();
            StartImage.Source = ImageSource.FromResource("EvvMobile.Images.StartTracking.png");
            StartImage.Aspect = Aspect.AspectFit;
            var startImageTapGestureRecognizer = new TapGestureRecognizer();
            startImageTapGestureRecognizer.Tapped += async (s, e) =>
            {
                await Task.Run(() => StartTrackingButtonClicked(s, e));
            };
            StartImage.GestureRecognizers.Add(startImageTapGestureRecognizer);

            EndImage.Aspect = Aspect.AspectFit;
            EndImage.Source = ImageSource.FromResource("EvvMobile.Images.EndTracking.png");
            var endImageTapGestureRecognizer = new TapGestureRecognizer();
            endImageTapGestureRecognizer.Tapped += async (s, e) =>
            {
                await Task.Run(() => EndTrackingButtonClicked(s, e));
            };
            EndImage.GestureRecognizers.Add(endImageTapGestureRecognizer);
            InitializeGeo();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            var locator = CrossGeolocator.Current;
            if (locator.IsListening)
            {
                Task.Run(async () => { var result =  await locator.StopListeningAsync(); });
            }
        }


        private  async void InitializeGeo()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    string address = "Unknown";
                    var locator = CrossGeolocator.Current;
                    CrossGeolocator.Current.AllowsBackgroundUpdates = true;
                    CrossGeolocator.Current.DesiredAccuracy = 50;
                    try
                    {
                        var position = await locator.GetPositionAsync(timeoutMilliseconds: 1000);
                        SystemViewModel.Instance.CurrentLatitude = position.Latitude;
                        SystemViewModel.Instance.CurrentLongitude = position.Longitude;
                        Geocoder geoCoder = new Geocoder();

                        var addressList = await geoCoder.GetAddressesForPositionAsync(new Position(SystemViewModel.Instance.CurrentLatitude,
                            SystemViewModel.Instance.CurrentLongitude));
                        address = addressList.FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        SystemViewModel.Instance.ErrorMessage = "Cannot get current location, will use default value.";
                    }
                    var currentPosition = new Position(SystemViewModel.Instance.CurrentLatitude,
                        SystemViewModel.Instance.CurrentLongitude);


                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = currentPosition,
                        Label = "Current Location",
                        Address = address
                    };

                    TackingMap.Pins.Add(pin);
                    TackingMap.ShowRoute = true;
                    TackingMap.MoveToRegion(MapSpan.FromCenterAndRadius(currentPosition, Distance.FromMiles(5.0)));
                }
                catch (Exception e)
                {
                    SystemViewModel.Instance.ErrorMessage = e.Message;
                }
            });
        }

        private async void StartTrackingButtonClicked(object sender, EventArgs e)
        {
            try
            {
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
                if (ViewModel.CreateLocationTrackingCommand.CanExecute(null))
                {

                    Device.BeginInvokeOnMainThread( () =>
                    {
                        MapStackLayout.Children.Clear();

                        var map = new CustomMap()
                        {
                            IsShowingUser = false,
                            MapType = MapType.Street,
                            ShowRoute = true
                        };

                        var currentPosition = new Position(SystemViewModel.Instance.CurrentLatitude, SystemViewModel.Instance.CurrentLongitude);

                        var pin = new Pin
                        {
                            Type = PinType.Place,
                            Position = currentPosition,
                            Label = "Current Location",
                            Address = address
                        };

                        map.Pins.Add(pin);
                        map.ShowRoute = true;
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(currentPosition, Distance.FromMiles(5.0)));
                        MapStackLayout.Children.Add(map);

                    });
                    


                    ViewModel.CreateLocationTrackingCommand.Execute(null);
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

                            Device.BeginInvokeOnMainThread(() =>
                                {
                                    StartImage.IsVisible = false;
                                    EndImage.IsVisible = true;
                                }
                            );
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

            }
            catch (Exception ex)
            {
                SystemViewModel.Instance.ErrorMessage = "Start Tracking Failed.";
            }
        }

        private void LocatorPositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
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
                    LocationTrackingId = ViewModel.Id,
                    Accuracy = position.Accuracy,
                    Timestamp = position.Timestamp
                };
                ViewModel.InsertLocationCommand.Execute(location);
            }
            catch (Exception)
            {
                // Log the error.
            }
        }

        private async void EndTrackingButtonClicked(object sender, EventArgs e)
        {
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
                SystemViewModel.Instance.ErrorMessage = "Cannot get last location, will use default value.";
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
                        LocationTrackingId = ViewModel.Id,
                        Accuracy = lastPosition.Accuracy,
                        Timestamp = lastPosition.Timestamp
                    };
                    ViewModel.InsertLocationCommand.Execute(location);
                }
                if (locator.IsListening)
                {
                   
                    bool stopped = await locator.StopListeningAsync();
                    if (!stopped)
                    {
                        // Log Error;
                    }
                }
                 ViewModel.EndLocationTrackingCommand.Execute(null);

                //calculate distance
                var allPositions = ViewModel.GetAllLocations();

                Device.BeginInvokeOnMainThread( () =>
                {
                    ShowRoute(allPositions);
                    EndImage.IsVisible = false;
                    StartImage.IsVisible = true;
        
                }
                );
            }
            catch (Exception)
            {
                SystemViewModel.Instance.ErrorMessage = "Ending Tracking Error.";
            }


        }

        private async void ShowRoute(List<Location> allPositions)
        {
            var startPosition = allPositions.FirstOrDefault();
            var endPosition = allPositions.LastOrDefault();
            var satrtAddress = "";
            var endAddress = "";
            try
            {
                Geocoder geoCoder = new Geocoder();

                if (startPosition != null)
                {

                    var addressList = await geoCoder.GetAddressesForPositionAsync(
                        new Position(startPosition.Latitude, startPosition.Longitude));
                    satrtAddress = addressList.FirstOrDefault();
                }
                if (endPosition != null)
                {

                    var addressList = await geoCoder.GetAddressesForPositionAsync(
                        new Position(endPosition.Latitude, endPosition.Longitude));
                    endAddress = addressList.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
            }
            try
            {

                MapStackLayout.Children.Clear();

                var map = new CustomMap()
                {
                    IsShowingUser = false,
                    MapType = MapType.Street,
                    ShowRoute = true
                };
                //add pins , start , end

                if (startPosition != null)
                {
                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = new Position(startPosition.Latitude, startPosition.Longitude),
                        Label = "Start",
                        Address = satrtAddress
                    };
                    map.Pins.Add(pin);
                    
                }
                if (endPosition != null && allPositions.Count > 1)
                {
                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = new Position(endPosition.Latitude, endPosition.Longitude),
                        Label = "End",
                        Address = endAddress
                    };
                    map.Pins.Add(pin);
                }
                //////////////
                foreach (var p in allPositions)
                {
                    map.RouteCoordinates.Add(new Position(p.Latitude, p.Longitude));
                }

                var moveToPosition = RouteService.CenterOfRoute(allPositions);
                if(moveToPosition!= null)
                    map.MoveToRegion(moveToPosition);

                MapStackLayout.Children.Add(map);
            }
            catch
            {
                SystemViewModel.Instance.ErrorMessage = "End Tracking Failed.";
            }
        }
    }

    public abstract class TrackLocationPageXaml : ModelBoundWithHomeButtonContentPage<LocationTrackingViewModel> { }
}
