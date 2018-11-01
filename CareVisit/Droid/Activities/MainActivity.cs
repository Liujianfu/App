﻿
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
using Android.Content.PM;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

using Android.Support.V4.View;
using Android.Content.Res;
using Android.Text;
using Android.Text.Style;
using Android.Support.V4.Content;
using Android.Graphics;

namespace CareVisit.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/carevisit_logo", Theme = "@style/CustomDrawerTheme",
          LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        //Declaring Variables to access throught this activity  
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        IMenuItem previousItem;
        Android.Support.V7.App.ActionBarDrawerToggle toggle;
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
           // SupportActionBar.SetDisplayShowTitleEnabled(false);
            //setting Hamburger icon Here  
            //SupportActionBar.SetHomeAsUpIndicator(CareVisit.Droid.Resource.Drawable.ic_menu);
            //Getting Drawer Layout declared in UI and handling closing and open events  
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerLayout.DrawerOpened += DrawerLayout_DrawerOpened;
            drawerLayout.DrawerClosed += DrawerLayout_DrawerClosed;
            navigationView = FindViewById<NavigationView>(Resource.Id.left_navigationView);
            toggle = new Android.Support.V7.App.ActionBarDrawerToggle
            (
                    this,
                    drawerLayout,
                    Droid.Resource.String.drawer_open,
                    Droid.Resource.String.drawer_close
            );
            drawerLayout.AddDrawerListener(toggle);

            //Synchronize the state of the drawer indicator/affordance with the linked DrawerLayout  
            toggle.SyncState();

            //Handling click events on Menu items  
            navigationView.NavigationItemSelected += (sender, e) =>
            {

                if (previousItem != null)
                    previousItem.SetChecked(false);

                navigationView.SetCheckedItem(e.MenuItem.ItemId);

                previousItem = e.MenuItem;

                MenuItemClicked(e.MenuItem.ItemId);



                drawerLayout.CloseDrawers();
            };

            //set menu item color
            //this is demo code
            var todayMenu = navigationView.Menu.FindItem(Resource.Id.nav_today);
            if(todayMenu!=null){
                SetTextColorForMenuItem(todayMenu,Resource.Color.primary);
            }
               
        }
        private void SetTextColorForMenuItem(IMenuItem menuItem, int color)
        {
            SpannableString spanString = new SpannableString(menuItem.TitleFormatted.ToString());
            spanString.SetSpan(new ForegroundColorSpan(new Color(ContextCompat.GetColor(this, color))), 0, spanString.Length(), 0);
            menuItem.SetTitle(spanString);
        }
        private void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            //SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);  
        }

        private void DrawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
        {
            // SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_back);  
        }
        private void MenuItemClicked(int menuId)
        {
            Android.Support.V4.App.Fragment fragment = null;
          /*  switch (menuId)
            {
                case 0:
                    fragment = new LoginFragment();
                    break;
                case 1:
                    fragment = new Signupfragment();
                    break;
            }*/
            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                               .Replace(Droid.Resource.Id.content_frame, fragment)
                               .Commit();
            }


        }
        //Handling Back Key Press  
        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen((int)GravityFlags.Start))
            {
                drawerLayout.CloseDrawer((int)GravityFlags.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return true;
        }



        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        //Resposnible for mainting state,suppose if you suddenly rotated screen than drawer should not losse it context so you have save drawer states like below  
        protected override void OnPostCreate(Bundle savedInstanceState)
        {

            base.OnPostCreate(savedInstanceState);
            toggle.SyncState();

        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            toggle.OnConfigurationChanged(newConfig);
        }
    }
}
