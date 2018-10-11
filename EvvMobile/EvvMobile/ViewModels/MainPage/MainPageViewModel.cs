using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Charts;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Messages;
using EvvMobile.ViewModels.Systems;
using Evv.Message.Portable.Schedulers.Identifiers;
using Xamarin.Forms;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.Extensions;
namespace EvvMobile.ViewModels.MainPage
{
    public class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel()
        {
            NewMessageList=new ObservableCollection<MessageViewModel>();
            TodayScheduleItems = new ObservableCollection<ScheduleViewModel>();
            IsShiftsEmpty = true;
        }
        #region Properties
        public int TodayShiftsCount { get; set; }

        public int NewShiftsCount { get; set; }

        public int UnsyncedShiftsCount { get; set; }
        public int NotificationsCount { get; set; }
        public string StaffFullName { get; set; }
        public ObservableCollection<MessageViewModel> NewMessageList { get; set; }
        private const string NumberOfNewMessagesPropertyName = "NumberOfNewMessages";
        private int _numberOfNewMessages;
        public int NumberOfNewMessages
        {
            get { return _numberOfNewMessages; }
            set { SetProperty(ref _numberOfNewMessages, value, NumberOfNewMessagesPropertyName); }
        }
        private const string FirstMessagePropertyName = "FirstMessageTitle";
        private string _firstMessage;
        public string FirstMessageTitle
        {
            get { return _firstMessage; }
            set { SetProperty(ref _firstMessage, value, FirstMessagePropertyName); }
        }
        #endregion
        public Chart TwelveMonthLineChart { get; set; }
        public ObservableCollection<ScheduleViewModel> TodayScheduleItems { get; set; }
        private ScheduleViewModel _firstShift;
        private const string FirstShiftPropertyName = "FirstShift";
        public ScheduleViewModel FirstShift
        {
            get { return _firstShift; }
            set { SetProperty(ref _firstShift, value, FirstShiftPropertyName); }
        }
        public int TotalShifts { get; set; }
        public bool IsShiftsEmpty { get; set; }

        #region commands


        Command _loadNewMessagesCommand;

        public Command LoadNewMessagesCommand
        {
            get
            {
                return _loadNewMessagesCommand ?? (
                           _loadNewMessagesCommand = new Command(async () =>
                                   await ExecuteLoadNewMessagesCommand(),
                               () => !IsBusy && SystemViewModel.Instance.HasNetworkConnection));
            }
        }

        public async Task ExecuteLoadNewMessagesCommand()
        {
            if (IsBusy || !SystemViewModel.Instance.HasNetworkConnection)
                return;

            IsBusy = true;
            IsInitialized = true;
            LoadNewMessagesCommand.ChangeCanExecute();
            try
            {
                //get new messages
                //  NewMessageList.Clear();
                //TODO:get new messages from DB

                NewMessageList = new ObservableCollection<MessageViewModel>(NewMessageList.Where(x => !x.IsViewed));
                NewMessageList.Add(new MessageViewModel
                {
                    MessageTitle = "Assignment Notification",
                    SenderName = "Provider Admin",
                    SentDateTime = DateTimeOffset.Now.AddDays(-1),
                    Message =
                        "A new shift assignment is assigned to you.Using advanced encryption, GPS, cellular and Wi-Fi technology, Axxess' Electronic Visit Verification records the date, time and location of home care visits using phones or tablets on both the Apple iOS or Google Android platforms. Our mobile apps are fully integrated with our software and automatically sync in real time!"
                });
                NewMessageList.Add(new MessageViewModel
                {
                    MessageTitle = "Reminds",
                    SenderName = "System",
                    SentDateTime = DateTimeOffset.Now.AddDays(-3),
                    Message = "You have a planned visit tomorrow."
                });
                NumberOfNewMessages = NewMessageList.Count;
                var firstMessage = NewMessageList.FirstOrDefault();
                if(firstMessage!=null)
                {
                    FirstMessageTitle = firstMessage.MessageTitle;
                    if(string.IsNullOrWhiteSpace(firstMessage.MessageTitle))
                    {
                        FirstMessageTitle = firstMessage.Message;
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                OnPropertyChanged("NewMessageList");
                IsBusy = false;
                LoadNewMessagesCommand.ChangeCanExecute();                
            }


        }

        Command _loadSchedulesCommand;
        /// <summary>
        /// download next 7 days schedules
        /// </summary>
        public Command LoadSchedulesCommand
        {
            get { return _loadSchedulesCommand ?? (_loadSchedulesCommand = new Command(async () => await ExecuteLoadSchedulesCommand(), () => !IsBusy)); }
        }

        private async Task ExecuteLoadSchedulesCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            var totalShifts = 0;
            LoadSchedulesCommand.ChangeCanExecute();
            /////////////////////////////
            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;

            DateTime startDate = DateTimeOffset.Now.Date;

            var searchCriteria = new ServiceVisitSearchCriteriaDto
            {
                PageSize = 1000,
                PageNumber = 1,
                PeriodStart = startDate,
                PeriodEnd = startDate.AddDays(1).AddMilliseconds(-1),
               // VisitStaffId = SystemViewModel.Instance.CurrentStaffId
            };

            ServiceVisitsWithPaginationResultDto serviceVisitResult = null;
            var errorMessage = "";
            Task.Run(() =>
            {
                try
                {
                    var response = schedulerDataService.DownloadMyFurtureSchedules(searchCriteria, SystemViewModel.Instance.HasNetworkConnection).Result;
                    if (response.ModelObject != null)
                    {
                        serviceVisitResult = response.ModelObject;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }

            }).Wait();
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                await App.CurrentApp.MainPage.DisplayAlert("Couldn't get today's shifts.", errorMessage, "Ok");

            }

            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {

                    if (serviceVisitResult != null)
                    {
                        var shifts =
                            serviceVisitResult.ServiceVisitDtos.Where(s => s.ServiceStartDateTime >= searchCriteria.PeriodStart &&
                                                                                        s.ServiceStartDateTime <= searchCriteria.PeriodEnd &&
                                                                                        s.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                                                        s.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                                                        s.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByStaff &&
                                                                                        s.WorkflowStatus != ServiceVisitWorkflowStatus.Discarded &&
                                                                                        s.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByClient &&
                                                                                        s.WorkflowStatus != ServiceVisitWorkflowStatus.DeniedByMmis)
                                                                                        .Select(x =>
                                                                                        {
                                                                                            var ss = AutoMapper.Mapper.Map<ServiceVisitDto, ScheduleViewModel>(x);
                                                                                            ss.ParentContainerViewModel = this;
                                                                                            return ss;
                                                                                        }
                                                                                        ).OrderBy(x => x.ServiceStartDateTime).ToList();
                        var scheduleItems = new ObservableCollection<ScheduleViewModel>(shifts);
                        ///////////////////test
                        if (scheduleItems.Count == 0)
                        {
                            var schedule1 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-6),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "700800900",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "A" },
                                ProcedureCode = "T1000",
                                ProcedureCodeAndModifier= "T1000_U1_U2",
                                Modifiers = "U1,U2",
                                ServiceName = "Transportation Service",
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                LocationMatched = true,
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                StaffAcceptanceStatus = AcceptanceStatus.NoResponse,
                                CurrentStaffId = "123456",
                                ClientAcceptanceStatus = AcceptanceStatus.NotRequired,
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                {
                    StaffId = "123456",
                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                } },
                                WorkflowStatus = ServiceVisitWorkflowStatus.Scheduled,
                                VisitMeasurements = new ObservableCollection<CollectClinicalData.VisitMeasurementViewModel>
                                {
                                    new CollectClinicalData.VisitMeasurementViewModel
                                    {
                                        Name = "Blood Presure",
                                        Code="CM1000",
                                        Instruction = "test",
                                        Attributes = new ObservableCollection<CollectClinicalData.MeasurementAttributeViewModel>
                                        {
                                            new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "blood presure",
                                                AttributeValue=""
                                            },
                                             new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "blood presure 2",
                                                AttributeValue=""
                                            }
                                        }
                                    },
                                     new CollectClinicalData.VisitMeasurementViewModel
                                    {
                                        Name = "Check Pulse",
                                        Code="CM1001",
                                        Instruction = "test",
                                        Attributes = new ObservableCollection<CollectClinicalData.MeasurementAttributeViewModel>
                                        {
                                            new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "Pulse",
                                                AttributeValue=""
                                            },
                                            new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "Pulse 2",
                                                AttributeValue=""
                                            }
                                        }
                                    }
                                }

                            };

                            scheduleItems.Add(schedule1);

                            var schedule2 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-1),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                    {
                                        StaffId = "123456",
                                        Status = VisitStaffAssignmentStatus.PendingAcceptance
                                    }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.Completed

                            };
                            scheduleItems.Add(schedule2);

                            var schedule6 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-4),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                {
                                    StaffId = "123456",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.ScheduleMissed

                            };
                            scheduleItems.Add(schedule6);
                            var schedule7 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-2),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                {
                                    StaffId = "123456",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.ScheduleMissed

                            };
                            scheduleItems.Add(schedule7);
                            var schedule8 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-4),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                {
                                    StaffId = "123456",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.ScheduleMissed

                            };
                            scheduleItems.Add(schedule8);
                            var schedule3 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",

                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(1),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "100200300",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "C" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2,U3",
                                ServiceName = "Personal Care Services",
                                LocationMatched = false,
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                LateOrEralyThreshold = 10,
                                CurrentStaffId = "123456",
                                ClientAcceptanceStatus = AcceptanceStatus.NotRequired,
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                            {
                                StaffId = "123456",
                                Status = VisitStaffAssignmentStatus.PendingAcceptance
                            },new VisitStaffDto
                                {
                                    StaffId = "12345678",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                } },
                                WorkflowStatus = ServiceVisitWorkflowStatus.Scheduled
                            };
                            scheduleItems.Add(schedule3);
                            var schedule4 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",

                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(3),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "100200300",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "C" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2,U3",
                                ServiceName = "Personal Care Services",
                                LocationMatched = false,
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                LateOrEralyThreshold = 10,
                                CurrentStaffId = "123456",
                                ClientAcceptanceStatus = AcceptanceStatus.NotRequired,
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                            {
                                StaffId = "123456",
                                Status = VisitStaffAssignmentStatus.Accepted
                            },new VisitStaffDto
                            {
                                StaffId = "12345678",
                                Status = VisitStaffAssignmentStatus.Accepted
                            } },
                                WorkflowStatus = ServiceVisitWorkflowStatus.Scheduled
                            };
                            scheduleItems.Add(schedule4);
                            scheduleItems = new ObservableCollection<ScheduleViewModel>(scheduleItems.OrderBy(x => x.ServiceStartDateTime));
                        }

                        ////////////////
                        totalShifts = scheduleItems.Count;
                        TodayScheduleItems.Clear();
                        TodayScheduleItems.AddRange(scheduleItems.Where(x => !x.IsClockInDone ||
                            !x.IsClockOutDone));
                    }
                    else
                    {
                        //test
                        var scheduleItems = new ObservableCollection<ScheduleViewModel>();
                        ///////////////////test
                        if (scheduleItems.Count == 0)
                        {
                            var schedule1 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-6),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "700800900",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "A" },
                                ProcedureCode = "T1000",
                                ProcedureCodeAndModifier = "T1000_U1_U2",
                                Modifiers = "U1,U2",
                                ServiceName = "Transportation Service",
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                LocationMatched = true,
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                StaffAcceptanceStatus = AcceptanceStatus.NoResponse,
                                CurrentStaffId = "123456",
                                ClientAcceptanceStatus = AcceptanceStatus.NotRequired,
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                {
                    StaffId = "123456",
                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                } },
                                WorkflowStatus = ServiceVisitWorkflowStatus.Scheduled,
                                VisitMeasurements = new ObservableCollection<CollectClinicalData.VisitMeasurementViewModel>
                                {
                                    new CollectClinicalData.VisitMeasurementViewModel
                                    {
                                        Name = "Check Blood Presure",
                                        Code="CM1000",
                                        Instruction = "Check Blood Presure ,test, 9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                        Attributes = new ObservableCollection<CollectClinicalData.MeasurementAttributeViewModel>
                                        {
                                            new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "Blood Presure",
                                                AttributeValue=""
                                            },
                                             new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "Blood Presure 2",
                                                AttributeValue=""
                                            }
                                        }
                                    },
                                     new CollectClinicalData.VisitMeasurementViewModel
                                    {
                                        Name = "Check Pulse",
                                        Code="CM1001",
                                        Instruction = "test",
                                        Attributes = new ObservableCollection<CollectClinicalData.MeasurementAttributeViewModel>
                                        {
                                            new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "Pulse",
                                                AttributeValue=""
                                            },
                                             new CollectClinicalData.MeasurementAttributeViewModel
                                            {
                                                AttributeName= "Pulse 2",
                                                AttributeValue=""
                                            }
                                        }
                                    }
                                }

                            };

                            scheduleItems.Add(schedule1);

                            var schedule2 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-1),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                    {
                                        StaffId = "123456",
                                        Status = VisitStaffAssignmentStatus.PendingAcceptance
                                    }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.Completed

                            };
                            scheduleItems.Add(schedule2);

                            var schedule6 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-4),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                {
                                    StaffId = "123456",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.ScheduleMissed

                            };
                            scheduleItems.Add(schedule6);
                            var schedule7 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-2),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                {
                                    StaffId = "123456",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.ScheduleMissed

                            };
                            scheduleItems.Add(schedule7);
                            var schedule8 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",
                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(-4),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "400500600",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "B" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "In Home Nursing Respite",
                                LocationMatched = false,
                                CurrentStaffId = "123456",
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                                {
                                    StaffId = "123456",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                }},
                                WorkflowStatus = ServiceVisitWorkflowStatus.ScheduleMissed

                            };
                            scheduleItems.Add(schedule8);
                            var schedule3 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",

                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(1),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "100200300",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "C" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2,U3",
                                ServiceName = "Personal Care Services",
                                LocationMatched = false,
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                LateOrEralyThreshold = 10,
                                CurrentStaffId = "123456",
                                ClientAcceptanceStatus = AcceptanceStatus.NotRequired,
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                            {
                                StaffId = "123456",
                                Status = VisitStaffAssignmentStatus.PendingAcceptance
                            },new VisitStaffDto
                                {
                                    StaffId = "12345678",
                                    Status = VisitStaffAssignmentStatus.PendingAcceptance
                                } },
                                WorkflowStatus = ServiceVisitWorkflowStatus.Scheduled
                            };
                            scheduleItems.Add(schedule3);
                            var schedule4 = new ScheduleViewModel()
                            {
                                ParentContainerViewModel = this,
                                ProviderName = "Provider 1",
                                ProviderMaNumber = "1234567890",

                                ServiceStartDateTime = DateTimeOffset.Now.AddHours(3),
                                ServiceEndDateTime = DateTimeOffset.Now,
                                ClientMaNumber = "100200300",
                                ClientName = new PersonNameDto { FirstName = "Client", LastName = "C" },
                                ClientPhone = new PhoneDto { Number = "(410)741-0485" },
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2,U3",
                                ServiceName = "Personal Care Services",
                                LocationMatched = false,
                                ServiceRenderAddress = "9755 Patuxent Woods Drive, Suite 300 Columbia, MD 21046",
                                LateOrEralyThreshold = 10,
                                CurrentStaffId = "123456",
                                ClientAcceptanceStatus = AcceptanceStatus.NotRequired,
                                VisitStaffs = new List<VisitStaffDto> {new VisitStaffDto
                            {
                                StaffId = "123456",
                                Status = VisitStaffAssignmentStatus.Accepted
                            },new VisitStaffDto
                            {
                                StaffId = "12345678",
                                Status = VisitStaffAssignmentStatus.Accepted
                            } },
                                WorkflowStatus = ServiceVisitWorkflowStatus.Scheduled
                            };
                            scheduleItems.Add(schedule4);
                            scheduleItems = new ObservableCollection<ScheduleViewModel>(scheduleItems.OrderBy(x => x.ServiceStartDateTime));
                        }

                        
                        totalShifts = scheduleItems.Count;
                        TodayScheduleItems.Clear();
                        TodayScheduleItems.AddRange(scheduleItems.Where(x => !x.IsClockInDone ||
                            !x.IsClockOutDone));

                        ////////////test end
                    }

                }
                finally
                {
                    IsBusy = false;
                    LoadSchedulesCommand.ChangeCanExecute();
                    TotalShifts = totalShifts;
                    IsShiftsEmpty = TotalShifts == 0;
                    OnPropertyChanged("TotalShifts");
                    OnPropertyChanged("IsShiftsEmpty");
                    var firstShift = TodayScheduleItems.FirstOrDefault();
                    FirstShift = firstShift;
                }
            });
            IsBusy = false;
        }
        #endregion
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            LoadNewMessagesCommand.ChangeCanExecute();
        }

        #region Private methods

        #endregion

        #region Fields



        #endregion
    }
}
