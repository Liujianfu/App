
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CareVisit.Droid.Fragments
{
    public class SafetyMonitoringFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        public static SafetyMonitoringFragment NewInstance() =>
            new SafetyMonitoringFragment { Arguments = new Bundle() };



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

             base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.safetymonitor_layout, container, false);

            return view;
        }
        public override void OnStart()
        {
            AppCompatActivity appCompatActivity = (AppCompatActivity)Context;
            appCompatActivity.SupportActionBar.Title = "Beneficiary Safety Monitoring";
            base.OnStart();

        }

        public override void OnStop()
        {
            base.OnStop();

        }

        public void BecameVisible()
        {

        }


    }
}
