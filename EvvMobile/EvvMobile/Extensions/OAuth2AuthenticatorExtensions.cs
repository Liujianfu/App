using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace EvvMobile.Extensions
{
    public static class OAuth2AuthenticatorExtensions
    {
		public static Task<int> RequestRefreshTokenAsync(this OAuth2Authenticator authenticator, string refreshToken)
        {
            var queryValues = new Dictionary<string, string>
            {
                {"refresh_token", refreshToken},
                {"client_id", authenticator.ClientId},
                {"grant_type", "refresh_token"}
            };

            if (!string.IsNullOrEmpty(authenticator.ClientSecret))
            {
                queryValues["client_secret"] = authenticator.ClientSecret;
            }

            return authenticator.RequestAccessTokenAsync(queryValues)
                    .ContinueWith(result =>
                    {
                        var accountProperties = result.Result;

                        authenticator.OnRetrievedAccountProperties(accountProperties);

                        return int.Parse(accountProperties["expires_in"]);
                    });
        }


    }
}
