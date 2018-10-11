using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.Extensions;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Schedules
{
    public class TodayShiftsViewModel : BaseViewModel
    {

        public TodayShiftsViewModel()
        {
            PastDueScheduleItems = new ObservableCollection<ScheduleViewModel>();
            UpcomingScheduleItems = new ObservableCollection<ScheduleViewModel>();
            IsBusy = false;
            IsShiftsEmpty = true;
            TotalShifts = 0;
        }

        public ObservableCollection<ScheduleViewModel> PastDueScheduleItems { get; set; }
        public ObservableCollection<ScheduleViewModel> UpcomingScheduleItems { get; set; }

        public int TotalShifts { get; set; }
        public bool IsShiftsEmpty { get; set; }
        #region Commands


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
              //  VisitStaffId = SystemViewModel.Instance.CurrentStaffId
            };

            ServiceVisitsWithPaginationResultDto serviceVisitResult=null;
            var errorMessage = "";
            Task.Run( () =>
            {
                try
                {
                    var response =  schedulerDataService.DownloadMyFurtureSchedules(searchCriteria, SystemViewModel.Instance.HasNetworkConnection).Result;
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
                                ProcedureCode = "T1019",
                                Modifiers = "U1,U2",
                                ServiceName = "Home Community Supports",
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
                                WorkflowStatus = ServiceVisitWorkflowStatus.CompletedWithoutClockIn,
                                IsUnsynced = true

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

                      /*  var scheduledemo = new ScheduleViewModel()
                        {
                            ParentContainerViewModel = this,
                            ProviderName = "Provider 1",
                            ProviderMaNumber = "1234567890",

                            ServiceStartDateTime = DateTimeOffset.Now.AddHours(1),
                            ServiceEndDateTime = DateTimeOffset.Now.AddHours(2),
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
                        scheduleItems.Add(scheduledemo);*/
                        ////////////////
                        totalShifts = scheduleItems.Count;
                        UpcomingScheduleItems.Clear();
                        UpcomingScheduleItems.AddRange(scheduleItems.Where(x =>
                            x.ServiceStartDateTime.HasValue && x.ServiceStartDateTime.Value.DateTime >= DateTime.Now ||
                            !x.ServiceStartDateTime.HasValue &&
                            (!x.IsClockInDone ||
                            !x.IsClockOutDone)));

                        PastDueScheduleItems.Clear();
                        PastDueScheduleItems.AddRange(scheduleItems.Where(x =>
                            x.ServiceStartDateTime.HasValue && x.ServiceStartDateTime.Value.DateTime < DateTime.Now ||
                            !x.ServiceStartDateTime.HasValue &&
                            x.IsClockInDone &&
                            x.IsClockOutDone));
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

            }
            });
            IsBusy = false;
        }
        #endregion  
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            LoadSchedulesCommand.ChangeCanExecute();
        }

        public void Refresh()
        {
            LoadSchedulesCommand.Execute(null);
        }
    }
}
