using Xamarin.Auth;
namespace EvvMobile.Droid.Authorization
{
    public class OAuthLoginPresenter
    {
        
        public void Login(Authenticator authenticator)
        {
            Xamarin.Forms.Forms.Context.StartActivity(authenticator.GetUI(Xamarin.Forms.Forms.Context));
        }
    }
}