using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CareVisit.Core;
using CareVisit.Core.Controls.Calendar;
using CareVisit.Core.ViewModels;
using CareVisit.Droid.Adapters;

namespace CareVisit.Droid.Fragments
{

    public class CalendarFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        public static CalendarFragment NewInstance() =>
            new CalendarFragment { Arguments = new Bundle() };



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.calendar_layout, container, false);
            //add global layout event listener
            var dateGridView = view.FindViewById<GridView>(Resource.Id.date_gridView);
            dateGridView.StretchMode = StretchMode.StretchColumnWidth;
            dateGridView.SetNumColumns(7);
            careVisitCalendar = new CareVisitCalendar();
            rowItems = careVisitCalendar.CreateCalendarCells();
            dateGridView.Adapter =new CalendarViewAdapter(this, rowItems);


            dateGridView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                rowItems[7].Date= rowItems[7].Date.AddDays(3);
                rowItems[7].IsSelected = !rowItems[7].IsSelected;
                rowItems[7].EventCount = 1;
                dateGridView.InvalidateViews();

            };
            dateGridView.ViewTreeObserver.GlobalLayout += OnGlobalLayout;
            return view;
        }

        public override void OnStart()
        {
            AppCompatActivity appCompatActivity = (AppCompatActivity)Context;
            appCompatActivity.SupportActionBar.Title = "Schedules";
            base.OnStart();

        }

        public override void OnStop()
        {
            base.OnStop();

        }

        public void BecameVisible()
        {

        }
        protected void OnGlobalLayout(object sender, EventArgs args)
        {
            if (View==null)
                return;

            ViewTreeObserver vto = (ViewTreeObserver)sender; 
            var dateGridView = View.FindViewById<GridView>(Resource.Id.date_gridView);
            if (vto.IsAlive)
                vto.GlobalLayout -= OnGlobalLayout;
            else if(dateGridView !=null)
                dateGridView.ViewTreeObserver.GlobalLayout-= OnGlobalLayout;
            if (dateGridView != null)
                dateGridView.LayoutParameters.Height = dateGridView.MeasuredHeight * rowItems.Count/7;

        }

        List<CalendarCellViewModel> rowItems = new List<CalendarCellViewModel>();
        private CareVisitCalendar careVisitCalendar;
    }


}
