using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Customizations;
using EvvMobile.Database.Models;
using EvvMobile.Database.Repositories;
using EvvMobile.Pages.Base;
using EvvMobile.Services;
using EvvMobile.ViewModels.Systems;
using EvvMobile.ViewModels.Trackings;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace EvvMobile.Pages.Syncs
{
    public partial class SyncLocationTrackingDetailPage : SyncLocationTrackingDetailPageXaml
    {
        public SyncLocationTrackingDetailPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            SystemViewModel.Instance.CleanMessages();
            SyncImage.Source = ImageSource.FromResource("EvvMobile.Images.Sync.png");
            SyncImage.Aspect = Aspect.AspectFit;
            var syncImageTapGestureRecognizer = new TapGestureRecognizer();
            syncImageTapGestureRecognizer.Tapped += (s, e) =>
            {
                SyncTrackingButtonClicked(s, e);
            };
            SyncImage.GestureRecognizers.Add(syncImageTapGestureRecognizer);

            DeleteImage.Source = ImageSource.FromResource("EvvMobile.Images.Delete.png");
            DeleteImage.Aspect = Aspect.AspectFit;
            var deleteImageTapGestureRecognizer = new TapGestureRecognizer();
            deleteImageTapGestureRecognizer.Tapped += (s, e) =>
            {
                DeleteTrackingButtonClicked(s, e);
            };
            DeleteImage.GestureRecognizers.Add(deleteImageTapGestureRecognizer);


            SystemViewModel.Instance.CleanMessages();
        }


        private  void SyncTrackingButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel.SyncLocationTrackingCommand.CanExecute(null))
                {
                    // Sync Tracking with Rest Service
                    ViewModel.SyncLocationTrackingCommand.Execute(null);
                    SyncImage.IsVisible = false;
                    DeleteImage.IsVisible = false;                    
                }


                //SystemViewModel.Instance.InfoMessage = "Done!";
            }
            catch (Exception)
            {
                //
            }
        }

        private void DeleteTrackingButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel.DeleteOfflineLocationTrackingCommand.CanExecute(null))
                {
                    ViewModel.DeleteOfflineLocationTrackingCommand.Execute(null);

                    ContentStackLayout.Children.Clear();
                    var stackLayout = new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    var image = new Image();
                    image.Aspect = Aspect.AspectFit;
                    image.Source = ImageSource.FromResource("EvvMobile.Images.Delete.png");
                    image.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    image.WidthRequest = 50;
                    stackLayout.Children.Add(image);

                    var lable = new Label()
                    {
                        Text = "Deleted!",
                        HorizontalOptions = LayoutOptions.CenterAndExpand
                    };
                    stackLayout.Children.Add(lable);

                    ContentStackLayout.Children.Add(stackLayout);                    
                }


            }
            catch (Exception)
            {

            }
        }



        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var tracking = ViewModel;
            if (tracking != null)
            {
                ShowMap(tracking);
            }
        }

        private void ShowMap(LocationTrackingViewModel tracking)
        {
            // Show Map
            try
            {
                if (tracking == null)
                    return;

                MapStackLayout.Children.Clear();

                var map = new CustomMap()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    IsShowingUser = false,
                    MapType = MapType.Street
                };
                var allPositions = locationOperation.GetLocationsAsync(tracking.Id).Result;
                if (allPositions != null)
                {
                    foreach (var p in allPositions)
                    {
                        map.RouteCoordinates.Add(new Position(p.Latitude, p.Longitude));
                    }

                    var moveToPosition = RouteService.CenterOfRoute(allPositions);

                    map.MoveToRegion(moveToPosition);

                    MapStackLayout.Children.Add(map);
                }

            }
            catch
            {
                SystemViewModel.Instance.ErrorMessage = "Showing Tracking Route Failed.";
            }
        }
        
        private LocationRepository locationOperation = new LocationRepository();
    }

    public abstract class SyncLocationTrackingDetailPageXaml : ModelBoundWithHomeButtonContentPage<LocationTrackingViewModel> { }
}
