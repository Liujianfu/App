using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.Pages.Systems;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.Pages.Schedules
{
    public partial class ScheduleTabPage : ScheduleTabPageXaml
    {
        public ScheduleTabPage()
        {
            InitializeComponent();
            Title = TextResources.Main_Schedule;
            NavigationPage.SetBackButtonTitle(this, "");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SystemViewModel.Instance.CleanMessages();
            ViewModel.IsBusy = false;
            if (ViewModel.IsInitialized)
                return;
            ViewModel.LoadSchedulesCommand.Execute(null);

            Children.Clear();
            var confirmedPage = new ScheduledScheduleListPage
            {
                Title = TextResources.Schedule_Scheduled,
                BindingContext = ViewModel,
                Icon = "schedule.png"
            };
            Children.Add(confirmedPage);
            var offSchedulePage = new OffScheduleListPage()
            {
                Title = TextResources.Schedule_OffSchedule,
                BindingContext = ViewModel,
                Icon = "schedule.png"
            };
            Children.Add(offSchedulePage);

            var newPage = new NewScheduleListPage()
            {
                Title = TextResources.Schedule_New,
                BindingContext = ViewModel,
                Icon = "schedule.png"
            };
            Children.Add(newPage);

            var unsyncPage = new UnsyncScheduleListPage
            {
                Title = TextResources.Schedule_Unsync,
                BindingContext = ViewModel,
                Icon = "schedule.png"
            };
            Children.Add(unsyncPage);
            ViewModel.IsInitialized = true;
        }

    }
    public abstract class ScheduleTabPageXaml : ModelBoundWithHomeButtonTabbedPage<ScheduleListViewModel> { }
}
