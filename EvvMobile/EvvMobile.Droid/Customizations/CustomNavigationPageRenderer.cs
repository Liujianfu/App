using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Graphics.Drawable;
using Android.Views;
using Android.Widget;
using EvvMobile.Droid.Customizations;
using EvvMobile.Pages.Systems;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationPageRenderer))]
namespace EvvMobile.Droid.Customizations
{
    public class CustomNavigationPageRenderer : NavigationPageRenderer
    {
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar == null)
                return;
            bool isNavigated = ((INavigationPageController)Element).StackDepth > 1;
            for (var i = 0; i < toolbar.ChildCount; i++)
            {
                var imageButton = toolbar.GetChildAt(i) as ImageButton;

                var drawerArrow = imageButton?.Drawable as DrawerArrowDrawable;
                if (drawerArrow == null)
                    continue;
                if (isNavigated)
                {
                    imageButton.SetImageDrawable(Forms.Context.GetDrawable(Resource.Drawable.back));
                }
                else
                {
                    imageButton.SetImageDrawable(Forms.Context.GetDrawable(Resource.Drawable.slideout));
                }
                break;
            }
        }

    }
}