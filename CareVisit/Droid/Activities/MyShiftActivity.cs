
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using CareVisit.Droid.Fragments;

namespace CareVisit.Droid.Activities
{
    [Activity(Label = "My Shift",LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MyShiftActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.myshift_layout;

        ViewPager pager;
        TabsAdapter adapter;
        IMenuItem previousItem;
        DrawerLayout drawerLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var navigationView = FindViewById<NavigationView>(Resource.Id.left_navigationView);
            drawerLayout.DrawerOpened += DrawerLayout_DrawerOpened;
            drawerLayout.DrawerClosed += DrawerLayout_DrawerClosed;
            //drawerLayout.SetBackgroundResource(Resource.Drawable.loginbackground);
            //navigationView.SetItemTextAppearance(Resource.Color.loginButton);
            var toggle = new Android.Support.V7.App.ActionBarDrawerToggle
                (
                   this,
                   drawerLayout,
                   Resource.String.drawer_open,
                   Resource.String.drawer_close
                 );
            drawerLayout.AddDrawerListener(toggle);
            toggle.SyncState();

            navigationView.NavigationItemSelected += (sender, e) =>
            {

                if (previousItem != null)
                    previousItem.SetChecked(false);

                navigationView.SetCheckedItem(e.MenuItem.ItemId);

                previousItem = e.MenuItem;
                MenuItemClicked(e.MenuItem.ItemId);
                drawerLayout.CloseDrawers();
            };
            navigationView.ItemIconTintList = null;
            adapter = new TabsAdapter(this, SupportFragmentManager);
            pager = FindViewById<ViewPager>(Resource.Id.viewpager);
            var tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            pager.Adapter = adapter;
            tabs.SetupWithViewPager(pager);
            pager.OffscreenPageLimit = 3;

            pager.PageSelected += (sender, args) =>
            {
                var fragment = adapter.InstantiateItem(pager, args.Position) as IFragmentVisible;

                fragment?.BecameVisible();
            };

            Toolbar.MenuItemClick += (sender, e) =>
            {

            };
            int menuId = Intent.GetIntExtra("menuId", 0);
            if (menuId != 0)
            {
                if (previousItem != null)
                    previousItem.SetChecked(false);

                navigationView.SetCheckedItem(menuId);
                MenuItemClicked(menuId);
                Intent.RemoveExtra("menuId");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        private void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
        }

        private void DrawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
        {
        }

        private void MenuItemClicked(int menuId)
        {
            switch (menuId)
            {
                case Resource.Id.nav_home:

                case Resource.Id.nav_SaftyMonitoring:

                case Resource.Id.nav_logout:
                case Resource.Id.nav_aboutinfo:
                case Resource.Id.nav_myprofile:
                case Resource.Id.nav_HomeHMData:
                    {
                        var intent = new Intent(this, typeof(MainActivity));
                        intent.PutExtra("menuId", menuId);

                        StartActivity(intent);
                        Finish();
                        return;
                    }

            }
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
    }
    class TabsAdapter : FragmentStatePagerAdapter
    {
        string[] titles;

        public override int Count => titles.Length;

        public TabsAdapter(Context context, Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
            titles = context.Resources.GetTextArray(Resource.Array.sections);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) =>
                            new Java.Lang.String(titles[position]);

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return CalendarFragment.NewInstance();
                case 1: return SafetyMonitoringFragment.NewInstance();
            }
            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag) => PositionNone;
    }
}
