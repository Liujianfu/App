
using System;

namespace CareVisit.Core
{
    public class GlobalSetting
    {
        #region fields
        private string _baseIdentityUrl;
        private string _baseMobileServiceUrl;
        #endregion

        public GlobalSetting()
        {
            AuthToken = "INSERT AUTHENTICATION TOKEN";
            BaseMobileServiceUrl = "http://YOUR_IP_OR_DNS_NAME"; // i.e.: "http://YOUR_IP" or "http://YOUR_DNS_NAME"
            BaseIdentityUrl = "http://YOUR_IP_OR_DNS_NAME";      // i.e.: "http://YOUR_IP" or "http://YOUR_DNS_NAME"
            ResetPasswordUrl = "https://demo.identityserver.io/";
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting();

        public string MobileServiceUrl { get; set; }

        #region Identity Properties

        public string ClientId { get { return "xamarin"; } }

        public string ClientSecret { get { return "secret"; } }

        public string AuthToken { get; set; }

        public string RegisterWebsite { get; set; }

        public string ResetPasswordUrl { get; set; }

        public string AuthorizeUrl { get; set; }

        public string UserInfoUrl { get; set; }

        public string TokenUrl { get; set; }

        public string LogoutUrl { get; set; }

        public string Callback { get; set; }

        public string LogoutCallback { get; set; }

        #endregion

        #region Private Properties
        private string BaseIdentityUrl
        {
            get { return _baseIdentityUrl; }
            set
            {
                _baseIdentityUrl = value;
                UpdateIdentityUrl(_baseIdentityUrl);
            }
        }

        private string BaseMobileServiceUrl
        {
            get { return _baseMobileServiceUrl; }
            set
            {
                _baseMobileServiceUrl = value;
                MobileServiceUrl = $"{_baseMobileServiceUrl}/api/";
            }
        }
        #endregion

        #region Private Methods

        private void UpdateIdentityUrl(string Url)
        {
            RegisterWebsite = $"{Url}/Account/Register";
            LogoutCallback = $"{Url}/Account/Redirecting";
            ResetPasswordUrl = $"{Url}/Account/PasswordReset";

            var connectBaseUrl = $"{Url}/connect";
            AuthorizeUrl = $"{connectBaseUrl}/authorize";
            UserInfoUrl = $"{connectBaseUrl}/userinfo";
            TokenUrl = $"{connectBaseUrl}/token";
            LogoutUrl = $"{connectBaseUrl}/endsession";

            var baseUri = ExtractBaseUri(Url);
            Callback = $"{baseUri}/xamarincallback";
        }

        private string ExtractBaseUri(string Url)
        {
            var uri = new Uri(Url);
            var baseUri = uri.GetLeftPart(System.UriPartial.Authority);

            return baseUri;
        }
        #endregion
    }
}
