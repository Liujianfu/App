using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CareVisit.Core;
using CareVisit.Core.Services;
using Android.Views.InputMethods;
using Android.Content.PM;

namespace CareVisit.Droid.Activities
{
    [Activity(Label = "Login", LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {

        EditText userName;
        EditText password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);
            userName = FindViewById<EditText>(Resource.Id.usernameEditText);
            password = FindViewById<EditText>(Resource.Id.passwordEditText);

            var signinButton = FindViewById<Button>(Resource.Id.loginBtn);
            var forgetPwButton = FindViewById<Button>(Resource.Id.forgetPassword);
            signinButton.Click += DoLogin;
            forgetPwButton.Click += ForgetPassword;
            userName.KeyPress += ProcessKeyPress;
            var scrollview = FindViewById<ScrollView>(Resource.Id.loginScrollview);
            scrollview.Touch += (s, e) =>

            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                if(scrollview!=null)
                    imm.HideSoftInputFromWindow(scrollview.WindowToken, 0);
                e.Handled = false;
            };
        }

        void ProcessKeyPress(object sender, View.KeyEventArgs e)
        {
            if(e.KeyCode==Keycode.Enter||e.KeyCode==Keycode.Escape)
            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(CurrentFocus.WindowToken,0);
            }
            
        }


        public void DoLogin(object sender,EventArgs eventArgs){
            //will add account check later
           var loginService =ServiceLocator.Instance.Get<IAccountService>();
            if(loginService.Login(userName.Text,password.Text)){
                //if login sucdeeded, go to main page
                var newIntent = new Intent(this, typeof(MainActivity));
                newIntent.AddFlags(ActivityFlags.ClearTop);
                newIntent.AddFlags(ActivityFlags.SingleTop);

                StartActivity(newIntent);
            }
            else{
                Toast.MakeText(this, Resource.String.sign_in_failed, ToastLength.Long).Show();
            }
        }

        public void ForgetPassword(object sender, EventArgs eventArgs)
        {
            //Go to ViewView
            var newIntent = new Intent(this, typeof(WebViewActivity));
            StartActivity(newIntent);
        }

    }
}