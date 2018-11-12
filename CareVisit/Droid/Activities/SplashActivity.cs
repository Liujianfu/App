using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using CareVisit.Droid.Activities;
namespace CareVisit.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/SplashTheme",Immersive =true, MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           // SetContentView(Resource.Layout.splash_screen);
            var newIntent = new Intent(this, typeof(LoginActivity));
            newIntent.AddFlags(ActivityFlags.ClearTop);
            newIntent.AddFlags(ActivityFlags.SingleTop);

            StartActivity(newIntent);
            Finish();
        }
    }
}
