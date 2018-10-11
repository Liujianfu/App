using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.Localization;
using EvvMobile.Pages.Schedules;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using EvvMobile.Database.Models;
using EvvMobile.Database.Repositories;
using EvvMobile.ViewModels.CollectClinicalData;
namespace EvvMobile.ViewModels.Schedules
{
    public class ScheduleViewModel : BaseViewModel
    {
        public ScheduleViewModel()
        {
            ClientAcceptanceStatus = AcceptanceStatus.NotRequired;
            LateOrEralyThreshold = AppConifgurations.DefaultLateOrEarlyThreshold;
            VisitTasks = new ObservableCollection<VisitTaskViewModel>();
            VisitMeasurements = new ObservableCollection<VisitMeasurementViewModel>();
            ///TODO:remove this
            //VisitTasks.Add(new VisitTaskViewModel
            //{
            //    TaskName = "task 1",
            //    Instruction = "add description here",
            //    IsTaskDeclined = true,
            //    IsScheduled = true
            //});
            //var taskModel = new VisitTaskViewModel
            //{
            //    TaskName = "feeding",
            //    Instruction = "add task details here. ",
            //    IsTaskDeclined = false,
            //    IsScheduled = true,
            //    IsShiftCompleted = true
            //};
            //VisitTasks.Add(taskModel);
            //var taskModel2 = new VisitTaskViewModel
            //{
            //    TaskName = "Clean up",
            //    Instruction = "add task details here. ",
            //    IsTaskDeclined = false,
            //    IsScheduled = false,
            //    IsShiftCompleted = false
            //};
            //VisitTasks.Add(taskModel2);
            //var taskModel3 = new VisitTaskViewModel
            //{
            //    TaskName = "Vital test",
            //    Instruction = "add task details here. ",
            //    IsTaskDeclined = false,
            //    IsScheduled = true,
            //    IsShiftCompleted = false
            //};
            //VisitTasks.Add(taskModel3);

        }

        //offline action type
        public string ActionType { get; set; }

        public string Id { get; set; }

        #region Provider Info

        public string OrganizationUnitId { get; set; }

        public string ProviderId { get; set; }

        public string ProviderName { get; set; }

        public string ProviderMaNumber { get; set; }

        public TaxIdentifierDto TaxIdentifier { get; set; }

        #endregion

        #region Client Info

        public string ClientId { get; set; }

        public PersonNameDto ClientName { get; set; }

        public string ClientIdentifier { get; set; }

        public string ClientMaNumber { get; set; }

        public PhoneDto ClientPhone { get; set; }

        public string PrimaryDiagnosisCode { get; set; }

        public string SecondaryDiagnosisCode { get; set; }
        public string ClientSignatureBase64Img { get; set; }
        private bool _clientFaceRecognized;
        public bool ClientFaceRecognized
        {
            get
            {
                return _clientFaceRecognized;
            }

            set
            {
                if (_clientFaceRecognized != value)
                {
                    SetProperty(ref _clientFaceRecognized, value, "ClientFaceRecognized");

                    OnPropertyChanged("ClientFaceRecognized");


                }
            }
        }
        #endregion

        #region Visit Info

        public DateTimeOffset? ServiceStartDateTime { get; set; }

        public DateTimeOffset? ServiceEndDateTime { get; set; }

        public string ServiceRenderAddress { get; set; }

        public string WaiverType { get; set; }

        public string ServiceName { get; set; }  //ServiceType??

        public string ProcedureCodeAndModifier
        {
            get
            {
                var codeAndModifiers = ProcedureCode;
                if(!string.IsNullOrWhiteSpace(Modifiers))
                    codeAndModifiers += "_"+ Modifiers;
                if (string.IsNullOrWhiteSpace(codeAndModifiers))
                    return "";
                return codeAndModifiers.Replace(",","_");
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var inputString = value;
                    var pos = inputString.IndexOf("_", StringComparison.Ordinal);
                    if (pos >= 0)
                    {
                        ProcedureCode = inputString.Substring(0, pos);
                        if (pos + 1 < inputString.Length)
                            inputString = inputString.Substring(pos + 1);
                        else
                        {
                            inputString = "";
                        }
                        Modifiers = inputString.Replace("_", ",");
                    }
                    else
                    {
                        ProcedureCode = inputString;
                    }
                   
                }
            }
            
        }

        public string NoteToCareGiver { get; set; }

        public string NoteToClient { get; set; }

        public IList<VisitStaffDto> VisitStaffs { get; set; }
        public string CurrentStaffId { get; set; }
        public ObservableCollection<VisitTaskViewModel> VisitTasks { get; set; }
        public ObservableCollection<VisitMeasurementViewModel> VisitMeasurements { get; set; }
        #endregion

        #region MMIS Granted

        public MmisAppovalStatus MmisAppovalStatus { get; set; }

        #endregion

        #region Clock Out/In 
        public bool IsClockInDone
        {
            get
            {
                return _clockInDone ;
            }

            set
            {
                if (_clockInDone != value)
                {
                    SetProperty(ref _clockInDone, value, ClockInDonePropertyName);

                    OnPropertyChanged("NeedReasonForLateOrEarly");
                    OnPropertyChanged("ClientCanAcceptOrReject");
                    OnPropertyChanged("StaffCanAcceptOrReject");

                }
            }
        }

        public bool IsClockOutDone
        {
            get
            {
                return _clockOutDone ;
            }

            set
            {
                if (_clockOutDone != value)
                {
                    SetProperty(ref _clockOutDone, value, ClockOutDonePropertyName);
                    OnPropertyChanged("ClientCanAcceptOrReject");
                    OnPropertyChanged("StaffCanAcceptOrReject");

                }
            }
        }


        public DateTimeOffset? ClockInTime{get;set;}



        public DateTimeOffset? ClockOutTime{get; set;}
        #region ClockIn/Out locations
        public double ClockInLatitude { get; set; }
        public double ClockInLongitude { get; set; }
        private string _clockInAddress;
        public string ClockInAddress
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_clockInAddress))
                    return _clockInAddress;
                else
                {
                    return ClockOutAddress;
                }
            }
            set { _clockInAddress = value; }
        }
        public double ClockOutLatitude { get; set; }
        public double ClockOutLongitude { get; set; }
        public string ClockOutAddress { get; set; }
        public int LocationTrackingId { get; set; }

        #endregion
        #endregion

        #region Staff Acceptence Info
        public AcceptanceStatus StaffAcceptanceStatus
        {
            get { return _staffAcceptanceStatus; }
            set
            {
                if (_staffAcceptanceStatus != value)
                {
                    SetProperty(ref _staffAcceptanceStatus, value, StaffAcceptanceStatusPropertyName);

                    OnPropertyChanged("NeedReasonForLateOrEarly");

                    OnPropertyChanged("ClientCanAcceptOrReject");
                    OnPropertyChanged("StaffCanAcceptOrReject");
                    OnPropertyChanged("ClockInEnable");
                    OnPropertyChanged("ClockOutEnable");
                }
            }
        }
        #endregion

        #region Client Acceptence Info
        public AcceptanceStatus ClientAcceptanceStatus
        {
            get { return _clientAcceptanceStatus; }
            set
            {
                if (_clientAcceptanceStatus != value)
                {
                    SetProperty(ref _clientAcceptanceStatus, value, ClientAcceptanceStatusPropertyName);
                    OnPropertyChanged("NeedReasonForLateOrEarly");
                    OnPropertyChanged("ClientCanAcceptOrReject");
                    OnPropertyChanged("StaffCanAcceptOrReject");
                    OnPropertyChanged("ClockInEnable");
                    OnPropertyChanged("ClockOutEnable");
                }
            }
        }
        #endregion

        public long UniqueVisitId { get; set; }

        public bool Overlapped { get; set; }

        public bool AutoAccepted { get; set; }

        public int HoursOfAutoAccepted { get; set; }

        public IList<AttachmentItemDto> Attachments { get; set; }
        public WorkflowStatusDto WorkflowStatus
        {
            get { return _workflowStatus; }
            set
            {
                if (_workflowStatus != value)
                {
                    SetProperty(ref _workflowStatus, value, WorkflowStatusPropertyName);
                    OnPropertyChanged("ClientCanAcceptOrReject");
                    OnPropertyChanged("StaffCanAcceptOrReject");
                    OnPropertyChanged("WorkflowStatusName");
                    OnPropertyChanged("WorkflowStatusDisplayName");
                    
                }
            }
        }


        public bool IsUnscheduled { get; set; }
        #region Reasons_Comments
        /// <summary>
        /// Minutes, how many minutes early/ late than scheduled time will be treated as too early / too late. 
        /// user will be asked to fill the early/late reason
        /// </summary>
        public int LateOrEralyThreshold { get; set; }

        public ObservableCollection<ReasonsCommentsDto> Notes
        {
            get { return _notes; }
            set
            {
                SetProperty(ref _notes, value, NotesPropertyName);
            }
        }
        public bool IsVisitLate { get;  set; }
        #endregion

        #region Location

        public double LocationLatitude { get; set; }

        public double LocationLongitude { get; set; }
        public bool LocationMatched { get; set; }

        private double _travelDistance;
        public double TravelDistance
        {
            get { return _travelDistance; }
            set
            {
                if (_travelDistance != value)
                {
                    _travelDistance = value;
                    OnPropertyChanged("TravelDistance");
                }
            }
        }
        #endregion

        #region PageUseOnly   

        private TimeSpan? _timeIn;
        public TimeSpan? TimeIn
        {
            get
            {
                if(!_timeIn.HasValue)
                {
                    if(ClockInTime.HasValue)
                        _timeIn=ClockInTime.Value.TimeOfDay;

                }
                return _timeIn;
            }
            set
            {
                _timeIn = value;
                OnPropertyChanged("ClockInEnable");                    
            }
        }

        private TimeSpan? _timeOut;
        public TimeSpan? TimeOut
        {
            get
            {
                if(!_timeOut.HasValue)
                {
                    if(ClockOutTime.HasValue)
                         _timeOut=ClockOutTime.Value.TimeOfDay;

                }
                return _timeOut;
            }
            set
            {
                _timeOut = value;
                OnPropertyChanged("ClockOutEnable");
            }
        }

        private DateTime? _clockInDate;
        public DateTime? ClockInDate
        {
            get
            {
                if (!_clockInDate.HasValue)
                {
                    if(ClockInTime.HasValue)
                        _clockInDate =ClockInTime.Value.Date;
                }
                return _clockInDate;
            }
            set
            {
                _clockInDate = value;
                OnPropertyChanged("ClockInEnable");
            }
        }

        private DateTime? _clockOutDate;
        public DateTime? ClockOutDate
        {
            get
            {
                if(!_clockOutDate.HasValue)
                {
                   if(ClockOutTime.HasValue)
                        _clockOutDate =ClockOutTime.Value.Date;
                }
                return _clockOutDate;
            }
            set
            {
                _clockOutDate = value;
                OnPropertyChanged("ClockOutEnable");
            }
        }
        public string ServiceImageName
        {
            get
            {
                if (IsTransportationService()) // Transportation
                {
                    return "Car";
                }
                else
                {
                    return "HomeService";
                }
            }
        }


        public string ProcedureCode { get; set; }
        public string Modifiers { get; set; }
        private bool _isUnsynced;
        public bool IsUnsynced
        {
            get { return _isUnsynced; }
            set
            {
                if (_isUnsynced != value)
                {
                    _isUnsynced = value;
                    OnPropertyChanged("IsUnsynced");
                }
            }
        }
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
                        ClientName= new PersonNameDto();
                    }
                    ClientName.FirstName = names[0];
                    if (names.Length > 1)
                    {
                        ClientName.LastName = names[1];
                    }
                }
            }
        }
        public WorkflowStatusDto CurrentStaffStatus
        {
            get
            {

                if (!string.IsNullOrWhiteSpace(CurrentStaffId) && VisitStaffs != null)
                {
                    foreach (var staff in VisitStaffs)
                    {
                        if (staff.StaffId.Equals(CurrentStaffId, StringComparison.OrdinalIgnoreCase))
                        {

                            return staff.Status ;
                        }
                    }
                }
                return VisitStaffAssignmentStatus.AcceptanceNotRequired;
            }
        }

        public string WorkflowStatusName
        {
            get
            {
                if (WorkflowStatus != null)
                    return WorkflowStatus.Name;
                else
                    return "";
            }
        }
        
        public string WorkflowStatusDisplayName
        {
            get
            {
                if (WorkflowStatus != null)
                    return WorkflowStatus.DisplayName;
                else
                    return "";
            }
        }

        public string ScheduleTimeRangeString
        {
            get
            {
                return string.Format("{0}-{1}", ServiceStartDateTime.HasValue ? ServiceStartDateTime.Value.ToString("t") : " ",
                    ServiceEndDateTime.HasValue ? ServiceEndDateTime.Value.ToString("t") : "");
            }
        }
        public bool NeedReasonForLateOrEarly
        {
            get
            {
                if (!ServiceStartDateTime.HasValue ||/* LateOrEralyThreshold <=0|| */!CanClockInOut|| !SystemViewModel.Instance.IsStaffLogin)
                    return false;
                if (IsVisitLate)
                {
                    return true;
                }
                if (IsClockInDone&& ClockInTime.HasValue)
                {
                    if (LateOrEralyThreshold > 0 &&
                        Math.Abs((ClockInTime.Value - ServiceStartDateTime.Value).TotalMinutes) > LateOrEralyThreshold)
                    {
                        IsVisitLate = true;
                        return true;
                    }
                    return false;
                }
                if( LateOrEralyThreshold > 0 && Math.Abs((DateTimeOffset.Now - ServiceStartDateTime.Value).TotalMinutes )>
                       LateOrEralyThreshold)
                {
                    IsVisitLate = true;
                    return true;
                }
                return false;
            }
        }
        public string OtherReasonForLateOrEarly
        {
            get
            {
                var currentStaff = CurrentStaff();


                if (currentStaff != null)
                {
                    if (currentStaff.StaffRejectLateReason == null)
                    {
                        currentStaff.StaffRejectLateReason = new ReasonsCommentsDto
                        {
                            Category = ReasonCommentCategory.LateOrEarlyReasonCategory,
                            Key = ReasonCommentCategory.OtherReasonKeyName,
                            SubKey = CurrentStaffId
                        };
                    }
                    if(currentStaff.StaffRejectLateReason.Category== ReasonCommentCategory.LateOrEarlyReasonCategory&& 
                        currentStaff.StaffRejectLateReason.Key == ReasonCommentCategory.OtherReasonKeyName)
                        _otherReasonForLateOrEarly = currentStaff.StaffRejectLateReason.Content;

                    return _otherReasonForLateOrEarly;
                }

                return _otherReasonForLateOrEarly;
            }
            set
            {
                SetProperty(ref _otherReasonForLateOrEarly, value, OtherReasonForLateOrEarlyPropertyName);
                OnPropertyChanged("ClockInEnable");
                OnPropertyChanged("ClockOutEnable");
                var currentStaff = CurrentStaff();
                if (currentStaff != null)
                {
                    if (currentStaff.StaffRejectLateReason == null)
                    {
                        currentStaff.StaffRejectLateReason = new ReasonsCommentsDto
                        {
                            Category = ReasonCommentCategory.LateOrEarlyReasonCategory,
                            Key = ReasonCommentCategory.OtherReasonKeyName,
                            SubKey = CurrentStaffId
                        };
                    }
                    currentStaff.StaffRejectLateReason.Category = ReasonCommentCategory.LateOrEarlyReasonCategory;
                    currentStaff.StaffRejectLateReason.Key = ReasonCommentCategory.OtherReasonKeyName;
                    currentStaff.StaffRejectLateReason.Content = _otherReasonForLateOrEarly;
                }
            }
        }

        public bool ShowOtherLateOrEarlyReason
        {
            get
            {
                var currentStaff = CurrentStaff();
                if (currentStaff != null&& currentStaff.StaffRejectLateReason != null)
                {
                    if (currentStaff.StaffRejectLateReason.Category == ReasonCommentCategory.LateOrEarlyReasonCategory && 
                        currentStaff.StaffRejectLateReason.Key == ReasonCommentCategory.OtherReasonKeyName)
                    {
                        return true;
                    }
                }
                return false;

            }
        }

        public string OtherStaffRejectionReason
        {
            get
            {
                var currentStaff = CurrentStaff();
                if (currentStaff == null)
                    return _otherStaffRejectionReason;
                if (string.IsNullOrWhiteSpace(_otherStaffRejectionReason))
                {
                    if (currentStaff.StaffRejectLateReason != null &&
                        currentStaff.StaffRejectLateReason.Category == ReasonCommentCategory.StaffRejctionReasonCategory&&
                        currentStaff.StaffRejectLateReason.Key.Equals(ReasonCommentCategory.OtherReasonKeyName, StringComparison.OrdinalIgnoreCase) )
                    {
                        _otherStaffRejectionReason = currentStaff.StaffRejectLateReason.Content;
                        return _otherStaffRejectionReason;
                    }
                }

                return _otherStaffRejectionReason;
            }
            set
            {
                SetProperty(ref _otherStaffRejectionReason, value, OtherStaffRejectionReasonPropertyName);
                var currentStaff = CurrentStaff();
                if (currentStaff != null)
                {
                    if (currentStaff.StaffRejectLateReason == null)
                    {
                        currentStaff.StaffRejectLateReason = new ReasonsCommentsDto
                        {
                            Category = ReasonCommentCategory.StaffRejctionReasonCategory,
                            Key = ReasonCommentCategory.OtherReasonKeyName
                        };
                    }
                    currentStaff.StaffRejectLateReason.Category = ReasonCommentCategory.StaffRejctionReasonCategory;
                    currentStaff.StaffRejectLateReason.Content = _otherStaffRejectionReason;
                    currentStaff.StaffRejectLateReason.SubKey = CurrentStaffId;
                }

            }
        }

        public bool ShowOtherStaffRejectionReason
        {
            get
            {
                var currentStaff = CurrentStaff();
                if (currentStaff!=null &&
                    currentStaff.StaffRejectLateReason != null&&
                    currentStaff.StaffRejectLateReason.Category == ReasonCommentCategory.StaffRejctionReasonCategory&&
                    currentStaff.StaffRejectLateReason.Key.Equals(ReasonCommentCategory.OtherReasonKeyName, StringComparison.OrdinalIgnoreCase) )
                {
                    return true;
                }

                return false;

            }
        }

        public bool ClientCanAcceptOrReject
        {
            get
            {
                return SystemViewModel.Instance.IsClientLogin&&!IsClockOutDone && !IsClockInDone && WorkflowStatus != ServiceVisitWorkflowStatus.Completed &&
                       WorkflowStatus != ServiceVisitWorkflowStatus.CompletedWithoutClockIn&& WorkflowStatus != ServiceVisitWorkflowStatus.Discarded && 
                       WorkflowStatus != ServiceVisitWorkflowStatus.InProgress&&
                       ClientAcceptanceStatus != AcceptanceStatus.Rejected;
            }
        }
        public bool StaffCanAcceptOrReject
        {
            get
            {
                return SystemViewModel.Instance.IsStaffLogin &&!IsClockOutDone && !IsClockInDone &&
                    WorkflowStatus != ServiceVisitWorkflowStatus.Completed &&
                       WorkflowStatus != ServiceVisitWorkflowStatus.CompletedWithoutClockIn &&
                       WorkflowStatus != ServiceVisitWorkflowStatus.Discarded &&
                       WorkflowStatus != ServiceVisitWorkflowStatus.InProgress &&
                       CurrentStaffStatus != VisitStaffAssignmentStatus.Accepted &&
                       CurrentStaffStatus != VisitStaffAssignmentStatus.Rejected
                       && CurrentStaffStatus != VisitStaffAssignmentStatus.RemovedByProvider;
            }
        }
        public bool CanClockInOut
        {
            get
            {
                return (StaffAcceptanceStatus == AcceptanceStatus.Accepted || StaffAcceptanceStatus == AcceptanceStatus.NotRequired) &&
                       (ClientAcceptanceStatus == AcceptanceStatus.Accepted || ClientAcceptanceStatus == AcceptanceStatus.NotRequired);
            }
        }

        public bool ClockInEnable
        {
            get
            {

                var enabled = !IsBusy && !IsClockInDone && (StaffAcceptanceStatus == AcceptanceStatus.Accepted ||
                                                           StaffAcceptanceStatus == AcceptanceStatus.NotRequired) &&
                             (ClientAcceptanceStatus == AcceptanceStatus.Accepted ||
                              ClientAcceptanceStatus == AcceptanceStatus.NotRequired) &&
                             (IsClockOutDone || !NeedReasonForLateOrEarly || NeedReasonForLateOrEarly &&
                              HasValidReason(ReasonCommentCategory.LateOrEarlyReasonCategory));
                if (NeedReasonForLateOrEarly)
                {
                    if (!TimeIn.HasValue || !ClockInDate.HasValue)
                    {
                        return false;
                    }
                    var clockinDateTime = new DateTimeOffset(ClockInDate.Value.Year, ClockInDate.Value.Month, 
                            ClockInDate.Value.Day, TimeIn.Value.Hours, TimeIn.Value.Minutes, 0, DateTimeOffset.Now.Offset);
                    return enabled && (!IsClockOutDone || clockinDateTime < ClockOutTime);
                }
                return enabled && (!IsClockOutDone || DateTimeOffset.Now < ClockOutTime);
            }
        }
        public bool ClockOutEnable
        {
            get
            {
                var enabled = !IsBusy && !IsClockOutDone &&
                             (StaffAcceptanceStatus == AcceptanceStatus.Accepted ||
                              StaffAcceptanceStatus == AcceptanceStatus.NotRequired) &&
                             (ClientAcceptanceStatus == AcceptanceStatus.Accepted ||
                              ClientAcceptanceStatus == AcceptanceStatus.NotRequired) &&
                             (IsClockInDone || !NeedReasonForLateOrEarly || NeedReasonForLateOrEarly &&
                              HasValidReason(ReasonCommentCategory.LateOrEarlyReasonCategory));
                if (NeedReasonForLateOrEarly)
                {
                    if (!TimeOut.HasValue || !ClockOutDate.HasValue)
                    {
                        return false;
                    }
                    var clockOutDateTime = new DateTimeOffset(ClockOutDate.Value.Year, ClockOutDate.Value.Month,
                        ClockOutDate.Value.Day, TimeOut.Value.Hours, TimeOut.Value.Minutes, 0, DateTimeOffset.Now.Offset);
                    return enabled&&(!IsClockInDone|| clockOutDateTime > ClockInTime) ;
                }
                return enabled && (!IsClockInDone || DateTimeOffset.Now > ClockInTime); 
            }
        }

        public IBaseViewModel ParentContainerViewModel { get; set; }

        public bool IsTransportationService()
        {
            return (ProcedureCode == "T1000"); //demo/test Transportation
        }

        public async Task<List<Location>> GetAllLocations(int trackingId)
        {
            if (!SystemViewModel.Instance.HasNetworkConnection)
            {
                return await _locationOperation.GetLocationsAsync(trackingId);

            }
            else
            {
                //TODO: get locations from RESTFul service
                return await _locationOperation.GetLocationsAsync(trackingId);
            }

        }

        public async Task<LocationTracking> GetLocationTrackingAsync(int trackingId)
        {
            return await _locationTrackingOperation.GetLocationTrackingAsync(trackingId);
        }
        #endregion



        #region commands
        Command _staffAcceptCommand;
        public Command StaffAcceptCommand
        {
            get { return _staffAcceptCommand ?? (_staffAcceptCommand = new Command(() => ExecuteStaffAcceptCommand(), () =>
                             {
                                 return SystemViewModel.Instance.HasNetworkConnection && !IsBusy&& !IsClockOutDone && 
                                 !IsClockInDone && StaffCanAcceptOrReject;
                             })); }
        }

        async void ExecuteStaffAcceptCommand()
        {
           
            if (IsBusy)
                return;
            
            IsBusy = true;
            try
            {
                var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;

                //call back end service to update status
                var shiftDto = AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(this);
                var acceptResponse = await schedulerDataService.StaffAccept(shiftDto, SystemViewModel.Instance.HasNetworkConnection);
                if (acceptResponse.ModelObject != null)
                {
                    if (CurrentStaff() != null)
                    {
                         CurrentStaff().Status = VisitStaffAssignmentStatus.Accepted;
                    }
                    StaffAcceptanceStatus = acceptResponse.ModelObject.StaffAcceptanceStatus;
                    WorkflowStatus.Name = acceptResponse.ModelObject.WorkflowStatus.Name;
                    WorkflowStatus.DisplayName = acceptResponse.ModelObject.WorkflowStatus.DisplayName;
                }
                else
                {
                    var errorMessage = acceptResponse.ErrorMessage;
                    await App.CurrentApp.MainPage.DisplayAlert("Accepting Shift Failed", errorMessage, "Ok");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                await App.CurrentApp.MainPage.DisplayAlert("Accepting Shift Failed", errorMessage, "Ok");
            }
            finally
            {
                IsBusy = false;
                RaiseCommandsChangeCanExecuteEvent();
            }
            Refresh();


        }
        Command _UnsyncedDeleteCommand;
        public Command UnsyncedDeleteCommand
        {
            get
            {
                return _UnsyncedDeleteCommand ?? (_UnsyncedDeleteCommand = new Command(() => ExecuteUnsyncedDeleteCommand(), () =>
                {
                    return IsUnsynced && !IsBusy ;
                }));
            }
        }

        async void ExecuteUnsyncedDeleteCommand()
        {
            var deleteSucceed = false;
            if (IsBusy)
                return;

            var answer = await App.CurrentApp.MainPage.DisplayAlert("Delete Confirmation",
                "Are you sure you want to delete the unsynced shift?", "Yes", "No");
            if (answer)
            {
                IsBusy = true;
                try
                {
                    var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
                    var shiftDto = AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(this);
                    deleteSucceed = await schedulerDataService.DeleteUnsyncedServiceVisit(shiftDto);
                    WorkflowStatus = ServiceVisitWorkflowStatus.Discarded;
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.Message;
                    await App.CurrentApp.MainPage.DisplayAlert("Deleting Shift Failed", errorMessage, "Ok");
                }
                finally
                {
                    IsBusy = false;
                    RaiseCommandsChangeCanExecuteEvent();
                }
            }
        }

        Command _staffRejectCommand;
        public Command StaffRejectCommand
        {
            get
            {
                return _staffRejectCommand ?? (_staffRejectCommand = new Command(async (visit) =>await ExecuteStaffRejectCommand(visit), (visit) =>
                {
                    return SystemViewModel.Instance.HasNetworkConnection && !IsBusy && !IsClockOutDone && !IsClockInDone && WorkflowStatus != ServiceVisitWorkflowStatus.Completed &&
                        WorkflowStatus != ServiceVisitWorkflowStatus.CompletedWithoutClockIn && WorkflowStatus != ServiceVisitWorkflowStatus.Discarded && WorkflowStatus != ServiceVisitWorkflowStatus.InProgress &&
                           CurrentStaffStatus != VisitStaffAssignmentStatus.Rejected&& CurrentStaffStatus != VisitStaffAssignmentStatus.RemovedByProvider;
                }));
            }
        }
   
        public async Task ExecuteStaffRejectCommand(object visit)
        {
            if (IsBusy)
                return;

            if (visit!=null)
            {
                IsBusy = true;
                try
                {
                    var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;



                    //call back end service to update status
                    var shiftDto = AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(this);
                    var response =
                        await schedulerDataService.StaffReject(shiftDto, SystemViewModel.Instance.HasNetworkConnection);
                    if (response.ModelObject != null)
                    {
                        if (!string.IsNullOrWhiteSpace(CurrentStaffId) && VisitStaffs != null)
                        {
                            foreach (var staff in VisitStaffs)
                            {
                                if (staff.StaffId.Equals(CurrentStaffId, StringComparison.OrdinalIgnoreCase))
                                {

                                    staff.Status = VisitStaffAssignmentStatus.Rejected;
                                    break;
                                }
                            }
                        }
                        StaffAcceptanceStatus = AcceptanceStatus.Rejected;
                        WorkflowStatus.Name = ServiceVisitWorkflowStatus.RejectedByStaff.Name;
                        WorkflowStatus.DisplayName = ServiceVisitWorkflowStatus.RejectedByStaff.DisplayName;
                    }
                    else
                    {
                        var errorMessage = response.ErrorMessage;
                        await App.CurrentApp.MainPage.DisplayAlert("Rejecting Shift Failed", errorMessage, "Ok");

                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.Message;
                    await App.CurrentApp.MainPage.DisplayAlert("Rejecting Shift Failed", errorMessage, "Ok");
                }

                finally
                {
                    IsBusy = false;

                    RaiseCommandsChangeCanExecuteEvent();
                    Refresh();
                }
            
              

            }
            else
            {
                if (ParentContainerViewModel != null && ParentContainerViewModel.Navigation != null)
                {
                    await ParentContainerViewModel.Navigation.PushAsync(new ScheduleRejectPage() { BindingContext = this, Title = TextResources.Schedule_Reject });
                }
                else
                {
                    await App.GetCurrentDetailPageNavigation().PushAsync(new ScheduleRejectPage() { BindingContext = this, Title = TextResources.Schedule_Reject });
                }
            }
        }

        Command _clientAcceptCommand;
        public Command ClientAcceptCommand
        {
            get { return _clientAcceptCommand ?? (_clientAcceptCommand = new Command(() => ExecuteClientAcceptCommand(), () =>
            {
                return !IsBusy && !IsClockOutDone && !IsClockInDone && WorkflowStatus != ServiceVisitWorkflowStatus.Completed &&
                        WorkflowStatus != ServiceVisitWorkflowStatus.CompletedWithoutClockIn && WorkflowStatus != ServiceVisitWorkflowStatus.Discarded && WorkflowStatus != ServiceVisitWorkflowStatus.InProgress &&
                       ClientAcceptanceStatus != AcceptanceStatus.Accepted&& ClientAcceptanceStatus != AcceptanceStatus.Rejected;
            })); }
        }

        async void ExecuteClientAcceptCommand()
        {
            if (IsBusy )
                return;

            IsBusy = true;
            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;


            ClientAcceptanceStatus = AcceptanceStatus.Accepted;
            MarkShiftUnsynced();
            //call back end service to update status
            

            IsBusy = false;
            RaiseCommandsChangeCanExecuteEvent();
            Refresh();
        }
        Command _clientRejectCommand;
        public Command ClientRejectCommand
        {
            get { return _clientRejectCommand ?? (_clientRejectCommand = new Command(() => ExecuteClientRejectCommand(), () =>
            {
                return !IsBusy && !IsClockOutDone && !IsClockInDone && WorkflowStatus != ServiceVisitWorkflowStatus.Completed &&
                       WorkflowStatus != ServiceVisitWorkflowStatus.CompletedWithoutClockIn && WorkflowStatus != ServiceVisitWorkflowStatus.Discarded && WorkflowStatus != ServiceVisitWorkflowStatus.InProgress &&
                       ClientAcceptanceStatus != AcceptanceStatus.Rejected;
            })); }
        }

        async void ExecuteClientRejectCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;

            ClientAcceptanceStatus = AcceptanceStatus.Accepted;
            MarkShiftUnsynced();
            WorkflowStatus = ServiceVisitWorkflowStatus.RejectedByClient;
            //call back end service to update status
            
            IsBusy = false;
            RaiseCommandsChangeCanExecuteEvent();
            Refresh();
        }
        Command _clockInCommand;
        public Command ClockInCommand
        {
            get { return _clockInCommand ?? (_clockInCommand = new Command(async () => await ExecuteClockInCommand(), () =>
            {
                return !IsBusy && !IsClockInDone && (StaffAcceptanceStatus == AcceptanceStatus.Accepted || StaffAcceptanceStatus == AcceptanceStatus.NotRequired) &&
                       (ClientAcceptanceStatus ==AcceptanceStatus.Accepted || ClientAcceptanceStatus == AcceptanceStatus.NotRequired)&&
                       (IsClockOutDone || !NeedReasonForLateOrEarly||NeedReasonForLateOrEarly && HasValidReason(ReasonCommentCategory.LateOrEarlyReasonCategory));
            })); }
        }

        public async Task ExecuteClockInCommand()
        {
            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;

            if (SystemViewModel.Instance.HasNetworkConnection && IsUnsynced)
            {
                await schedulerDataService.Synchronize(AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(this));
            }
            IsVisitLate = NeedReasonForLateOrEarly;
            
            ClockInTime = DateTimeOffset.Now;
            if(ClockInDate.HasValue && TimeIn.HasValue)
            {
                ClockInTime = new DateTimeOffset(ClockInDate.Value.Year, ClockInDate.Value.Month,
                       ClockInDate.Value.Day, TimeIn.Value.Hours, TimeIn.Value.Minutes, 0, DateTimeOffset.Now.Offset);
            }
            OnPropertyChanged("TimeIn");
            IsClockInDone = true;
            MarkShiftUnsynced();
            ClockInAddress = "Unknown";//current address. call google api?
            try
            {
                Geocoder geoCoder = new Geocoder();

                var addressList = await geoCoder.GetAddressesForPositionAsync(new Position(SystemViewModel.Instance.CurrentLatitude, SystemViewModel.Instance.CurrentLongitude));
                ClockInAddress = addressList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                SystemViewModel.Instance.ErrorMessage = "Cannot get current address, will use default value.";
            }
            if (string.IsNullOrWhiteSpace(ClockInAddress))
            {
                ClockInAddress = "Unknown:" + SystemViewModel.Instance.CurrentLatitude + ", " +
                          SystemViewModel.Instance.CurrentLongitude;
            }
            ClockInLatitude = SystemViewModel.Instance.CurrentLatitude;
            ClockInLongitude = SystemViewModel.Instance.CurrentLongitude;
            ///TODO: remove demo code
            ClockInLatitude = 39.275279;
            ClockInLongitude = -76.850376;
            //
            if (IsClockOutDone)
            {
                WorkflowStatus = ServiceVisitWorkflowStatus.Completed;
            }
            else
            {
                WorkflowStatus = ServiceVisitWorkflowStatus.InProgress;
            }
            //TODO:Clockin 
            var shiftDto = AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(this);
            try
            {
                var clockInResponse =
                    await schedulerDataService.ClockIn(shiftDto, SystemViewModel.Instance.HasNetworkConnection);
                if (clockInResponse.ModelObject != null)
                {
                    await App.CurrentApp.MainPage.DisplayAlert("Clock In Successful", "You have successfully clocked in.", "Ok");

                }
                else
                {
                    IsClockInDone = false;
                    var errorMessage = clockInResponse.ErrorMessage;
                    await App.CurrentApp.MainPage.DisplayAlert("Clock In Failed", errorMessage, "Ok");
                    
                }

            }
            catch (Exception ex)
            {
                IsClockInDone = false;
                var errorMessage = "Couldn't clock in. Exceptions: " + ex;
                await App.CurrentApp.MainPage.DisplayAlert("Clock In Failed", errorMessage, "Ok");
            }
            Refresh();
            OnPropertyChanged("ClockInTime");
        }

        Command _clockOutCommand;
        public Command ClockOutCommand
        {
            get
            {
                return _clockOutCommand ?? (_clockOutCommand = new Command(async () =>await ExecuteClockOutCommand(), () =>
                {
                    return !IsBusy && !IsClockOutDone && (StaffAcceptanceStatus == AcceptanceStatus.Accepted || StaffAcceptanceStatus == AcceptanceStatus.NotRequired) &&
                           (ClientAcceptanceStatus == AcceptanceStatus.Accepted || ClientAcceptanceStatus == AcceptanceStatus.NotRequired)&&
                           (IsClockInDone || !NeedReasonForLateOrEarly || NeedReasonForLateOrEarly && HasValidReason(ReasonCommentCategory.LateOrEarlyReasonCategory));
                }));
            }
        }

        public async Task ExecuteClockOutCommand()
        {

            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;           
            if (SystemViewModel.Instance.HasNetworkConnection &&IsUnsynced)
            {
                var response = await schedulerDataService.Synchronize(AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(this));
                if (response.ModelObject != null)
                {
                    this.ActionType = response.ModelObject.ActionType;
                    this.IsUnsynced = response.ModelObject.IsUnsynced;
                    this.WorkflowStatus = response.ModelObject.WorkflowStatus;
                }
                else {
                    System.Diagnostics.Debug.WriteLine("Synchronization Failed When Clocking Out.");

                }
            }
            IsVisitLate = NeedReasonForLateOrEarly;
            ClockOutTime = DateTimeOffset.Now;
            if(ClockOutDate.HasValue&& TimeOut.HasValue)
            {
              ClockOutTime = new DateTimeOffset(ClockOutDate.Value.Year, ClockOutDate.Value.Month,
                ClockOutDate.Value.Day, TimeOut.Value.Hours, TimeOut.Value.Minutes, 0, DateTimeOffset.Now.Offset);               
            }
            OnPropertyChanged("TimeOut");
            if (IsUnscheduled)
            {
                ServiceEndDateTime = ClockOutTime;
            }
            IsClockOutDone = true;
            MarkShiftUnsynced();
            ClockOutAddress = "Unknown";//current address. call google api?
            try
            {
                Geocoder geoCoder = new Geocoder();
                var addressList = await geoCoder.GetAddressesForPositionAsync(new Position(SystemViewModel.Instance.CurrentLatitude, SystemViewModel.Instance.CurrentLongitude));
                ClockOutAddress = addressList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                SystemViewModel.Instance.ErrorMessage = "Cannot get current address, will use default value.";
            }
            if (string.IsNullOrWhiteSpace(ClockOutAddress))
            {
                ClockOutAddress = "Unknown:" + SystemViewModel.Instance.CurrentLatitude + ", " +
                                 SystemViewModel.Instance.CurrentLongitude;
            }
            ClockOutLatitude = SystemViewModel.Instance.CurrentLatitude;
            ClockOutLongitude = SystemViewModel.Instance.CurrentLongitude;
            ///TODO: remove demo code
            ClockInLatitude = 39.275279;
            ClockInLongitude = -76.850376;
            //
            if (IsClockInDone)
            {
                WorkflowStatus = ServiceVisitWorkflowStatus.Completed;
            }
            else
            {
                WorkflowStatus = ServiceVisitWorkflowStatus.CompletedWithoutClockIn;
            }

            //TODO:Clockout 

            var shiftDto = AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(this);
            var clockOuRresponse = await schedulerDataService.ClockOut(shiftDto, SystemViewModel.Instance.HasNetworkConnection);
            if (clockOuRresponse.ModelObject != null)
            {
                await App.CurrentApp.MainPage.DisplayAlert("Clock Out Successful", "You have successfully clocked out.", "Ok");

                Refresh();
                OnPropertyChanged("ClockOutTime");
                OnPropertyChanged("ServiceEndDateTime");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Clocking Out Failed.");
                var errorMessage = clockOuRresponse.ErrorMessage;
                await App.CurrentApp.MainPage.DisplayAlert("Clock Out faied", errorMessage, "Ok");
                IsClockOutDone = false;
                return;
            }
        }

        #endregion

        #region PrivateOrInternalUseOnly
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            ClientAcceptCommand.ChangeCanExecute();
            ClientRejectCommand.ChangeCanExecute();
            StaffRejectCommand.ChangeCanExecute();
            StaffAcceptCommand.ChangeCanExecute();
            ClockOutCommand.ChangeCanExecute();
            ClockInCommand.ChangeCanExecute();
            UnsyncedDeleteCommand.ChangeCanExecute();
        }
        public  bool HasValidReason(string category)
        {
            var currentStaff = CurrentStaff();
            if (currentStaff != null )
            {
                if (currentStaff.StaffRejectLateReason != null && 
                    currentStaff.StaffRejectLateReason.Category== category)
                {
                    return !string.IsNullOrWhiteSpace(currentStaff.StaffRejectLateReason.Content);
                }
                return false;
            }

            return false;
        }

        public VisitStaffDto CurrentStaff()
        {
            if (_currentStaff != null)
                return _currentStaff;
            if (!string.IsNullOrWhiteSpace(CurrentStaffId) && VisitStaffs != null)
            {
                _currentStaff = VisitStaffs.FirstOrDefault(x => x.StaffId.Equals(CurrentStaffId, StringComparison.OrdinalIgnoreCase));
            }
            return _currentStaff;
        }

        public void PropertiesChanged()
        {
            OnPropertyChanged("ShowOtherLateOrEarlyReason");
            OnPropertyChanged("OtherReasonForLateOrEarly");
            OnPropertyChanged("ShowOtherStaffRejectionReason");
            OnPropertyChanged("OtherStaffRejectionReason");
            OnPropertyChanged("ClockInEnable");
            OnPropertyChanged("ClockOutEnable");
        }
 

        private void Refresh(bool updateData=false )
        {
            if (ParentContainerViewModel != null)
            {
                if (ParentContainerViewModel is CalenderViewModel)
                {
                    var calenderViewModel = ParentContainerViewModel as CalenderViewModel;
                    if (updateData)
                    {
                        calenderViewModel.Refresh();
                    }
                    else
                    {
                        calenderViewModel.UpdateAppointmentCountPerDay();
                    }
                }
                else if (ParentContainerViewModel is TodayShiftsViewModel)
                {
                    var todayViewModel = ParentContainerViewModel as TodayShiftsViewModel;
 
                    todayViewModel.Refresh();
                }

            }
        }

        private void MarkShiftUnsynced()
        {
            if (!SystemViewModel.Instance.HasNetworkConnection)
            {
                IsUnsynced = true;
            }
        }


        #endregion

        private bool _clockInDone;
        private const string ClockInDonePropertyName = "IsClockInDone";
        private bool _clockOutDone;
        private const string ClockOutDonePropertyName = "IsClockOutDone";

        private AcceptanceStatus _staffAcceptanceStatus;
        private const string StaffAcceptanceStatusPropertyName = "StaffAcceptanceStatus";
        private AcceptanceStatus _clientAcceptanceStatus;
        private const string ClientAcceptanceStatusPropertyName = "ClientAcceptanceStatus";
        private string _otherReasonForLateOrEarly;
        private const string OtherReasonForLateOrEarlyPropertyName = "OtherReasonForLateOrEarly";
        private ObservableCollection<ReasonsCommentsDto> _notes = new ObservableCollection<ReasonsCommentsDto>();
        public const string NotesPropertyName = "Notes";
        private string _otherStaffRejectionReason;
        private const string OtherStaffRejectionReasonPropertyName = "OtherStaffRejectionReason";
        private WorkflowStatusDto _workflowStatus;
        public const string WorkflowStatusPropertyName = "WorkflowStatus";

        private VisitStaffDto _currentStaff;

        private readonly LocationTrackingRepository _locationTrackingOperation = new LocationTrackingRepository();
        private readonly LocationRepository _locationOperation = new LocationRepository();
    }
}
