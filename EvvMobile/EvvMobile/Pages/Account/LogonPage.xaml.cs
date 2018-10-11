using System;
using System.Threading.Tasks;
using EvvMobile.Authorization;
using EvvMobile.Converters;
using EvvMobile.Notifications;
using EvvMobile.Pages.Systems;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Systems;
using Xamarin.Auth;
using Xamarin.Forms;

namespace EvvMobile.Pages.Account
{
    public partial class LogonPage : ContentPage
    {
        public LogonPage()
        {
            InitializeComponent();

            SystemViewModel.Instance.CleanMessages();

            NavigationPage.SetHasNavigationBar(this, false);

            //  this.BackgroundImage = "EvvMobile.Images.logInBackground.jpg";
           // MainBackgroundImage.Source = ImageSource.FromResource("EvvMobile.Images.logInBackground.jpg");
            LogoImage.Source = ImageSource.FromResource("EvvMobile.Images.FEilogo.png");
            UserImage.Source = ImageSource.FromResource("EvvMobile.Images.User.png");

            LockImage.Source = ImageSource.FromResource("EvvMobile.Images.Lock.png");

            var binding = new Binding()
            {
                Path = "HasNetworkConnection",
                Source = SystemViewModel.Instance,
                Converter = new BoolToSignInImageSourceConverter(),
            };

            SignInImage.SetBinding(Image.SourceProperty, binding);
            SignInImage.Aspect = Aspect.AspectFit;

            var signInImageTapGestureRecognizer = new TapGestureRecognizer();
            signInImageTapGestureRecognizer.Tapped += (s, e) =>
            {
                if (SystemViewModel.Instance.IsBusy)
                {
                    return;
                }
                LogonButtonClicked(s, e);
            };
            SignInImage.GestureRecognizers.Add(signInImageTapGestureRecognizer);
        }

        private async void LogonButtonClicked(object sender, EventArgs e)
        {
            /////
            /*if the network is disconnected, login with local account.
             will add this part*/
            ///////

            //TODO: will be removed, this is for Dev
            {
                SystemViewModel.Instance.IsBusy = true;
                try
                {
                    var defaultTags = new string[] { SystemViewModel.Instance.CurrentStaffId };

                    var result = await NotificationRegistrationService.Instance.RegisterDeviceAsync(defaultTags);
                    if (!result)
                    {
                        System.Diagnostics.Debug.WriteLine("Error registering with notification hub");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PushNotificationError]: Device registration failed with error {ex.Message}");
                }
                SystemViewModel.Instance.IsBusy = false;
                App.GoToRoot(); 
           
                return;                
            }

            ///////////////////////////////////////
            SystemViewModel.Instance.IsBusy = true;
            SystemViewModel.Instance.IdentityServerAuthenticator = new IdentityServerResourceOwerAuthenticator(IdentityServer3OpenIdAuthConstants.ClientId, IdentityServer3OpenIdAuthConstants.ClientSecret,
                IdentityServer3OpenIdAuthConstants.Scope);
            var task = SystemViewModel.Instance.IdentityServerAuthenticator.LoginForResourceOwner(UserNameEntry.Text, PasswordEntry.Text);
           // await task;
            if (await Task.WhenAny(task, Task.Delay(15000)) == task)
            {
                SystemViewModel.Instance.IsBusy = false;
                if (SystemViewModel.Instance.IdentityServerAuthenticator.IsAuthenticated)
                {


                        try
                        {
                            var result = await NotificationRegistrationService.Instance.RegisterDeviceAsync();
                            if (!result)
                            {
                                System.Diagnostics.Debug.WriteLine("Error registering with notification hub");
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[PushNotificationError]: Device registration failed with error {ex.Message}");
                        }
                        Device.BeginInvokeOnMainThread(
                            () => {App.GoToRoot();});

                    //will store/update local password and user name
                    
                }

            }
            else
            {
                SystemViewModel.Instance.IsBusy = false;
                //error();

            }
   
  /*   //openid auth for third party        
      var authenticator = new OpenIdAuthenticator(
                OpenIdAuthConstants.ClientId,
                 OpenIdAuthConstants.ClientSecret,
                OpenIdAuthConstants.Scope,
                new Uri(OpenIdAuthConstants.AuthorizeUrl),
                new Uri(OpenIdAuthConstants.RedirectUrl),null);

            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator); */

         SystemViewModel.Instance.IsBusy = false;
        }

        private void RegisterButtonClicked(object sender, EventArgs e)
        {
        }
         /*   //openid auth for third party    
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OpenIdAuthenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            if (e.IsAuthenticated)
            {


            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OpenIdAuthenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            //Debug.WriteLine("Authentication error: " + e.Message);
        }*/
    }
}
