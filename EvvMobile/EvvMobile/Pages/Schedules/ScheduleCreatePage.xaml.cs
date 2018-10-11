using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.Customizations;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using EvvMobile.Views.Schedules;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace EvvMobile.Pages.Schedules
{
    public partial class ScheduleCreatePage : ScheduleCreatePageXaml
    {
        public ScheduleCreatePage()
        {
            InitializeComponent();
            Title = TextResources.Add_Shift;
            SystemViewModel.Instance.CleanMessages();
            NavigationPage.SetBackButtonTitle(this, "");
            _tempScheduleViewModel =new ScheduleViewModel();
            _AvailableModifiers = new List<string>();
            SignatureImage.Source = ImageSource.FromResource("EvvMobile.Images.EmptySignature.png");

            SignatureImage.Aspect = Aspect.AspectFit;
            var signatureImageTapGestureRecognizer = new TapGestureRecognizer();
            signatureImageTapGestureRecognizer.Tapped += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(ViewModel.ClientSignatureBase64Img))
                {
                    _tempScheduleViewModel.ClientSignatureBase64Img = ViewModel.ClientSignatureBase64Img;
                    await Navigation.PushAsync(new Schedules.SignaturePage() { BindingContext = _tempScheduleViewModel, Title = TextResources.Signature });
                }
            };
            SignatureImage.GestureRecognizers.Add(signatureImageTapGestureRecognizer);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrWhiteSpace(_tempScheduleViewModel.ClientSignatureBase64Img))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(_tempScheduleViewModel.ClientSignatureBase64Img);
                    ViewModel.ClientSignatureBase64Img = _tempScheduleViewModel.ClientSignatureBase64Img;
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

        private async void OnClockInButtonClicked(object sender, EventArgs e)
        {
            if(ViewModel.ServiceStartDateTime == DateTimeOffset.MinValue)
                ViewModel.ServiceStartDateTime = DateTimeOffset.Now;

            
            //TODO:validation 
            ViewModel.Validate();

            if (ViewModel.HasErrors)
            {
                // Error message
                ViewModel.ScrollToControlProperty(ViewModel.GetFirstInvalidPropertyName);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ViewModel.ClientSignatureBase64Img))
                {
                    await DisplayAlert("Signature", "Beneficiary's signature is reuired.", "OK");
                    return;
                }
                ClockInButton.IsEnabled = false;
                // No error
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

                //TODO: create and save schedule to db & clock in

                //update list view model
                try
                {
                    var clientFullName = ViewModel.ClientFullName;
                    var names = !string.IsNullOrWhiteSpace(clientFullName)
                        ? clientFullName.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                        : new string[2] {"", ""};
                    var lastName = "";
                    var middleName = "";
                    if (names.Length == 2)
                        lastName = names[1];
                    if (names.Length > 2)
                    {
                        middleName = names[1];
                        lastName = names[2];
                    }
                    var modifiers = ViewModel.Modifiers.Select(x => x.SelectedModifier).ToList();
                    var modifiersString = "";
                    foreach (var m in modifiers)
                    {
                        if (string.IsNullOrWhiteSpace(modifiersString))
                            modifiersString = m;
                        else
                        {
                            modifiersString += "," + m;
                        }
                    }
                    var newShift = new ScheduleViewModel
                    {
                        ServiceStartDateTime = ViewModel.ServiceStartDateTime,
                        ClockInTime = ViewModel.ServiceStartDateTime,
                        IsClockInDone = true,

                        ClientName = new PersonNameDto
                        {
                            FirstName = names[0],
                            MiddleName = middleName,
                            LastName = lastName
                        },
                        ClientFullName = ViewModel.ClientFullName,
                        ClientMaNumber = ViewModel.ClientMaNumber,
                        ProviderName = ViewModel.ProviderName,
                        ProviderMaNumber = ViewModel.ProviderMaNumber,
                        ProcedureCode = ViewModel.ProcedureCode,
                        Modifiers = modifiersString,
                        ServiceName = ViewModel.SelectedServiceName,
                        WorkflowStatus = ServiceVisitWorkflowStatus.InProgress,
                        CurrentStaffId = SystemViewModel.Instance.CurrentStaffId,
                        IsUnscheduled = true,
                        ClockInAddress = address,
                        ClockInLatitude = SystemViewModel.Instance.CurrentLatitude,
                        ClockInLongitude = SystemViewModel.Instance.CurrentLongitude,
                        ClientSignatureBase64Img = ViewModel.ClientSignatureBase64Img

                    };

                    var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;

                    var response = await schedulerDataService.CreateAndClockIn(
                        AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(newShift),
                        SystemViewModel.Instance.HasNetworkConnection);
                    var shiftDtoResult= response.ModelObject;
                    if (shiftDtoResult != null)
                    {
                        if (ViewModel.ParentListViewModel != null)
                        {
                            newShift.Id = shiftDtoResult.Id;
                            newShift.IsUnsynced = shiftDtoResult.IsUnsynced;

                            ViewModel.ParentListViewModel.AddNewShift(newShift);

                        }


                        await DisplayAlert("Clock In Successful", "You have successfully clocked in.", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var errormessage = "Couldn't clock in. Check the fields you entered or try it later. " +response.ErrorMessage;
                        await DisplayAlert("Clock In Failed",
                            errormessage, "OK");
                    }




                }
                catch (Exception exception)
                {
                    var errormessage = "Couldn't clock in. Check the fields you entered or try it later. Exception:" + exception;
                    await DisplayAlert("Clock In Failed",
                        errormessage, "OK");

                }
                finally
                {
                    ClockInButton.IsEnabled = true;
                }
            }
            ClockInButton.IsEnabled = true;

        }

        private void ServiceName_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker != null)
            {
                var selectedIndex = picker.SelectedIndex;
                if (selectedIndex != -1)
                {
                    var serviceType = ViewModel.ServiceTypes[selectedIndex];
                    _AvailableModifiers = serviceType.AvailableModifiers;
                    ViewModel.SelectedServiceName = serviceType.ServiceName;
                    ViewModel.ProcedureCode = serviceType.ProcedureCode;
                    ModifersLayout.Children.Clear();
                    var newModifier = new ModifierCellViewModel
                    {
                        ModifierStrings = _AvailableModifiers,
                        SelectedModifier = ""
                    };

                    newModifier.ModifierDeleted += OnModifierDeleted;
                    ViewModel.Modifiers.Clear();
                    ViewModel.Modifiers.Add(newModifier);
                    ModifersLayout.Children.Add(new ModifierGridView { BindingContext = newModifier });                    
                }

            }
        }

        private void OnAddButtonClicked(object sender, EventArgs e)
        {
           var newModifier = new ModifierCellViewModel
           {
               ModifierStrings = _AvailableModifiers,
               SelectedModifier = ""
           };

            newModifier.ModifierDeleted += OnModifierDeleted;
            ViewModel.Modifiers.Add(newModifier);
            ModifersLayout.Children.Add(new ModifierGridView {BindingContext = newModifier});
        }

        private void OnModifierDeleted(object sender, EventArgs e)
        {
            var modifierModel = sender as ModifierCellViewModel;
            if (modifierModel != null)
            {
                var index = ViewModel.Modifiers.IndexOf(modifierModel);
                ModifersLayout.Children.RemoveAt(index);
                ViewModel.Modifiers.RemoveAt(index);
            }
        }

        private async void ProviderNumber_TextChanged(object sender, TextChangedEventArgs e)
       {
            if (SystemViewModel.Instance.HasNetworkConnection)
            {
                if (e.OldTextValue == null)
                    return;
                var oldText = e.OldTextValue.Trim();
                var newText = e.NewTextValue.Trim();
                var providerMaNumber = newText;
                if (oldText != newText && newText.Length == 8)
                {
                    var clientMaNumber = this.BeneficiaryMaNumber.Text.Trim();
                    var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;

                    //get service types
                    var serviceTypes = await schedulerDataService.GetServiceTypesByMaNumber(clientMaNumber, providerMaNumber, DateTimeOffset.Now);
                    if (serviceTypes.ModelObject == null)
                    {
                        //error, show error dialog?
                        await this.DisplayAlert("Couldn't Get Service Types.", serviceTypes.ErrorMessage, "OK");
                        return;                       
                    }

                    ViewModel.ServiceTypes.Clear();
                    foreach (var lxService in serviceTypes.ModelObject)
                    {
                        var serviceTypeVM = new ServiceTypeViewModel() { ServiceName = lxService.Name, ProcedureCode = lxService.ProcedureCode };
                        var firstDelimiterIndex = lxService.UniqueName.IndexOf('_');
                        if (firstDelimiterIndex >= 0)
                        {
                            serviceTypeVM.AvailableModifiers = lxService.UniqueName
                                .Substring(firstDelimiterIndex)
                                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        else
                        {
                            serviceTypeVM.AvailableModifiers = new List<string>();
                        }

                        ViewModel.ServiceTypes.Add(serviceTypeVM);
                    }
                    //get provider info
                    var providerInfo = await schedulerDataService.GetActiveProviderByMaNumber(providerMaNumber);
                    if (!providerInfo.IsFalied)
                    {
                        if (providerInfo.ModelObject != null)
                        {
                            ViewModel.ProviderName = providerInfo.ModelObject.ProviderName;
                        }
                        else {
                            await this.DisplayAlert("Message :", "PROVIDER NUMBER is invalid !", "OK");
                        }
                        
                    }
                    else
                    {
                        await this.DisplayAlert("Message :", "Internal Server Error!", "OK");
                    }
                }
            }
            
        }

        private async void BeneficiaryMaNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SystemViewModel.Instance.HasNetworkConnection)
            {
                if (e.OldTextValue == null)
                    return;
                var oldText = e.OldTextValue.Trim();
                var newText = e.NewTextValue.Trim();
                if (oldText != newText)
                {
                    if (newText.Length == 9)
                    {
                        var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
                        var resposne = await schedulerDataService.GetClientInfoByMaNumber(newText);
                        if (!resposne.IsFalied)
                        {
                            if (resposne.ModelObject!=null && resposne.ModelObject.ValidationStatus == "Single")
                            {
                                ViewModel.ClientFullName = resposne.ModelObject.PersonName != null ? resposne.ModelObject.PersonName.FullName : "";
                            }
                            else
                            {
                                await this.DisplayAlert("Message :", "BENEFICIARY MA# is invalid !", "OK");
                            }
                        }
                        else
                        {
                            await this.DisplayAlert("Message :", "Internal Server Error!", "OK");
                        }
                    }
                }
            }
       }

        private IList<string> _AvailableModifiers;
        private ScheduleViewModel _tempScheduleViewModel;
    }
    public abstract class ScheduleCreatePageXaml : ModelBoundWithHomeButtonContentPage<AddShiftViewModel> { }
}
