using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Statics
{
    public class IdentityServer3OpenIdAuthConstants
    {
        public static string ClientId = "A8375B66";
        public static string ClientSecret = "A32D8C3CBE9A";
        public static string Scope = "openid offline_access";//offline_access is for Identity Server, to get refresh token
        public static string ResponseType = "code id_token token";
        public static string AuthorizeUrl = "https://jianfuliult4.fei.local/Identity/connect/authorize/";//
        public static string AccessTokenUrl = "https://jianfuliult4.fei.local/Identity/connect/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public static string RedirectUrl = "https://jianfuliult4.fei.local/Identity/connect/authorize/";   

    }
}
