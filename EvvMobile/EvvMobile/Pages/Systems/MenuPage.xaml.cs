using System.Collections.Generic;
using EvvMobile.Localization;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.Pages.Systems
{
    public partial class MenuPage : ContentPage
    {
        RootPage root;
        List<HomeMenuItem> menuItems;
        public MenuPage(RootPage root)
        {
            this.root = root;

            InitializeComponent();
            BindingContext = new BaseViewModel(Navigation)
            {
                Title = "Menu",
                Subtitle = "",
                Icon = "slideout.png"
            };

            ListViewMenu.ItemsSource = menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem { Title = TextResources.Main_Page, MenuType = MenuType.MainPage, Icon =ImageSource.FromResource("EvvMobile.Images.Schedule.png" )},
                    new HomeMenuItem { Title = TextResources.Schedule_TodayList, MenuType = MenuType.TodayShifts, Icon =ImageSource.FromResource("EvvMobile.Images.today.png" )},

                    new HomeMenuItem { Title = TextResources.Main_Login, MenuType = MenuType.Login, Icon =ImageSource.FromResource("EvvMobile.Images.signout.png" )},
                    new HomeMenuItem { Title = TextResources.Schedule_MyCalendar, MenuType = MenuType.Calendar, Icon =ImageSource.FromResource("EvvMobile.Images.Calendar.png") },
                    new HomeMenuItem { Title = TextResources.Main_Chart, MenuType = MenuType.MainChart, Icon =ImageSource.FromResource("EvvMobile.Images.analysis.png") },
                    new HomeMenuItem { Title = TextResources.Main_Schedule, MenuType = MenuType.ScheduleList, Icon =ImageSource.FromResource("EvvMobile.Images.Schedule.png") },
                    new HomeMenuItem { Title = TextResources.Track_Location, MenuType = MenuType.Tracking, Icon =ImageSource.FromResource("EvvMobile.Images.Tracking.png") },
                    new HomeMenuItem { Title = TextResources.Main_About, MenuType = MenuType.About, Icon = ImageSource.FromResource("EvvMobile.Images.about.png") }
                };

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (ListViewMenu.SelectedItem == null)
                    return;

                await this.root.NavigateAsync(((HomeMenuItem)e.SelectedItem).MenuType);
                ListViewMenu.SelectedItem = null;
            };
        }
    }
}
