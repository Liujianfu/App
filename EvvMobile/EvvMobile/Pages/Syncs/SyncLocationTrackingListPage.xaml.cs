using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.Systems;
using EvvMobile.ViewModels.Trackings;
using Xamarin.Forms;

namespace EvvMobile.Pages.Syncs
{
    public partial class SyncLocationTrackingListPage : SyncLocationTrackingListPageXaml
    {
        public SyncLocationTrackingListPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            SyncAllImage.Source = ImageSource.FromResource("EvvMobile.Images.SyncAll.png");
            SyncAllImage.Aspect = Aspect.AspectFit;
            var syncAllImageTapGestureRecognizer = new TapGestureRecognizer();
            syncAllImageTapGestureRecognizer.Tapped += (s, e) =>
            {
                SyncAllButtonClicked(s, e);
            };
            SyncAllImage.GestureRecognizers.Add(syncAllImageTapGestureRecognizer);

            SystemViewModel.Instance.CleanMessages();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SystemViewModel.Instance.CleanMessages();
            ListItemSelected = false;
            //always reload data
            // if (ViewModel.IsInitialized)
            //      return;

            ViewModel.LoadLocationTrackingsCommand.Execute("");

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

            var tracking = e.SelectedItem as LocationTrackingViewModel;

            var trackingDetailPage = new SyncLocationTrackingDetailPage() { Title = TextResources.SyncLocationTracking, BindingContext = tracking };
            await Navigation.PushAsync(trackingDetailPage);
              


        }

        private  void SyncAllButtonClicked(object sender, EventArgs e)
        {
            try
            {
                SystemViewModel.Instance.CleanMessages();
                if (ViewModel.SyncAllTrackingsCommand.CanExecute(null))
                {
                    // Sync Tracking with Rest Service
                    ViewModel.SyncAllTrackingsCommand.Execute(null);

                    SystemViewModel.Instance.IsBusy = true;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ContentStackLayout.Children.Clear();

                        var stackLayout = new StackLayout()
                        {
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        };

                        var image = new Image();
                        image.Aspect = Aspect.AspectFit;
                        image.Source = ImageSource.FromResource("EvvMobile.Images.Done.png");
                        image.HorizontalOptions = LayoutOptions.CenterAndExpand;
                        image.WidthRequest = 50;
                        stackLayout.Children.Add(image);

                        var lable = new Label()
                        {
                            Text = "Synchronized!",
                            HorizontalOptions = LayoutOptions.CenterAndExpand
                        };
                        stackLayout.Children.Add(lable);

                        ContentStackLayout.Children.Add(stackLayout);
                    });

                    SystemViewModel.Instance.IsBusy = false;
                    ViewModel.IsBusy = false;
                }
                
            }
            catch (Exception)
            {
                SystemViewModel.Instance.IsBusy = false;
                SystemViewModel.Instance.InfoMessage = "Sync All Failed!";
            }
        }
        private bool ListItemSelected { get; set; }
        private static object locker = new object();
    }
    public abstract class SyncLocationTrackingListPageXaml : ModelBoundWithHomeButtonContentPage<LocationTrackingListViewModel> { }
}
