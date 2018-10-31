
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android;

using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

using Android.Support.V4.View;
using Android.Content.Res;



namespace CareVisit.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/carevisit_logo", Theme = "@style/CustomDrawerTheme")]
    public class SlidingPaneActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(CareVisit.Droid.Resource.Layout.DrawerLayout);
            // Create your application here
            //Finding toolbar and adding to actionbar  
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            //For showing back button  
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

        }
    }
}
