using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace EvvMobile.Schedule.NUnit.Droid
{
	[Activity (Label = "EvvMobile.Schedule.NUnit", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

		    global::Xamarin.Forms.Forms.Init(this, bundle);

		    // This will load all tests within the current project
		    var nunit = new global::NUnit.Runner.App();

		    // If you want to add tests in another assembly
		    //nunit.AddTestAssembly(typeof(MyTests).Assembly);

		    // Do you want to automatically run tests when the app starts?
		    nunit.Options.AutoRun = true;

		    LoadApplication(nunit);
        }
	}
}

