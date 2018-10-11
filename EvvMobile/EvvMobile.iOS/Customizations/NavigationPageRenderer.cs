using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using EvvMobile.iOS.Customizations;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]
namespace EvvMobile.iOS.Customizations
{
    public class NavigationPageRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            // Create the back arrow icon image
            var arrowImage = UIImage.FromBundle("back.png");
            NavigationBar.BackIndicatorImage = arrowImage;
            NavigationBar.BackIndicatorTransitionMaskImage = arrowImage;
            
            // Set the back button title to empty since the Design doesn't use it.
            if (NavigationItem?.BackBarButtonItem != null)
                NavigationItem.BackBarButtonItem.Title = "";
            if (NavigationBar.BackItem != null)
            {
                NavigationBar.BackItem.Title = "";
                NavigationBar.BackItem.BackBarButtonItem.Image = arrowImage;
            }
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (this.NavigationBar == null) return;

            if (NavigationItem?.BackBarButtonItem != null)
                NavigationItem.BackBarButtonItem.Title = "";
            if (NavigationBar.BackItem != null)
            {
                NavigationBar.BackItem.Title = "";
                NavigationBar.BackItem.BackBarButtonItem.Title = "";
            }
        }
        
    }
}