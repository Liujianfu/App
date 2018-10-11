using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Statics;
using Xamarin.Auth;
using Xamarin.Utilities;

namespace EvvMobile.Authorization
{
    public class OpenIdAuthenticator: WebRedirectAuthenticator
    {
        private string _clientId;
        private string _clientSecret;
        private string _scope;
        private Uri _authorizeUrl;
        private Uri _accessTokenUrl;
        private Uri _redirectUrl;
        GetUsernameAsyncFunc _getUsernameAsync;

        string requestState;
        bool reportedForgery = false;
        /// <summary>
        /// Gets the redirect URL.
        /// WebRedirectAuthenticator doesn't have it?
        /// </summary>
        /// <value>The redirect URL.</value>
        public Uri RedirectUrl
        {
            get { return this._redirectUrl; }
        }
        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId
        {
            get { return this._clientId; }
        }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        public string ClientSecret
        {
            get { return this._clientSecret; }
        }

        /// <summary>
        /// Gets the authorization scope.
        /// </summary>
        /// <value>The authorization scope.</value>
        public string Scope
        {
            get { return this._scope; }
        }

        /// <summary>
        /// Gets the authorize URL.
        /// </summary>
        /// <value>The authorize URL.</value>
        public Uri AuthorizeUrl
        {
            get { return this._authorizeUrl; }
        }

        /// <summary>
        /// Gets the access token URL.
        /// </summary>
        /// <value>The URL used to request access tokens after an authorization code was received.</value>
        public Uri AccessTokenUrl
        {
            get { return this._accessTokenUrl; }
        }


        /// <summary>
        /// Initializes a new instance <see cref="Xamarin.Auth.OAuth2Authenticator"/>
        /// that authenticates using authorization codes (code).
        /// </summary>
        /// <param name='clientId'>
        /// Client identifier.
        /// </param>
        /// <param name='clientSecret'>
        /// Client secret.
        /// </param>
        /// <param name='scope'>
        /// Authorization scope.
        /// </param>
        /// <param name='authorizeUrl'>
        /// Authorize URL.
        /// </param>
        /// <param name='redirectUrl'>
        /// Redirect URL.
        /// </param>
        /// <param name='accessTokenUrl'>
        /// URL used to request access tokens after an authorization code was received.
        /// </param>
        /// <param name='getUsernameAsync'>
        /// Method used to fetch the username of an account
        /// after it has been successfully authenticated.
        /// </param>
        public OpenIdAuthenticator(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl, GetUsernameAsyncFunc getUsernameAsync = null)
			: this (redirectUrl, clientSecret, accessTokenUrl)
		{
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId must be provided", "clientId");
            }
            this._clientId = clientId;


            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("clientSecret must be provided", "clientSecret");
            }
            this._clientSecret = clientSecret;
            _redirectUrl = redirectUrl;
            this._scope = scope ?? "";

            if (authorizeUrl == null)
            {
                throw new ArgumentNullException("authorizeUrl");
            }
            this._authorizeUrl = authorizeUrl;

            this._accessTokenUrl = accessTokenUrl;

            this._getUsernameAsync = getUsernameAsync;
        }


        OpenIdAuthenticator(Uri redirectUrl, string clientSecret = null, Uri accessTokenUrl = null)
			: base (redirectUrl, redirectUrl)
		{
            _redirectUrl = redirectUrl;
            this._clientSecret = clientSecret;

            this._accessTokenUrl = accessTokenUrl;

            //
            // Generate a unique state string to check for forgeries
            //
            var chars = new char[16];
            var rand = new Random();
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)rand.Next((int)'a', (int)'z' + 1);
            }
            this.requestState = new string(chars);
        }

        bool IsImplicit { get { return _accessTokenUrl == null; } }

        /// <summary>
        /// Method that returns the initial URL to be displayed in the web browser.
        /// </summary>
        /// <returns>
        /// A task that will return the initial URL.
        /// </returns>
        public override Task<Uri> GetInitialUrlAsync()
        {
            var url = new Uri(string.Format(
                "{0}?client_id={1}&redirect_uri={2}&response_type={3}&scope={4}&state={5}",
                _authorizeUrl.AbsoluteUri,
                Uri.EscapeDataString(_clientId),
                Uri.EscapeDataString(RedirectUrl.AbsoluteUri),
                IsImplicit ? Uri.EscapeDataString(OpenIdAuthConstants.ImplicitResponseType) : Uri.EscapeDataString(OpenIdAuthConstants.ResponseType),//id_token
                Uri.EscapeDataString(_scope),
                Uri.EscapeDataString(requestState)));

            var tcs = new TaskCompletionSource<Uri>();
            tcs.SetResult(url);
            return tcs.Task;
        }

        /// <summary>
        /// Raised when a new page has been loaded.
        /// </summary>
        /// <param name='url'>
        /// URL of the page.
        /// </param>
        /// <param name='query'>
        /// The parsed query of the URL.
        /// </param>
        /// <param name='fragment'>
        /// The parsed fragment of the URL.
        /// </param>
        protected override void OnPageEncountered(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            var all = new Dictionary<string, string>(query);
            foreach (var kv in fragment)
                all[kv.Key] = kv.Value;

            //
            // Check for forgeries
            //
            if (all.ContainsKey("state"))
            {
                if (all["state"] != requestState && !reportedForgery)
                {
                    reportedForgery = true;
                    OnError("Invalid state from server. Possible forgery!");
                    return;
                }
            }

            //
            // Continue processing
            //
            base.OnPageEncountered(url, query, fragment);
        }

        /// <summary>
        /// Raised when a new page has been loaded.
        /// </summary>
        /// <param name='url'>
        /// URL of the page.
        /// </param>
        /// <param name='query'>
        /// The parsed query string of the URL.
        /// </param>
        /// <param name='fragment'>
        /// The parsed fragment of the URL.
        /// </param>
        protected override void OnRedirectPageLoaded(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            //
            // Look for the access_token
            //
            if (fragment.ContainsKey("access_token"))
            {
                //
                // We found an access_token
                //
                OnRetrievedAccountProperties(fragment);
            }
            else if (!IsImplicit)
            {
                //
                // Look for the code
                //
                if (query.ContainsKey("code"))
                {
                    var code = query["code"];
                    RequestAccessTokenAsync(code).ContinueWith(task => {
                        if (task.IsFaulted)
                        {
                            OnError(task.Exception);
                        }
                        else
                        {
                            OnRetrievedAccountProperties(task.Result);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    OnError("Expected code in response, but did not receive one.");
                    return;
                }
            }
            else
            {
                OnError("Expected access_token in response, but did not receive one.");
                return;
            }
        }

        /// <summary>
        /// Asynchronously requests an access token with an authorization <paramref name="code"/>.
        /// </summary>
        /// <returns>
        /// A dictionary of data returned from the authorization request.
        /// </returns>
        /// <param name='code'>The authorization code.</param>

        Task<IDictionary<string, string>> RequestAccessTokenAsync(string code)
        {
            var queryValues = new Dictionary<string, string> {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", RedirectUrl.AbsoluteUri },
                { "client_id", _clientId },
            };
            if (!string.IsNullOrEmpty(_clientSecret))
            {
                queryValues["client_secret"] = _clientSecret;
            }

            return RequestAccessTokenAsync(queryValues);
        }
        public  Task<int> RequestRefreshTokenAsync(string refreshToken)
        {
            var queryValues = new Dictionary<string, string>
            {
                {"refresh_token", refreshToken},
                {"client_id", _clientId},
                {"grant_type", "refresh_token"}
            };

            if (!string.IsNullOrEmpty(_clientSecret))
            {
                queryValues["client_secret"] = _clientSecret;
            }

            return RequestAccessTokenAsync(queryValues)
                    .ContinueWith(result =>
                    {
                        var accountProperties = result.Result;

                        OnRetrievedAccountProperties(accountProperties);

                        return int.Parse(accountProperties["expires_in"]);
                    });
        }
        /// <summary>
        /// Asynchronously makes a request to the access token URL with the given parameters.
        /// </summary>
        /// <param name="queryValues">The parameters to make the request with.</param>
        /// <returns>The data provided in the response to the access token request.</returns>
        protected async Task<IDictionary<string, string>> RequestAccessTokenAsync(IDictionary<string, string> queryValues)
        {
            var query = queryValues.FormEncode();
            var httpClient = new HttpClient();

            if (null ==_accessTokenUrl)
            {
                _accessTokenUrl = _authorizeUrl;
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _accessTokenUrl);
            request.Content =  new StringContent(query, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpClient.SendAsync(request);
            var text = await response.Content.ReadAsStringAsync();

            var data = text.Contains("{") ? WebEx.JsonDecode(text) : WebEx.FormDecode(text);

            if (data.ContainsKey("error"))
            {
                throw new AuthException("Error authenticating: " + data["error"]);
            }
            else if (data.ContainsKey("access_token"))
            {
                return data;
            }
            else
            {
                throw new AuthException("Expected access_token in access token response, but did not receive one.");
            }
        }

        /// <summary>
        /// Event handler that is fired when an access token has been retreived.
        /// </summary>
        /// <param name='accountProperties'>
        /// The retrieved account properties
        /// </param>
        protected virtual void OnRetrievedAccountProperties(IDictionary<string, string> accountProperties)
        {
            //
            // Now we just need a username for the account
            //
            if (_getUsernameAsync != null)
            {
                _getUsernameAsync(accountProperties).ContinueWith(task => {
                    if (task.IsFaulted)
                    {
                        OnError(task.Exception);
                    }
                    else
                    {
                        OnSucceeded(task.Result, accountProperties);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                OnSucceeded("", accountProperties);
            }
        }
    }

}

