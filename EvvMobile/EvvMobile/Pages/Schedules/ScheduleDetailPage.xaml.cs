using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.Customizations;
using EvvMobile.Database.Models;
using EvvMobile.Extensions;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.Pages.Clients;
using EvvMobile.Pages.CollectClinicalData;
using EvvMobile.Services;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Clients;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using EvvMobile.ViewModels.CollectClinicalData;
using EvvMobile.Views.Schedules;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Media;
using EvvMobile.Camera;
using EvvMobile.ViewModels.BarCode;
using EvvMobile.Pages.BarCode;

namespace EvvMobile.Pages.Schedules
{
    public partial class ScheduleDetailPage : ScheduleDetailPageXaml
    {
        public ScheduleDetailPage()
        {
            InitializeComponent();
            Title = TextResources.Schedule_Details;
            NavigationPage.SetBackButtonTitle(this, "");
            SystemViewModel.Instance.CleanMessages();

            SignatureImage.Source = ImageSource.FromResource("EvvMobile.Images.EmptySignature.png");

            SignatureImage.Aspect = Aspect.AspectFit;
            var signatureImageTapGestureRecognizer = new TapGestureRecognizer();
            signatureImageTapGestureRecognizer.Tapped += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(ViewModel.ClientSignatureBase64Img))
                {
                    await Navigation.PushAsync(new Schedules.SignaturePage() { BindingContext = ViewModel, Title = TextResources.Signature });
                }
            };
            SignatureImage.GestureRecognizers.Add(signatureImageTapGestureRecognizer);
            var timelineInImageTapGestureRecognizer = new TapGestureRecognizer();
            timelineInImageTapGestureRecognizer.Tapped += (s, e) =>
            {
                TimeLineButtonClicked(s, e);
            };
            TimeLineImage.GestureRecognizers.Add(timelineInImageTapGestureRecognizer);

            var collapseMapTapGestureRecognizer = new TapGestureRecognizer();
            collapseMapTapGestureRecognizer.Tapped += (s, e) =>
            {
                CollapseImageClicked(s, e);
            };
            CollapseImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            CollapseServiceInfoImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            CollapseTasksImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            var profileTapGestureRecognizer = new TapGestureRecognizer();
            profileTapGestureRecognizer.Tapped += (s, e) =>
            {
                ClientProfileClicked(s, e);

            };
            ClientProfileImage.GestureRecognizers.Add(profileTapGestureRecognizer);

            var pinTapGestureRecognizer = new TapGestureRecognizer();
            pinTapGestureRecognizer.Tapped += (s, e) =>
            {
                PinView_OnClicked(s, e);
            };
            PinView.GestureRecognizers.Add(pinTapGestureRecognizer);
            PinTaskView.GestureRecognizers.Add(pinTapGestureRecognizer);
            var addTaskTapGestureRecognizer = new TapGestureRecognizer();
            addTaskTapGestureRecognizer.Tapped += (s, e) =>
            {
                AddTask_OnClicked(s, e);
            };
            AddNewTask.GestureRecognizers.Add(addTaskTapGestureRecognizer);
            VisitTaskList.HeightRequest = App.ScreenHeight * 0.2;
            CameraButton.Clicked += CameraButton_Clicked;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var currentStaff = ViewModel.CurrentStaff();


            if (currentStaff != null)
            {
                if (currentStaff.StaffRejectLateReason == null)
                {
                    currentStaff.StaffRejectLateReason = new ReasonsCommentsDto
                    {
                        Category = ReasonCommentCategory.LateOrEarlyReasonCategory,
                        Key = ReasonCommentCategory.OtherReasonKeyName,
                        SubKey = ViewModel.CurrentStaffId
                    };
                }
                if (currentStaff.StaffRejectLateReason.Category == ReasonCommentCategory.LateOrEarlyReasonCategory)
                {
                    var index = LateEarlReason.Items.IndexOf(currentStaff.StaffRejectLateReason.Key);
                    if (index >= 0)
                        LateEarlReason.SelectedIndex = index;
                }

                ViewModel.PropertiesChanged();
            }

            InitializeMap();
            if (!string.IsNullOrWhiteSpace(ViewModel.ClientSignatureBase64Img))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(ViewModel.ClientSignatureBase64Img);

                    var memImage = new MemoryStream(bytes);
                    var signature = new Image
                    {
                        Source = ImageSource.FromStream(() => memImage),
                        Aspect = Aspect.AspectFit
                    };
                    //if allow user edit signature, turn on below code
                    /*
                    var signatureImageTapGestureRecognizer = new TapGestureRecognizer();
                    signatureImageTapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        if (string.IsNullOrWhiteSpace(ViewModel.ClientSignatureBase64Img))
                        {
                            await Navigation.PushAsync(new Schedules.SignaturePage() { BindingContext = ViewModel, Title = TextResources.Signature });
                        }
                    };
                    signature.GestureRecognizers.Add(signatureImageTapGestureRecognizer);
                    */
                    SignatureImageLayout.Children.Clear();
                    SignatureImageLayout.Children.Add(signature);
                    SignatureImage = null;
                }
                catch (Exception e)
                {
                    ViewModel.ClientSignatureBase64Img = "";//clear invalid signature
                }


            }
        }
        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            var takePhotoService = DependencyService.Get<ITakePhotoService>();
            var file = await takePhotoService.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                SaveToAlbum = true,
                CustomPhotoSize=50
            });

            if (file == null)
                return;

            PhotoImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });
            using (var stream = file.GetStream())
            {
                //TODO:Face recognition 
                ViewModel.ClientFaceRecognized = true;
                if (!ViewModel.ClientFaceRecognized)
                {
                    FaceRecognitionFail.IsVisible = true;
                    FaceRecognitionFail.Text = "Face Recognition Failed";
                }
                else
                {
                    FaceRecognitionFail.IsVisible = false;
                    FaceRecognitionFail.Text = "";
                }
            }

        }
        private void OnReasonsSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            if (picker != null)
            {
                var currentStaff = ViewModel.CurrentStaff();
                if (currentStaff != null)
                {
                    currentStaff.StaffRejectLateReason = new ReasonsCommentsDto
                    {
                        Category = ReasonCommentCategory.LateOrEarlyReasonCategory,
                        Key = picker.Items[picker.SelectedIndex],
                        SubKey = ViewModel.CurrentStaffId,
                        Content = picker.Items[picker.SelectedIndex] == ReasonCommentCategory.OtherReasonKeyName ?
                            ViewModel.OtherReasonForLateOrEarly : picker.Items[picker.SelectedIndex]
                    };
                    ViewModel.PropertiesChanged();
                }
            }

        }

        private async void OnClockInButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel.ClockInCommand.CanExecute(null))
            {
                var trackingStarted = false;
                if (ViewModel.IsTransportationService()) // Transportation
                {
                     trackingStarted = await TrackingService.StartTracking(ViewModel.Id);
  
                }
                if (!trackingStarted)
                {
                    try
                    {
                        var locator = CrossGeolocator.Current;
                        var position = await locator.GetPositionAsync(timeoutMilliseconds: 1000);
                        SystemViewModel.Instance.CurrentLatitude = position.Latitude;
                        SystemViewModel.Instance.CurrentLongitude = position.Longitude;
                    }
                    catch (Exception ex)
                    {
                        SystemViewModel.Instance.ErrorMessage = "Cannot get current location, will use default value.";
                    }
                }

                await ViewModel.ExecuteClockInCommand();
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Clock In failed", "You cannot clock in.", "Ok");
            }

        }

        private async void OnClockOutButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel.ClockOutCommand.CanExecute(null))
            {
                
                if (ViewModel.IsTransportationService()) // Transportation
                {
                    var trackingId = await TrackingService.EndTracking(ViewModel.Id);
                    if (trackingId < 0)
                    {
                        await DisplayAlert("Clock Out failed", "You cannot clock out.", "Ok");
                        return;
                    }
                    else
                    {
                        ViewModel.LocationTrackingId = trackingId;
                        var locationTrackingRecord = await ViewModel.GetLocationTrackingAsync(ViewModel.LocationTrackingId);
                        //update distance and duration
                        if(locationTrackingRecord!=null)
                            ViewModel.TravelDistance = locationTrackingRecord.Distance;
                    }
                }

                try
                {
                    var locator = CrossGeolocator.Current;
                    var position = await locator.GetPositionAsync(timeoutMilliseconds: 1000);
                    SystemViewModel.Instance.CurrentLatitude = position.Latitude;
                    SystemViewModel.Instance.CurrentLongitude = position.Longitude;
                }
                catch (Exception ex)
                {
                    SystemViewModel.Instance.ErrorMessage = "Cannot get current location, will use default value.";
                }
                await ViewModel.ExecuteClockOutCommand();
                await Navigation.PopAsync();

            }
            else
            {
                await DisplayAlert("Clock Out failed", "You cannot clock out.", "Ok");
            }
        }
        //add clockin/out pins,
        //add current positon pin
        private async void InitializeMap()
        {
            if (ViewModel.IsTransportationService() && ViewModel.LocationTrackingId >=0) // Transportation
            {
                var allPositions = await ViewModel.GetAllLocations(ViewModel.LocationTrackingId);
                var locationTrackingRecord = await ViewModel.GetLocationTrackingAsync(ViewModel.LocationTrackingId);
                if (locationTrackingRecord != null)
                    ViewModel.TravelDistance = locationTrackingRecord.Distance;
                ShowRoute(allPositions);
            }
            else
            {
                try
                {
                    List<Location> allPositions = new List<Location>();
                    string address = "Unknown";
                    try
                    {
                        var locator = CrossGeolocator.Current;
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
                    if (string.IsNullOrWhiteSpace(address))
                    {
                        address = "Unknown:" + SystemViewModel.Instance.CurrentLatitude + ", " +
                                  SystemViewModel.Instance.CurrentLongitude;
                    }
                    var currentPosition = new Position(SystemViewModel.Instance.CurrentLatitude, SystemViewModel.Instance.CurrentLongitude);

                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = currentPosition,
                        Label = "Current Location",
                        Address = address
                    };
                    if (!ViewModel.IsClockInDone || !ViewModel.IsClockOutDone)
                    {
                        ClockInOutLocationMap.Pins.Add(pin);
                        allPositions.Add(new Location
                        {
                            Latitude = currentPosition.Latitude,
                            Longitude = currentPosition.Longitude,
                        });
                    }
                    //add clockin/out pins
                    if (Math.Abs(ViewModel.ClockInLatitude - default(Double)) > 0.001d && ViewModel.IsClockInDone)
                    {
                        var startPin = new Pin
                        {
                            Type = PinType.Place,
                            Position = new Position(ViewModel.ClockInLatitude, ViewModel.ClockInLongitude),
                            Label = "Clock In Location",
                            Address = ViewModel.ClockInAddress
                        };
                        ClockInOutLocationMap.Pins.Add(startPin);
                        allPositions.Add(new Location
                        {
                            Latitude = ViewModel.ClockInLatitude,
                            Longitude = ViewModel.ClockInLongitude,
                        });
                    }
                    if (Math.Abs(ViewModel.ClockOutLatitude - default(Double)) > 0.001d && ViewModel.IsClockOutDone)
                    {
                        var endPin = new Pin
                        {
                            Type = PinType.Place,
                            Position = new Position(ViewModel.ClockOutLatitude, ViewModel.ClockOutLongitude),
                            Label = "Clock Out Location",
                            Address = ViewModel.ClockOutAddress
                        };
                        ClockInOutLocationMap.Pins.Add(endPin);
                        allPositions.Add(new Location
                        {
                            Latitude = ViewModel.ClockOutLatitude,
                            Longitude = ViewModel.ClockOutLongitude,
                        });
                    }

                    var moveToPosition = RouteService.CenterOfRoute(allPositions);
                    if (moveToPosition != null)
                        ClockInOutLocationMap.MoveToRegion(moveToPosition);

                }
                catch (Exception e)
                {
                    SystemViewModel.Instance.ErrorMessage = e.Message;
                }
            }


        }

        private async void TimeLineButtonClicked(object sender, EventArgs e)
        {

            var notesViewModel = new NotesViewModel();
            if (ViewModel.Notes != null)
                notesViewModel.Notes.AddRange(ViewModel.Notes);
            //////////////
            //testing code
            notesViewModel.Notes.Add(new ReasonsCommentsDto
            {
                Name = "Tester",
                NoteTime = DateTime.Now.AddHours(-6),
                Category = "Note",
                Content = "Shift Created"
            });
            notesViewModel.Notes.Add(new ReasonsCommentsDto
            {
                Name = "Staff",
                NoteTime = DateTime.Now.AddHours(-5),
                Category = "AcceptNote",
                Content = "Shift Accepted"
            });
            notesViewModel.Notes.Add(new ReasonsCommentsDto
            {
                Name = "MMIS",
                NoteTime = DateTime.Now.AddHours(-3),
                Category = "ApprovalNote",
                Content = "Approved"
            });
            ////////////// 
            /// 
            await Navigation.PushAsync(new Schedules.ShiftTimeLinePage() { BindingContext = notesViewModel, Title = TextResources.TimeLine });

        }

        private void AddTask_OnClicked(object sender, EventArgs e)
        {
            var popupLayout = PopupView;
            if (popupLayout.IsPopupActive)
            {
                popupLayout.DismissPopup();
            }
            else
            {

                var addTaskViewModel = new AddVisitTaskViewModel();

                var popupView = new AddTaskView()
                {

                    BackgroundColor = Color.Gray,
                    BindingContext = addTaskViewModel,
                    HeightRequest = App.ScreenHeight * .5,
                    WidthRequest = App.ScreenWidth * .8

                };
                popupView.OkButtonClicked += (s, args) =>
                {
                    popupLayout.DismissPopup();
                    ViewModel.VisitTasks.Add(new VisitTaskViewModel
                    {
                        Comment = addTaskViewModel.Comment,
                        Instruction = addTaskViewModel.Instruction,
                        TaskName = addTaskViewModel.TaskName,
                        Code = addTaskViewModel.TaskCode,
                        Category = addTaskViewModel.Category
                    });
                };
                popupView.CancelButtonClicked += (s, args) =>
                {
                    popupLayout.DismissPopup();

                };

                popupLayout.ShowPopup(popupView);
            }
        }
        private void PinView_OnClicked(object sender, EventArgs e)
        {
            ShiftDetailScrollview.InputTransparent = !ShiftDetailScrollview.InputTransparent;
            if (ShiftDetailScrollview.InputTransparent)
            {
                PinView.Source = ImageSource.FromResource("EvvMobile.Images.Unpin.png");
                PinTaskView.Source = ImageSource.FromResource("EvvMobile.Images.Unpin.png");
            }
            else
            {
                PinView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
                PinTaskView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
            }
        }

        private void CollapseImageClicked(object sender, EventArgs e)
        {
            bool isVisible = false;
            if (sender.Equals(CollapseImage))
            {
                isVisible = MapStackLayout.IsVisible = !MapStackLayout.IsVisible;
                PinView.IsVisible = isVisible;
            }
            else if (sender.Equals(CollapseServiceInfoImage))
            {
                isVisible = ServiceInfoGrid.IsVisible = !ServiceInfoGrid.IsVisible;
            }
            else if (sender.Equals(CollapseTasksImage))
            {
                isVisible = VisitTaskList.IsVisible = !VisitTaskList.IsVisible;
                PinTaskView.IsVisible = isVisible;
                AddNewTask.IsVisible = isVisible;
            }
            else
                return;
            if (isVisible)
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.collapsearrow40.png");
            }
            else
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.expandarrow40.png");
                ShiftDetailScrollview.InputTransparent = false;
                PinView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
                PinTaskView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
            }              
        }

        private async void OnScanBarCode(object sender, EventArgs e)
        {
            var barModel = new BarCodeViewModel();
            await Navigation.PushAsync(new BarCodeScanPage() { BindingContext = barModel, Title = "" });
            var barcode = barModel.ResultText;
        }
        private async void ClientProfileClicked(object sender, EventArgs e)
        {
            //Get client profile from back end
            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
            var clientId = ViewModel.ClientId;
            if (!String.IsNullOrEmpty(clientId))
            {
                try
                {
                    var resposne = await schedulerDataService.GetMobileClientProfileByClientId(clientId);
                    if (!resposne.IsFalied)
                    {
                        if (resposne.ModelObject != null)
                        {
                            var profileModel = AutoMapper.Mapper.Map<Evv.Message.Portable.Schedulers.Dtos.MobileClientProfileDto,
                                ClientProfileViewModel>(resposne.ModelObject);
                            await Navigation.PushAsync(new ClientProfilePage() { BindingContext = profileModel, Title = TextResources.ClientProfile });

                        }
                        else
                        {
                            await this.DisplayAlert("Message :", "No Client profile found!", "OK");
                        }
                    }
                    else
                    {
                        await this.DisplayAlert("Failed to get client profile", resposne.ErrorMessage, "OK");
                    }
                }
                catch (Exception exception)
                {
                    await this.DisplayAlert("Exceptions", exception.Message, "OK");
                }

            }
            else
            {
                await this.DisplayAlert("Error", "ClientId is empty!", "OK");
            }
            /*     //test
                 var profileModel = new ClientProfileViewModel
                 {
                     ClientPictureBase64Img =
                         "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",
                     ClientFullName = "Test Client",
                     ClientMaNumber = ViewModel.ClientMaNumber,// "123456789",
                     Gender = "Male",
                     DateOfBirth = DateTime.Now.AddYears(-50),
                     MaritalStatus = "Married",
                     IsEnglishSpeaking = true,
                     Race = "White",
                     IsHispanicLatino = false,
                     PrimaryLanguage = "English",
                     PhoneNumber = "111-112-0099",
                     CountySpecify = "Howard",
                     EmailAddress = "test@gmail.com",
                     CurrentAddress = "9755 Patuxent Woods Dr. Columbia, MD 21046",
                     CurrentEmergencyContact = "Mike A",
                     CurrentEmergencyContactPhoneNumber = "111-112-0099",
                     CurrentEmergencyContactRelation = "Son",
                     SecondEmergencyContact = "test 2",
                     SecondEmergencyContactPhoneNumber = "222-333-4444",
                     SecondEmergencyContactRelation = "Colleague"
                 };

                 await Navigation.PushAsync(new ClientProfilePage() { BindingContext = profileModel, Title = TextResources.ClientProfile });*/


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
                if (moveToPosition != null)
                    map.MoveToRegion(moveToPosition);

                MapStackLayout.Children.Add(map);
            }
            catch
            {
                SystemViewModel.Instance.ErrorMessage = "Ending Tracking Failed.";
            }
        }
        private async void OnCollectCMButtonClicked(object sender, EventArgs e)
        {
            var clinicalModel = new ClinicalDataViewModel(ViewModel);

            await Navigation.PushAsync(new CollectClinicalDataPage() { BindingContext = clinicalModel, Title = TextResources.Collect_ClinicalData });
        }
    }
    public abstract class ScheduleDetailPageXaml : ModelBoundWithHomeButtonContentPage<ScheduleViewModel> { }
}
