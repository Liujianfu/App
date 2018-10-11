using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.Customizations;
using EvvMobile.Localization;
using EvvMobile.Pages.Schedules;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Schedules
{
    public class ScheduleListViewModel : BaseViewModel
    {

        public ScheduleListViewModel()
        {
  
            ScheduledItems = new List<ScheduleViewModel>();
            OffScheduleItems = new List<ScheduleViewModel>();
            NewItems = new List<ScheduleViewModel>();
            UnsyncedItems = new List<ScheduleViewModel>();
            SchedulededShiftsGroups = new ObservableCollection<Grouping<ScheduleViewModel, string>>();

            UnsyncedShiftsGroups = new ObservableCollection<Grouping<ScheduleViewModel, string>>();
            NewShiftsGroups = new ObservableCollection<Grouping<ScheduleViewModel, string>>();
            OffScheduleShiftsGroups = new ObservableCollection<Grouping<ScheduleViewModel, string>>();
            IsBusy = false;
        }

       #region properties
        public IList<ScheduleViewModel> ScheduledItems { get;  set; }
        public IList<ScheduleViewModel> OffScheduleItems { get; set; }
        public IList<ScheduleViewModel> NewItems { get; set; }
        public IList<ScheduleViewModel> UnsyncedItems { get; set; }
        public ObservableCollection<Grouping<ScheduleViewModel, string>> SchedulededShiftsGroups { get; set; }
        public ObservableCollection<Grouping<ScheduleViewModel, string>> UnsyncedShiftsGroups { get; set; }
        public ObservableCollection<Grouping<ScheduleViewModel, string>> NewShiftsGroups { get; set; }
        public ObservableCollection<Grouping<ScheduleViewModel, string>> OffScheduleShiftsGroups { get; set; }
        public bool EnableSyncButton
        {
            get { return !IsBusy && SystemViewModel.Instance.HasNetworkConnection && UnsyncedShiftsGroups != null && UnsyncedShiftsGroups.Count > 0; }
        }
        #endregion

        #region Commands
        Command _loadSheduledShiftsCommand;
        public Command LoadSheduledShiftsCommand
        {
            get { return _loadSheduledShiftsCommand ?? (_loadSheduledShiftsCommand =
                    new Command(async () =>
                    {
                        if (IsBusy)
                            return;
                        IsBusy = true;
                        LoadSheduledShiftsCommand.ChangeCanExecute();
                        try
                        {
                            var shifts =await LoadCurrentCycleSchedules();
                            UpdateScheduldGroup(shifts);
                        }
                        catch (Exception e)
                        {
                            await App.CurrentApp.MainPage.DisplayAlert("Loading shifts failed", e.Message, "Ok");
                        }
                        IsBusy = false;
                        LoadSheduledShiftsCommand.ChangeCanExecute();
                    }, () => !IsBusy)); }
        }

        public void UpdateScheduldGroup(IList<ScheduleViewModel> shifts)
        {
            var inputs = shifts!=null? shifts: ScheduledItems;
            
            ScheduledItems = inputs.Where(x => (x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.Scheduled.Name) ||
                                                          x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.Completed.Name) ||
                                                          x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.CompletedWithoutClockIn.Name) ||
                                                          x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.InProgress.Name) ||
                                                          x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.ScheduleMissed.Name)) &&
                                                          x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                          x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                          !x.IsUnscheduled).ToList();
            lock (locker)
            {
                Device.BeginInvokeOnMainThread(() => {
                    SchedulededShiftsGroups.Clear();
                    foreach (var sheduledShift in ScheduledItems)
                    {
                        sheduledShift.ParentContainerViewModel = this;
                        if (sheduledShift.ServiceStartDateTime.HasValue)
                        {
                            var key = sheduledShift.ServiceStartDateTime.Value.ToString("D");

                            var existingGroup = FindExistingGroup(SchedulededShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(sheduledShift);
                            }

                            else
                            {
                                var items = new ObservableCollection<ScheduleViewModel>();
                                items.Add(sheduledShift);
                                var newGroup = new Grouping<ScheduleViewModel, string>(items, key);
                                SchedulededShiftsGroups.Add(newGroup);
                            }
                        }

                    }
                });
            }
        }
        Command _loadOffScheduleShiftsCommand;
        public Command LoadOffSheduleShiftsCommand
        {
            get
            {
                return _loadOffScheduleShiftsCommand ?? (_loadOffScheduleShiftsCommand =
                  new Command(async () =>
                  {
                      if (IsBusy)
                          return;
                      IsBusy = true;
                      LoadOffSheduleShiftsCommand.ChangeCanExecute();
                      try
                      {
                         var shifts = await LoadCurrentCycleSchedules();
                          UpdateOffScheduleGroup(shifts);
                      }
                      catch (Exception e)
                      {
                          await App.CurrentApp.MainPage.DisplayAlert("Loading shifts failed", e.Message, "Ok");
                      }
                      IsBusy = false;
                      LoadOffSheduleShiftsCommand.ChangeCanExecute();
                  }, () => !IsBusy));
            }
        }
        public void UpdateOffScheduleGroup(IList<ScheduleViewModel> shifts)
        {
            var inputs = shifts != null ? shifts : OffScheduleItems;

            lock (locker)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    OffScheduleShiftsGroups.Clear();
                    OffScheduleItems = inputs.Where(x => x.IsUnscheduled &&
                                                              x.StaffAcceptanceStatus !=
                                                              AcceptanceStatus.Rejected &&
                                                              x.ClientAcceptanceStatus !=
                                                              AcceptanceStatus.Rejected &&
                                                              x.WorkflowStatus != ServiceVisitWorkflowStatus
                                                                  .RejectedByStaff &&
                                                              x.WorkflowStatus != ServiceVisitWorkflowStatus
                                                                  .Discarded &&
                                                              x.WorkflowStatus != ServiceVisitWorkflowStatus
                                                                  .RejectedByClient &&
                                                              x.WorkflowStatus != ServiceVisitWorkflowStatus
                                                                  .DeniedByMmis).ToList();
                    foreach (var shift in OffScheduleItems)
                    {
                        shift.ParentContainerViewModel = this;
                        if (shift.ServiceStartDateTime.HasValue)
                        {
                            var key = shift.ServiceStartDateTime.Value.ToString("D");
                            var existingGroup = FindExistingGroup(OffScheduleShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(shift);
                            }
                            else
                            {
                                var itmes = new ObservableCollection<ScheduleViewModel>();
                                itmes.Add(shift);
                                var newGroup = new Grouping<ScheduleViewModel, string>(itmes, key);
                                OffScheduleShiftsGroups.Add(newGroup);
                            }
                        }
                    }
                });
            }
        }

        Command _loadNewScheduleShiftsCommand;
        public Command LoadNewSheduleShiftsCommand
        {
            get
            {
                return _loadNewScheduleShiftsCommand ?? (_loadNewScheduleShiftsCommand =
                  new Command(async () =>
                  {
                      if (IsBusy)
                          return;
                      IsBusy = true;
                      LoadNewSheduleShiftsCommand.ChangeCanExecute();
                      try
                      {
                          var shifts = await LoadCurrentCycleSchedules();
                          UpdateNewGroup(shifts);

                      }
                      catch (Exception e)
                      {
                          await App.CurrentApp.MainPage.DisplayAlert("Loading shifts failed", e.Message, "Ok");
                      }
                      IsBusy = false;
                      LoadNewSheduleShiftsCommand.ChangeCanExecute();
                  }, () => !IsBusy));
            }
        }
        public void UpdateNewGroup(IList<ScheduleViewModel> shifts)
        {
            var inputs = shifts != null ? shifts : NewItems;

            lock (locker)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NewShiftsGroups.Clear();
                    NewItems = inputs.Where(x =>
                        x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.Initiated.Name) &&
                        x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                        x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                        !x.IsUnscheduled).ToList();
                    foreach (var newShift in NewItems)
                    {
                        newShift.ParentContainerViewModel = this;
                        if (newShift.ServiceStartDateTime.HasValue)
                        {
                            var key = newShift.ServiceStartDateTime.Value.ToString("D");

                            var existingGroup = FindExistingGroup(NewShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(newShift);
                            }
                            else
                            {
                                var items = new ObservableCollection<ScheduleViewModel>();
                                items.Add(newShift);
                                var newGroup = new Grouping<ScheduleViewModel, string>(items, key);
                                NewShiftsGroups.Add(newGroup);
                            }
                        }
                    }
                });
            }
        }
        Command _loadUnsyncScheduleShiftsCommand;
        public Command LoadUnsyncSheduleShiftsCommand
        {
            get
            {
                return _loadUnsyncScheduleShiftsCommand ?? (_loadUnsyncScheduleShiftsCommand =
                  new Command(async () =>
                  {
                      if (IsBusy)
                          return;
                      IsBusy = true;
                      LoadUnsyncSheduleShiftsCommand.ChangeCanExecute();
                      try
                      {
                          var shifts = await LoadCurrentCycleSchedules();
                          UpdateUnsyncedGroup(shifts);
                      }
                      catch (Exception e)
                      {
                          await App.CurrentApp.MainPage.DisplayAlert("Loading shifts failed", e.Message, "Ok");
                      }
                      IsBusy = false;
                      LoadUnsyncSheduleShiftsCommand.ChangeCanExecute();
                  }, () => !IsBusy));
            }
        }
        public void UpdateUnsyncedGroup(IList<ScheduleViewModel> shifts)
        {
            var inputs = shifts != null ? shifts : UnsyncedItems;

            lock (locker)
            {
                Device.BeginInvokeOnMainThread(() => {
                    UnsyncedShiftsGroups.Clear();
                    UnsyncedItems = inputs.Where(x => x.IsUnsynced).ToList();
                    foreach (var unsyncedItem in UnsyncedItems)
                    {
                        unsyncedItem.ParentContainerViewModel = this;
                        if (unsyncedItem.ServiceStartDateTime.HasValue)
                        {
                            var key = unsyncedItem.ServiceStartDateTime.Value.ToString("D");
                            var existingGroup = FindExistingGroup(UnsyncedShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(unsyncedItem);
                            }
                            else
                            {
                                var itmes = new ObservableCollection<ScheduleViewModel>();
                                itmes.Add(unsyncedItem);
                                var newGroup = new Grouping<ScheduleViewModel, string>(itmes, key);
                                UnsyncedShiftsGroups.Add(newGroup);
                            }
                        }
                    }
                    OnPropertyChanged("EnableSyncButton");

                });
            }
        }
        Command _loadSchedulesCommand;
        /// <summary>
        /// download next 7 days schedules
        /// </summary>
        public Command LoadSchedulesCommand
        {
            get { return _loadSchedulesCommand ?? (_loadSchedulesCommand = new Command(async () => await ExecuteLoadSchedulesCommand(),() =>!IsBusy)); }
        }

        public  async Task ExecuteLoadSchedulesCommand()
        {
           if (IsBusy)
                return;
            var errorMessage = "";

            try
            {
                
                IsBusy = true;
                OnPropertyChanged("EnableSyncButton");
                LoadSchedulesCommand.ChangeCanExecute();
                var shifts = await LoadCurrentCycleSchedules();
                RefreshGroups(shifts);

            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    await App.CurrentApp.MainPage.DisplayAlert("Loading shifts failed", errorMessage, "Ok");

                }
                IsBusy = false;
                OnPropertyChanged("EnableSyncButton");
                LoadSchedulesCommand.ChangeCanExecute();

            }

        }

        public async Task<bool> ExecuteSyncAllOfflineShiftsCommand()
        {
 
           if (IsBusy || !SystemViewModel.Instance.HasNetworkConnection)
                    return false;
                SystemViewModel.Instance.CleanMessages();
           var allSynced = true;
           try
            {
                IsBusy = true;
                // Sync with Rest Service
                var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
                if (schedulerDataService != null)
                {

                    foreach (var group in UnsyncedShiftsGroups)
                    {
                        foreach (var shift in group)
                        {
                            var shiftDto = AutoMapper.Mapper.Map<ScheduleViewModel, ServiceVisitDto>(shift);
                            var result = await schedulerDataService.Synchronize(shiftDto);
                            if (result.ModelObject != null)
                            {
                                shift.IsUnsynced = result.ModelObject.IsUnsynced;
                                shift.ActionType = result.ModelObject.ActionType;
                            }
                            else {
                                allSynced = false;
                            }
                        }
                    }
                    IsBusy = false;
                    LoadUnsyncSheduleShiftsCommand.Execute(null);
                    return allSynced;
                }
            }
            catch (Exception)
            {
                IsBusy = false;
                return false;
            }
            IsBusy = false;
            return true;
        }
        #endregion  
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            LoadSchedulesCommand.ChangeCanExecute();
            LoadUnsyncSheduleShiftsCommand.ChangeCanExecute();
            LoadSheduledShiftsCommand.ChangeCanExecute();
            LoadNewSheduleShiftsCommand.ChangeCanExecute();
            LoadUnsyncSheduleShiftsCommand.ChangeCanExecute();
        }

        public void AddNewShift(ScheduleViewModel newShift)
        {
            if (newShift == null)
                return;
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (locker)
                {
                    newShift.ParentContainerViewModel = this;
                    if (newShift.ServiceStartDateTime.HasValue)
                    {
                        var key = newShift.ServiceStartDateTime.Value.ToString("D");
                        var existingGroup = FindExistingGroup(OffScheduleShiftsGroups, key);
                        if (existingGroup != null)
                        {
                            existingGroup.AddItem(newShift);
                        }
                        else
                        {
                            var itmes = new ObservableCollection<ScheduleViewModel>();
                            itmes.Add(newShift);
                            var newGroup = new Grouping<ScheduleViewModel, string>(itmes, key);
                            OffScheduleShiftsGroups.Add(newGroup);
                        }
                    }                    
                }

            });
        }

        public  void RefreshGroups(IList<ScheduleViewModel>  shifts)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (locker)
                {
                    UnsyncedShiftsGroups.Clear();
                    SchedulededShiftsGroups.Clear();
                    NewShiftsGroups.Clear();
                    OffScheduleShiftsGroups.Clear();
                    //Unsynced group
                    UnsyncedItems = shifts.Where(x => x.IsUnsynced).ToList();
                    foreach (var unsyncedItem in UnsyncedItems)
                    {
                        unsyncedItem.ParentContainerViewModel = this;
                        if (unsyncedItem.ServiceStartDateTime.HasValue)
                        {
                            var key = unsyncedItem.ServiceStartDateTime.Value.ToString("D");
                            var existingGroup = FindExistingGroup(UnsyncedShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(unsyncedItem);
                            }
                            else
                            {
                                var itmes = new ObservableCollection<ScheduleViewModel>();
                                itmes.Add(unsyncedItem);
                                var newGroup = new Grouping<ScheduleViewModel, string>(itmes, key);
                                UnsyncedShiftsGroups.Add(newGroup);
                            }
                        }
                    }
                    OnPropertyChanged("EnableSyncButton");
                    ////////////////
                    //Scheduled group
                    ScheduledItems = shifts.Where(x => (x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.Scheduled.Name) ||
                                                                  x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.Completed.Name) ||
                                                                  x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.CompletedWithoutClockIn.Name) ||
                                                                  x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.InProgress.Name) ||
                                                                  x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.ScheduleMissed.Name)) &&
                                                                  x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                                  x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                                  !x.IsUnscheduled).ToList();
                    foreach (var sheduledShift in ScheduledItems)
                    {
                        sheduledShift.ParentContainerViewModel = this;
                        if (sheduledShift.ServiceStartDateTime.HasValue)
                        {
                            var key = sheduledShift.ServiceStartDateTime.Value.ToString("D");

                            var existingGroup = FindExistingGroup(SchedulededShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(sheduledShift);
                            }

                            else
                            {
                                var items = new ObservableCollection<ScheduleViewModel>();
                                items.Add(sheduledShift);
                                var newGroup = new Grouping<ScheduleViewModel, string>(items, key);
                                SchedulededShiftsGroups.Add(newGroup);
                            }
                        }
                    }

                    ////////////////
                    //new group
                    NewItems = shifts.Where(x => x.WorkflowStatus.Name.Equals(ServiceVisitWorkflowStatus.Initiated.Name) &&
                                                             x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                             x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                             !x.IsUnscheduled).ToList();
                    foreach (var newShift in NewItems)
                    {
                        newShift.ParentContainerViewModel = this;
                        if (newShift.ServiceStartDateTime.HasValue)
                        {
                            var key = newShift.ServiceStartDateTime.Value.ToString("D");

                            var existingGroup = FindExistingGroup(NewShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(newShift);
                            }
                            else
                            {
                                var items = new ObservableCollection<ScheduleViewModel>();
                                items.Add(newShift);
                                var newGroup = new Grouping<ScheduleViewModel, string>(items, key);
                                NewShiftsGroups.Add(newGroup);
                            }
                        }
                    }

                    ////////////////
                    //off schedule group
                    OffScheduleItems = shifts.Where(x => x.IsUnscheduled &&
                                                                     x.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                                     x.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                                                     x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByStaff &&
                                                                     x.WorkflowStatus != ServiceVisitWorkflowStatus.Discarded &&
                                                                     x.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByClient &&
                                                                     x.WorkflowStatus != ServiceVisitWorkflowStatus.DeniedByMmis).ToList();
                    foreach (var shift in OffScheduleItems)
                    {
                        shift.ParentContainerViewModel = this;
                        if (shift.ServiceStartDateTime.HasValue)
                        {
                            var key = shift.ServiceStartDateTime.Value.ToString("D");
                            var existingGroup = FindExistingGroup(OffScheduleShiftsGroups, key);
                            if (existingGroup != null)
                            {
                                existingGroup.AddItem(shift);
                            }
                            else
                            {
                                var itmes = new ObservableCollection<ScheduleViewModel>();
                                itmes.Add(shift);
                                var newGroup = new Grouping<ScheduleViewModel, string>(itmes, key);
                                OffScheduleShiftsGroups.Add(newGroup);
                            }
                        }
                    }
                }

            });

        }

        private Grouping<ScheduleViewModel, string> FindExistingGroup(ObservableCollection<Grouping<ScheduleViewModel, string>> groupList, string key)
        {
            if (groupList == null)
                return null;
            foreach (var group in groupList)
            {
                if (group.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return group;
                }
            }
            return null;
        }

        private async Task<IList<ScheduleViewModel>> LoadCurrentCycleSchedules()
        {
            var errorMessage = "";
            try
            {
                DateTime startDate, endDate;
                GetCurrentCycleDateRange(out startDate, out endDate);
                var searchCriteria = new ServiceVisitSearchCriteriaDto
                {
                    PageSize = 1000,
                    PageNumber = 1,
                    PeriodStart = startDate,
                    PeriodEnd = endDate,
                    // VisitStaffId = SystemViewModel.Instance.CurrentStaffId//TODO:turn on this
                };
                ServiceVisitsWithPaginationResultDto serviceVisitResult = null;
                var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
                var response = await schedulerDataService.DownloadMyFurtureSchedules(searchCriteria,
                        SystemViewModel.Instance.HasNetworkConnection);
                if (response.IsFalied || response.ModelObject == null)
                {
                    errorMessage = response.ErrorMessage;

                }

                if (response.ModelObject != null)
                {
                    serviceVisitResult = response.ModelObject;
                    var shifts =
                        serviceVisitResult.ServiceVisitDtos.Where(s =>
                                s.StaffAcceptanceStatus != AcceptanceStatus.Rejected &&
                                s.ClientAcceptanceStatus != AcceptanceStatus.Rejected &&
                                s.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByStaff &&
                                s.WorkflowStatus != ServiceVisitWorkflowStatus.Discarded &&
                                s.WorkflowStatus != ServiceVisitWorkflowStatus.RejectedByClient &&
                                s.WorkflowStatus != ServiceVisitWorkflowStatus.DeniedByMmis)
                            .Select(x => AutoMapper.Mapper.Map<ServiceVisitDto, ScheduleViewModel>(x)).ToList();

                    return shifts;

                }

            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    await App.CurrentApp.MainPage.DisplayAlert("Loading shifts failed", errorMessage, "Ok");

                }
            }
            return new List<ScheduleViewModel>();
        }
        private void GetCurrentCycleDateRange(out DateTime startDate, out DateTime endDate)
        {
            DateTime dtNow = DateTimeOffset.Now.Date;
            int dayNum = 4 - (int)dtNow.DayOfWeek;
            startDate = dtNow.AddDays(dayNum); //Thursday of this week
            if (dayNum > 0)
            {
                startDate = startDate.AddDays(-7); //last Thursday
            }
            endDate = startDate.AddDays(7).AddMilliseconds(-1);
        }


        private static object locker = new object();
    }
}
