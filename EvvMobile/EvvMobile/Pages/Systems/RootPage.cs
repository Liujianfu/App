using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Pages.About;
using EvvMobile.Pages.Account;
using EvvMobile.Statics;
using EvvMobile.ViewModels.About;
using EvvMobile.ViewModels.Base;
using EvvMobile.Localization;
using EvvMobile.Notifications;
using EvvMobile.Pages.Charts;
using EvvMobile.Pages.MainPage;
using EvvMobile.Pages.Schedules;
using EvvMobile.Pages.Trackings;
using EvvMobile.ViewModels.Charts;
using EvvMobile.ViewModels.MainPage;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Trackings;
using Xamarin.Forms;

namespace EvvMobile.Pages.Systems
{
    public class RootPage : MasterDetailPage
    {
        Dictionary<MenuType, NavigationPage> Pages { get; set; }
        MenuType CurrentMenuType = MenuType.Login;
        public RootPage()
        {
            Pages = new Dictionary<MenuType, NavigationPage>();
            Master = new MenuPage(this);
            BindingContext = new BaseViewModel(Navigation)
            {
                Title = "EVV",
                //Icon = "icon.png"
            };
            //setup home page
            NavigateAsync(MenuType.MainPage);
        }

        void SetDetailIfNull(Page page)
        {
            if (Detail == null && page != null)
                Detail = page;
        }

        public async Task NavigateAsync(MenuType id)
        {
            Page newPage;
            if (!Pages.ContainsKey(id))
            {
                switch (id)
                {
                    default:
                    case MenuType.Login:
                        {
                            //Hide navigation bar and jump to login page
                            await Task.Run( () =>
                            {
                                try
                                {
                                     NotificationRegistrationService.Instance.DeregisterDeviceAsync().Wait();
                                    System.Diagnostics.Debug.WriteLine("Should be deregistered");
                                }
                                catch (System.Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[PushNotificationError]: Device deregistration failed with error {ex.Message}");
                                }
                            });
                            await Task.Run(() =>
                            {
                                Device.BeginInvokeOnMainThread(
                                    () => { App.GoToLoginPage(); });
                            });
                            return;
                        }

                    case MenuType.ScheduleList:                    
                        {
                            var page = new EVVNavigationPage(new ScheduleTabPage()
                            {
                                Title = TextResources.Main_Schedule,
                                Icon = "back.png",
                                BindingContext = new ScheduleListViewModel() { }
                            });
                            
                            SetDetailIfNull(page);
                            Pages.Add(id, page);
                            break;
                        }
                    case MenuType.Calendar:
                    {
                        var page = new EVVNavigationPage(new CalendarPage()
                        {
                            Title = TextResources.Schedule_MyCalendar,

                            BindingContext = new CalenderViewModel() { }
                        });
                        SetDetailIfNull(page);
                        Pages.Add(id, page);
                        break;
                    }
                    case MenuType.Tracking:
                    {
                        var page = new EVVNavigationPage(new TrackLocationPage()
                        {
                            Title = TextResources.Track_Location,

                            BindingContext = new LocationTrackingViewModel() { }
                        });
                        SetDetailIfNull(page);
                        Pages.Add(id, page);
                        break;
                    }
                    case MenuType.About:
                        {
                            var page = new EVVNavigationPage(new AboutItemListPage()
                            {
                                Title = TextResources.Main_About,

                                BindingContext = new AboutItemListViewModel() { }
                            });
                            SetDetailIfNull(page);
                            Pages.Add(id, page);
                            break;
                        }
                    case MenuType.MainChart:
                    {
                        var page = new EVVNavigationPage(new MainChartPage()
                        {
                            Title = TextResources.Main_Chart,

                            BindingContext = new MainChartViewModel() { }
                        });
                        SetDetailIfNull(page);
                        Pages.Add(id, page);
                        break;
                    }
                    case MenuType.MainPage:
                    {
                        var page = new EVVNavigationPage(new HomePage()
                        {
                            Title = TextResources.Main_Page,

                            BindingContext = new MainPageViewModel() { }
                        });
                        SetDetailIfNull(page);
                        Pages.Add(id, page);
                        break;
                    }
                    case MenuType.TodayShifts:
                    {
                         var page = new EVVNavigationPage(new TodayShiftsListPage()
                        {
                            Title = TextResources.Schedule_TodayList,

                            BindingContext = new TodayShiftsViewModel() { }
                        });
                        SetDetailIfNull(page);
                        Pages.Add(id, page);
                        break;
                        }
                        //add other pages
                }
            }

            newPage = Pages[id];
            if (newPage == null)
                return;
            if (MenuType.MainPage == id)
            {
                await Detail.Navigation.PopToRootAsync();
            }
            else
            //pop to root for Windows Phone
            if (Detail != null && (Device.RuntimePlatform != Device.iOS && Device.RuntimePlatform != Device.Android))
            {
                await Detail.Navigation.PopToRootAsync();
            }

            Detail = newPage;

            if (Device.Idiom != TargetIdiom.Tablet)
                IsPresented = false;
        }
        protected override bool OnBackButtonPressed()
        {
            Detail.Navigation.PopAsync();
            return true;
        }

        public Page GetPage(MenuType menuType)
        {
            NavigationPage page;
            if (Pages.TryGetValue(menuType, out page))
            {
                return page;
            }
            return null;
        }
    }

    public class RootTabPage : TabbedPage
    {
        public RootTabPage()
        {
            Children.Add(new EVVNavigationPage(new LogonPage
            {
                Title = TextResources.Main_Login
            }));

            Children.Add(new EVVNavigationPage(new ScheduleTabPage
            {
                Title = TextResources.Main_Schedule,
                BindingContext = new ScheduleListViewModel { }
            }));

            Children.Add(new EVVNavigationPage(new AboutItemListPage()
            {
                Title = TextResources.Main_About,
                BindingContext = new AboutItemListViewModel() { }
            }));


            //add other pages
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            this.Title = this.CurrentPage.Title;
        }
    }

    public class EVVNavigationPage : NavigationPage
    {
        public EVVNavigationPage(Page root)
            : base(root)
        {
            Init();
        }

        public EVVNavigationPage()
        {
            Init();
        }

        void Init()
        {

            BarBackgroundColor = Palette.NavigationbarColor;
            BarTextColor = Color.White;
        }
    }
}
