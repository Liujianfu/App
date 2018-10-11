using System;
using System.Collections.ObjectModel;
using EvvMobile.Customizations.CustomControls.Calendar;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using EvvMobile.Views.Schedules;
using Xamarin.Forms;

namespace EvvMobile.Pages.Schedules
{
    public partial class CalendarPage : CalendarPageXaml
    {

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SystemViewModel.Instance.CleanMessages();
            ViewModel.IsBusy = false;
            ListItemSelected = false;

            ViewModel.UpdateAppointmentCountPerDay();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarPage"/> class.
        /// </summary>
        public CalendarPage()
        {
            InitializeComponent();
            _width = this.Width;
            _height = this.Height;

            NavigationPage.SetBackButtonTitle(this, "");

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
        private double _width;
        private double _height;
        private void Calendar_OnOnEndRenderCalendar(object sender, DateTimeEventArgs e)
        {

            if (ViewModel.LoadOneMonthSchedulesCommand.CanExecute(e.DateTime) && !ViewModel.IsInitialized)
            {
                ViewModel.IsInitialized = true;
                ViewModel.LoadOneMonthSchedulesCommand.Execute(e.DateTime);

            }
            ListviewLayout.Children.Clear();
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            var oldWidth = _width;
            const double sizenotallocated = -1;

            base.OnSizeAllocated(width, height);
            if (Equals(_width, width) && Equals(_height, height)) return;

            _width = width;
            _height = height;
            if (Equals(oldWidth, sizenotallocated)) return;
            ViewModel.UpdateAppointmentCountPerDay();
        }

        private void Calendar_OnDateClicked(object sender, DateTimeEventArgs e)
        {
            ListviewLayout.Children.Clear();
            ViewModel.GetSelectedDateItems(e);
            var listview = new CalendarScheduleListView
            {
                ItemsSource = ViewModel.SelectedScheduleItems,
                
            };
            listview.ItemSelected += OnListItemSelected;
            ListviewLayout.Children.Add(listview);
        }
    }

    public abstract class CalendarPageXaml : ModelBoundWithHomeButtonContentPage<CalenderViewModel> { }
}
