using System;
using System.Net;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using EvvMobile.Droid.Authorization;
using EvvMobile.Statics;
using ZXing.Net.Mobile;

namespace EvvMobile.Droid
{
    [Activity(Label = "EvvMobile", Icon = "@drawable/icon", Theme = "@style/MainTheme",  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Instance;

        protected override void OnCreate(Bundle bundle)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            global::Xamarin.Forms.Forms.Init(this, bundle);
            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            //for Oauth2 
            /*     Xamarin.Auth.Presenters.OAuthLoginPresenter.PlatformLogin = (authenticator) =>
                 {
                     var oAuthLogin = new OAuthLoginPresenter();
                     oAuthLogin.Login(authenticator);
                 };
                 */

            var width = Resources.DisplayMetrics.WidthPixels;
            var height = Resources.DisplayMetrics.HeightPixels;
            var density = Resources.DisplayMetrics.Density;

            App.ScreenWidth = (width - 0.5f) / density;
            App.ScreenHeight = (height - 0.5f) / density;
            App.ScreenHeightForTrackingMap = (height - 0.5f) / density - 200;
            Xamarin.FormsMaps.Init(this, bundle);
            LoadApplication(new App());

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

