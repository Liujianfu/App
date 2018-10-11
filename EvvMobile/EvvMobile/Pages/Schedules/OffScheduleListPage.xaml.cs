using System;
using System.Collections.Specialized;
using System.Linq;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.Pages.Systems;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using EvvMobile.Views.Schedules;
using Xamarin.Forms;

namespace EvvMobile.Pages.Schedules
{
    public partial class OffScheduleListPage : OffScheduleListPageXaml
    {
        public OffScheduleListPage()
        {
            InitializeComponent();
            Title = TextResources.Schedule_OffSchedule;
            NavigationPage.SetBackButtonTitle(this, "");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var parentPage = Parent as ScheduleTabPage;
            if (parentPage != null)
            {
                parentPage.Title = this.Title;
            }
            SystemViewModel.Instance.CleanMessages();
            ListItemSelected = false;
            ViewModel.UpdateOffScheduleGroup(ViewModel.OffScheduleItems);
            if (ViewModel.IsInitialized)
            {
                return;
            }

            ViewModel.IsInitialized = true;
        }
        private async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            lock (locker)
            {
                if (e.SelectedItem == null || ListItemSelected)
                {
                    return;
                }
                ListItemSelected = true;
            }

            var schedule = e.SelectedItem as ScheduleViewModel;

            await Navigation.PushAsync(new ScheduleDetailPage() { BindingContext = schedule, Title = TextResources.Schedule_Details });
            lock (locker)
            {
                ListItemSelected = false;
            }

        }
        private void OnCreateUnscheduledShift(object sender, EventArgs e)
        {
            //TODO:get data from DB/server
            var newShiftViewModel = new AddShiftViewModel
            {
                ClientMaNumber = "985658410",
                ProviderName = "provider Abc",
                ProviderMaNumber = "56874181",
                ParentListViewModel= ViewModel
            };
            newShiftViewModel.ServiceTypes.Add(EvvServiceTypes.EandDInHomeRespite);
            newShiftViewModel.ServiceTypes.Add(EvvServiceTypes.EandDNonEmergencyTransportation);
            
            //////////////////////////////////
            Navigation.PushAsync(new ScheduleCreatePage() { BindingContext = newShiftViewModel, Title = TextResources.Add_Shift });
        }

        private bool ListItemSelected { get; set; }
        private static object locker = new object();
    }

    public abstract class OffScheduleListPageXaml : ModelBoundContentPage<ScheduleListViewModel> { }
}
