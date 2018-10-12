using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.Customizations.CustomControls.Calendar;
using EvvMobile.Extensions;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Schedules
{
    public class CalenderViewModel: BaseViewModel
    {
        public CalenderViewModel()
        {
            ScheduleItemsInDay = new List< IList<ScheduleViewModel> >();
            for (int i=0;i< MaxDaysInNormalView; i++ )
            {
                ScheduleItemsInDay.Add( new List<ScheduleViewModel>());
            }
            _selectedScheduleItems = new List<ScheduleViewModel>();
            _appointmentsCountPerDay = new ObservableCollection<int>();
            DaysOfLoadedSchedule = 41;


        }
        #region Properties

        public IList<IList<ScheduleViewModel>> ScheduleItemsInDay { get; set; }
        public DateTime SelectedDate { get; set; }

        private IList<ScheduleViewModel> _selectedScheduleItems;
        private const string SelectedScheduleItemsPropertyName = "SelectedScheduleItems";
        public IList<ScheduleViewModel> SelectedScheduleItems
        {
            get { return _selectedScheduleItems; }
            set { SetProperty(ref _selectedScheduleItems, value, SelectedScheduleItemsPropertyName); }
        }
        private IList<int> _appointmentsCountPerDay;
        private const string AppointmentsCountPerDayPropertyName = "AppointmentsCountPerDay";
        public IList<int> AppointmentsCountPerDay
        {
            get { return _appointmentsCountPerDay; }
            set { SetProperty(ref _appointmentsCountPerDay, value, AppointmentsCountPerDayPropertyName); }
        }

        public int DaysOfLoadedSchedule { get; set; }
        #endregion

        #region Commands
        Command _loadOneMonthSchedulesCommand;
        /// <summary>
        /// download next 7 days schedules
        /// </summary>
        public Command LoadOneMonthSchedulesCommand
        {
            get { return _loadOneMonthSchedulesCommand ?? (_loadOneMonthSchedulesCommand = new Command<DateTime>( (dateTime) =>  ExecuteLoadOneMonthSchedulesCommand(dateTime), (dateTime) => !IsBusy)); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime">should be first date of the calendar</param>
        /// <returns></returns>
        private  async Task ExecuteLoadOneMonthSchedulesCommand(DateTime dateTime)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
            var startDate = dateTime.Date;
            StartDate = startDate;

            var endDate = startDate.AddDays(DaysOfLoadedSchedule);

            var searchCriteria = new ServiceVisitSearchCriteriaDto
            {
                PageSize = 5000,
                PageNumber = 1,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                ///  VisitStaffId = "123456789"//TODO:SystemViewModel.Instance.CurrentStaffId
            };
            ServiceVisitsWithPaginationResultDto serviceVisitResult = null;
            var errorMessage = "";
            Task task = Task.Run( () =>
            {
                var response = schedulerDataService.GetSchedules(searchCriteria,
                    SystemViewModel.Instance.HasNetworkConnection).Result;
                if (response.IsFalied || response.ModelObject == null)
                {
                        errorMessage = response.ErrorMessage;
                         
                }
                else
                {
                    serviceVisitResult = response.ModelObject;
                }

            });
            try
            {
                task.Wait();
            }
            catch (Exception e)
            {
                 errorMessage = e.Message;
                //TODO: only display alert for online mode
                await App.CurrentApp.MainPage.DisplayAlert("Failed to download data", "Couldn't download shifts. Check the connection.", "Ok");

            }

            Device.BeginInvokeOnMainThread( () =>
            {
                try
                {

                    lock (locker)
                    {
                        LoadOneMonthSchedulesCommand.ChangeCanExecute();
                        /////////////////////////////
                        SelectedScheduleItems = new List<ScheduleViewModel>();
                        for (int i = 0; i < MaxDaysInNormalView; i++)
                        {
                            ScheduleItemsInDay[i].Clear();
                        }

                        if (serviceVisitResult != null)
                        {
                            var scheduledShifts = new List<ScheduleViewModel>(serviceVisitResult
                                .ServiceVisitDtos
                                .Where(x => x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                            x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                            x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByStaff &&
                                            x.WorkflowStatus != ServiceVisitWorkflowStatus.Discarded &&
                                            x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByClient &&
                                            x.WorkflowStatus != ServiceVisitWorkflowStatus.DeniedByMmis)
                                .Select(x =>
                                {
                                    var ss = AutoMapper.Mapper.Map<ServiceVisitDto, ScheduleViewModel>(x);
                                    ss.ParentContainerViewModel = this;
                                    return ss;
                                }));
                            //fill ScheduleItemsInDay
                            for (int i = 0; i < MaxDaysInNormalView; i++)
                            {
                                var serviceDate = startDate.AddDays(i).Date;
                                var shifts = scheduledShifts
                                    .Where(x => x.ServiceStartDateTime.HasValue &&
                                                x.ServiceStartDateTime.Value.Date == serviceDate)
                                    .ToList();
                                if (shifts.Any())
                                    ScheduleItemsInDay[i].AddRange(shifts);
                                if (serviceDate.Date == DateTime.Now.Date)
                                {
                                    SelectedScheduleItems.AddRange(ScheduleItemsInDay[i]);
                                }
                            }

                        }
                        UpdateAppointmentCountPerDay();
                    }

                }
                catch (Exception e)
                {

                }
                finally
                {
                    LoadOneMonthSchedulesCommand.ChangeCanExecute();
                    if ( DateTime.Now>=dateTime.Date && DateTime.Now<= dateTime.Date.AddDays(41))
                    {
                        SelectedDate = DateTime.Now;
                        OnPropertyChanged("SelectedDate");                        
                    }

                }
            });
            IsBusy = false;
        }
        public IList<ScheduleViewModel> GetSelectedDateItems(DateTimeEventArgs dateTimeEventArg)
        {
            lock (locker)
            {
                try
                {
                    SelectedScheduleItems = new List<ScheduleViewModel>();
                if (dateTimeEventArg != null)
                {
                    var selectedIndex = dateTimeEventArg.SelectedIndex;

                    if (selectedIndex >= 0 & selectedIndex < MaxDaysInNormalView)
                    {
                        var selectedItems = new List<ScheduleViewModel>(ScheduleItemsInDay[selectedIndex].Where(x =>x!=null&&
                                    x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                    x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                    x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByStaff &&
                                    x.WorkflowStatus != ServiceVisitWorkflowStatus.Discarded &&
                                    x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByClient &&
                                    x.WorkflowStatus != ServiceVisitWorkflowStatus.DeniedByMmis).ToList());
                            SelectedScheduleItems= selectedItems;
                    }
                                
                }
                           

            }
            catch (Exception e)
            {
                               
            }
            }
            return SelectedScheduleItems;
        }
        #endregion

        public void UpdateAppointmentCountPerDay()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (locker)
                {
                    var appointmentCounts = new ObservableCollection<int>();
                    for (int i = 0; i < MaxDaysInNormalView; i++)
                    {
                        if (ScheduleItemsInDay[i] != null)
                        {
                            ScheduleItemsInDay[i] = ScheduleItemsInDay[i].Where(x =>
                                x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByStaff &&
                                x.WorkflowStatus != ServiceVisitWorkflowStatus.Discarded &&
                                x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByClient &&
                                x.WorkflowStatus != ServiceVisitWorkflowStatus.DeniedByMmis).ToList();
                            appointmentCounts.Add(ScheduleItemsInDay[i].Count);
                        }
                        else
                        {
                            appointmentCounts.Add(0);
                        }

                    }

                    AppointmentsCountPerDay = appointmentCounts;
                }
            });

        }

        public void Refresh()
        {
            if (StartDate.HasValue)
            {
                ExecuteLoadOneMonthSchedulesCommand(StartDate.Value);
            }
        }
        private DateTime? StartDate;
        private static object locker = new object();
        private  const int MaxDaysInNormalView = 42;
    }
}
