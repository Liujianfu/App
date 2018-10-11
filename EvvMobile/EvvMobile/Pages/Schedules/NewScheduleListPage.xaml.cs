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
    public partial class NewScheduleListPage : NewScheduleListPageXaml
    {
        public NewScheduleListPage()
        {
            InitializeComponent();
            Title = TextResources.Schedule_New;

            NavigationPage.SetBackButtonTitle(this, "");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var parentPage =Parent as ScheduleTabPage;
            if (parentPage != null)
            {
                parentPage.Title = this.Title;
            }
            SystemViewModel.Instance.CleanMessages();
            ListItemSelected = false;
            ViewModel.UpdateNewGroup(ViewModel.NewItems);
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

            await Navigation.PushAsync(new NewScheduleDetailPage() { BindingContext = schedule, Title = TextResources.Schedule_Details });
            lock (locker)
            {
                ListItemSelected = false;
            }

        }
        private bool ListItemSelected { get; set; }
        private static object locker = new object();


    }

    public abstract class NewScheduleListPageXaml : ModelBoundContentPage<ScheduleListViewModel> { }
}
