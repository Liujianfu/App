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

namespace CareVisit.Droid.Activities
{
    [Activity(Label = "Login")]
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
            signinButton.Click += DoLogin;
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
    }
}