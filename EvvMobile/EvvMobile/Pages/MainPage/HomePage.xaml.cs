using System;
using System.Threading.Tasks;
using EvvMobile.Authorization;
using EvvMobile.Converters;
using EvvMobile.Localization;
using EvvMobile.Notifications;
using EvvMobile.Pages.Systems;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Systems;
using EvvMobile.Pages.Base;
using EvvMobile.Pages.Messages;
using EvvMobile.Pages.Schedules;
using Xamarin.Forms;
using EvvMobile.ViewModels.MainPage;
using EvvMobile.ViewModels.Messages;
using EvvMobile.ViewModels.Schedules;

namespace EvvMobile.Pages.MainPage
{
    public partial class HomePage : MainPageXaml
    {
        public HomePage()
        {
            InitializeComponent();

            SystemViewModel.Instance.CleanMessages();
            var pinTapGestureRecognizer = new TapGestureRecognizer();
            pinTapGestureRecognizer.Tapped += (s, e) =>
            {
                PinView_OnClicked(s, e);
            };
            PinView.GestureRecognizers.Add(pinTapGestureRecognizer);
            ShiftsListPinView.GestureRecognizers.Add(pinTapGestureRecognizer);
            var collapseTapGestureRecognizer = new TapGestureRecognizer();
            collapseTapGestureRecognizer.Tapped += (s, e) =>
            {
                CollapseImageClicked(s, e);
            };
            CollapseMessageListImage.GestureRecognizers.Add(collapseTapGestureRecognizer);
            CollapseShiftsListImage.GestureRecognizers.Add(collapseTapGestureRecognizer);

            var todayShiftViewAllTapGestureRecognizer = new TapGestureRecognizer();
            todayShiftViewAllTapGestureRecognizer.Tapped += (s, e) =>
            {
                TodayShifts_OnClicked(s, e);
            };
            ViewAllShifts.GestureRecognizers.Add(todayShiftViewAllTapGestureRecognizer);
            var messagesViewAllTapGestureRecognizer = new TapGestureRecognizer();
            messagesViewAllTapGestureRecognizer.Tapped += (s, e) =>
            {
                MyNewMessages_OnClicked(s, e);
            };
            ViewAllMessages.GestureRecognizers.Add(messagesViewAllTapGestureRecognizer);
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) => OnFirstShiftClicked();
            FirstShiftServiceNameLabel.GestureRecognizers.Add(tgr);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SystemViewModel.Instance.CleanMessages();
            if (!ViewModel.IsInitialized)
            {
                ViewModel.IsInitialized = true;
           
            }
            ListItemSelected = false;
            ViewModel.LoadNewMessagesCommand.Execute(null);
            ViewModel.LoadSchedulesCommand.Execute(null);
        }
        private async void OnFirstShiftClicked()
        {
            lock (locker)
            {
                if (ViewModel.FirstShift == null)
                {
                    return;
                }
            }

            await Navigation.PushAsync(new ScheduleDetailPage() { BindingContext = ViewModel.FirstShift, Title = TextResources.Schedule_Details });

        }
        private async void OnShiftItemSelected(object sender, SelectedItemChangedEventArgs e)
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
        private void CollapseImageClicked(object sender, EventArgs e)
        {
            bool isVisible = false;
            if (sender.Equals(CollapseMessageListImage))
            {
                isVisible = NewMessageList.IsVisible = !NewMessageList.IsVisible;
                FirstMessageLayout.IsVisible = !isVisible;
                PinView.IsVisible = isVisible;
            }
            else if (sender.Equals(CollapseShiftsListImage))
            {
                isVisible = TodayShiftsListView.IsVisible = !TodayShiftsListView.IsVisible;
                FirstShiftLayout.IsVisible = !isVisible;
                ShiftsListPinView.IsVisible = isVisible;
            }
            

            if (isVisible)
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.collapsearrow40.png");
            }
            else
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.expandarrow40.png");
                //cancel pin
                HomeScrollView.InputTransparent = false;    
                PinView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
                ShiftsListPinView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
                
            }
  
        }
        private async void TodayShifts_OnClicked(object sender, EventArgs e)
        {
            var rootPage = App.CurrentApp.MainPage as RootPage;
            if (rootPage != null)
            {
                await rootPage.NavigateAsync(MenuType.TodayShifts);
            }
        }

        private async void MyShifts_OnClicked(object sender, EventArgs e)
        {
            var rootPage = App.CurrentApp.MainPage as RootPage;
            if (rootPage !=null)
            {
               await rootPage.NavigateAsync(MenuType.ScheduleList);
            }
        }

        private async void MyCalendar_OnClicked(object sender, EventArgs e)
        {
            var rootPage = App.CurrentApp.MainPage as RootPage;
            if (rootPage != null)
            {
                await rootPage.NavigateAsync(MenuType.Calendar);
            }
        }

        private async void MainDashboard_OnClicked(object sender, EventArgs e)
        {
            var rootPage = App.CurrentApp.MainPage as RootPage;
            if (rootPage != null)
            {
                await rootPage.NavigateAsync(MenuType.MainChart);
            }
        }

        private async void Logout_OnClicked(object sender, EventArgs e)
        {
            var rootPage = App.CurrentApp.MainPage as RootPage;
            if (rootPage != null)
            {
                await rootPage.NavigateAsync(MenuType.Login);
            }
        }

        private async void MyMessages_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MessageListPage { BindingContext = new MessageListViewModel(), Title = TextResources.MessageList });

        }

        private async void MyNewMessages_OnClicked(object sender, EventArgs e)
        {
            var messageListViewModel = new MessageListViewModel
            {
                NewMessageList = ViewModel.NewMessageList
            };
            await Navigation.PushAsync(new NewMessageListPage{ BindingContext = messageListViewModel, Title = TextResources.NewMessageList });

        }
        private void PinView_OnClicked(object sender, EventArgs e)
        {

            HomeScrollView.InputTransparent = !HomeScrollView.InputTransparent;
            if (HomeScrollView.InputTransparent)
            {
                PinView.Source = ImageSource.FromResource("EvvMobile.Images.Unpin.png");
                ShiftsListPinView.Source = ImageSource.FromResource("EvvMobile.Images.Unpin.png");
            }
            else
            {
                PinView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
                ShiftsListPinView.Source = ImageSource.FromResource("EvvMobile.Images.pin.png");
            }
        }
    }
    public class MainPageXaml : ModelBoundContentPage<MainPageViewModel> { }
}
