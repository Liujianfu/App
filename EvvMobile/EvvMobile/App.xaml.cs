using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Automapper;
using EvvMobile.Pages.Account;
using EvvMobile.Pages.Systems;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.Geolocator;
using Xamarin.Forms;
using EvvMobile.Database.FileStorages;
using EvvMobile.Database.Common;
using EvvMobile.Database.Models;
using EvvMobile.Pages.Schedules;

namespace EvvMobile
{
    public partial class App : Application
    {
        static Application app;
        public static double ScreenHeight;
        public static double ScreenWidth;
        public static double ScreenHeightForTrackingMap;
        public static Application CurrentApp
        {
            get { return app; }
        }
        public App()
        {
            InitializeComponent();
            app = this;

            BindingContext = SystemViewModel.Instance;

            var mainPage = new LogonPage();

            MainPage = new NavigationPage(mainPage);

            InitializeMapper();
            InitializeDatabase();
            Task.Run(() => InitializeSystem());
        }
        
        public static async void GoToRoot()
        {
            if (CurrentApp.MainPage is RootPage)
            {
                await ((RootPage) CurrentApp.MainPage).NavigateAsync(MenuType.MainPage);
            }
            else
            {
                CurrentApp.MainPage = new RootPage();
            }
        }
        public static void GoToLoginPage()
        {
            var mainPage = new LogonPage();
            CurrentApp.MainPage = new NavigationPage(mainPage);

        }

        public static INavigation GetCurrentDetailPageNavigation()
        {
            if (CurrentApp.MainPage is RootPage)
            {
                var rootPage = CurrentApp.MainPage as RootPage;
                return rootPage.Detail.Navigation;
            }
            return null;
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void InitializeMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ScheduleMappingProfile>();
            });
        }
        private async void InitializeSystem()
        {

            bool isConnected = CrossConnectivity.Current.IsConnected;
            SystemViewModel.Instance.HasNetworkConnection = isConnected;

            CrossConnectivity.Current.ConnectivityChanged += CurrentConnectivityChanged;

            var locator = CrossGeolocator.Current;


            if (locator.IsGeolocationEnabled && locator.IsGeolocationAvailable)
            {
                SystemViewModel.Instance.HasGps = true;
                locator.PositionChanged += CurrentPositionChanged;
                locator.DesiredAccuracy = 20;
                try
                {
                    var position = await locator.GetPositionAsync(timeoutMilliseconds: 1000);
                    SystemViewModel.Instance.CurrentLatitude = position.Latitude;
                    SystemViewModel.Instance.CurrentLongitude = position.Longitude;
                    SystemViewModel.Instance.InitialLocationCaptured = true;
                }
                catch (Exception ex)
                {
                    // Show Alert Message
                    SystemViewModel.Instance.ErrorMessage = "GPS is not enalbed or available!" + ex.Message;
                }

            }
            else
            {
                // Show Alert Message
                SystemViewModel.Instance.ErrorMessage = "GPS is not enalbed or available!";
            }
        }

        private void InitializeDatabase()
        {
            var fileHelper = DependencyService.Get<IFileHelper>();
            var filePath = fileHelper.GetLocalFilePath("EvvSQLite.db3");

            var instance = EvvDatabase.Instance;
            instance.Initialize(filePath, conn => {
                conn.CreateTableAsync<ServiceVisit>().Wait();
                conn.CreateTableAsync<ReasonsComments>().Wait();
                conn.CreateTableAsync<VisitStaff>().Wait();
                conn.CreateTableAsync<VisitTask>().Wait();
                conn.CreateTableAsync<AttachmentItem>().Wait();
                conn.CreateTableAsync<VisitMeasurement>().Wait();
                conn.CreateTableAsync<AttributeNameValue>().Wait();
                conn.CreateTablesAsync<Location, LocationTracking>();
            });
        }

        private void CurrentPositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var position = e.Position;
            if (position != null)
            {
                SystemViewModel.Instance.CurrentLatitude = position.Latitude;
                SystemViewModel.Instance.CurrentLongitude = position.Longitude;
            }
        }

        private void CurrentConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            SystemViewModel.Instance.HasNetworkConnection = e.IsConnected;
            if (CurrentApp.MainPage is RootPage)
            {
                var rootPage = ((RootPage)CurrentApp.MainPage);
                if (rootPage != null)
                {
                    var page = rootPage.GetPage(MenuType.ScheduleList) as EVVNavigationPage;
                    if (page.CurrentPage is ScheduleTabPage)
                    {
                        var scheduleListVm = page.CurrentPage.BindingContext as ScheduleListViewModel;
                        if(scheduleListVm!=null)
                            scheduleListVm.OnPropertyChanged("EnableSyncButton");
                    }
                }
            }
        }

        private void GetCurrentLocation()
        {

        }
    }
}
