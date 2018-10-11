using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Statics
{
    public class OAuthSettings
    {

        // OAuth
        public static string ClientId = "A8375B66";
        public static string ClientSecret = "A32D8C3CBE9A";

        public static string Scope = "profile";
        public static string AuthorizeUrl = "http://xamuath.azurewebsites.net/oauth/authorize";
        public static string AccessTokenUrl = "http://xamuath.azurewebsites.net/oauth/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        // Set this property to the location the user will be redirected too after successfully authenticating
        public static string RedirectUrl = "http://localhost:5592/myapp";
    }
}
