using System;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace EvvMobile.Pages.Schedules
{
    public partial class TodayShiftsListPage : TodayShiftsListPageXaml
    {
        public TodayShiftsListPage()
        {
            InitializeComponent();
            Title = TextResources.Schedule_TodayList;
            NavigationPage.SetBackButtonTitle(this, "");
            var collapseMapTapGestureRecognizer = new TapGestureRecognizer();
            collapseMapTapGestureRecognizer.Tapped += (s, e) =>
            {
                CollapseImageClicked(s, e);
            };
            CollapseImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            PastDueCollapseImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);

        }

        public static double HalfScreenHeight
        {
            get { return App.ScreenHeight / 2 > 0 ? App.ScreenHeight / 2 : 200; }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var parentPage = Parent as Page;
            if (parentPage != null)
            {
                parentPage.Title = this.Title;
            }
            SystemViewModel.Instance.CleanMessages();
            ListItemSelected = false;
            ViewModel.LoadSchedulesCommand.Execute(null);
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


        }
        private bool ListItemSelected { get; set; }
        private static object locker = new object();

        private void AddNewShift_OnClicked(object sender, EventArgs e)
        {
            //TODO:get data from DB/server
            var newShiftViewModel = new AddShiftViewModel
            {
                ClientMaNumber = "985658410",
                ProviderName = "provider Abc",
                ProviderMaNumber = "56874181",
                ParentListViewModel = null
            };
            newShiftViewModel.ServiceTypes.Add(EvvServiceTypes.EandDInHomeRespite);
            newShiftViewModel.ServiceTypes.Add(EvvServiceTypes.EandDNonEmergencyTransportation);

            //////////////////////////////////
            Navigation.PushAsync(new ScheduleCreatePage() { BindingContext = newShiftViewModel, Title = TextResources.Add_Shift });

        }

        private void CollapseImageClicked(object sender, EventArgs e)
        {
            bool isVisible = false;
            if (sender.Equals(CollapseImage))
                isVisible = UpcomingListViewLayout.IsVisible = !UpcomingListViewLayout.IsVisible;
            else if (sender.Equals(PastDueCollapseImage))
                isVisible = PastDueListViewLayout.IsVisible = !PastDueListViewLayout.IsVisible;

            if (isVisible)
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.collapsearrow40.png");
            }
            else
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.expandarrow40.png");
            }

        }
    }
    public abstract class TodayShiftsListPageXaml : ModelBoundWithHomeButtonContentPage<TodayShiftsViewModel> { }
}
