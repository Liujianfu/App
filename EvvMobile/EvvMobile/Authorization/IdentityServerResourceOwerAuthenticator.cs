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
    public class IdentityServerResourceOwerAuthenticator
    {

        public IdentityServerResourceOwerAuthenticator(string clientId, string clientSecret, string scope, GetUsernameAsyncFunc getUsernameAsync = null)
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
            this._scope = scope ?? "";

            _client = new HttpClient();
        }


        public bool IsAuthenticated {
            get { return _isAuthenticated; }
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



        public IDictionary<string, string> Tokens
        {
            get { return _authenticationResults; }
        }

        public async Task<IDictionary<string, string>> LoginForResourceOwner(string userName, string passWord)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("userName must be provided", "userName");
            }
            if (string.IsNullOrEmpty(passWord))
            {
                throw new ArgumentException("passWord must be provided", "passWord");
            }
            _authenticationResults.Clear();
            _userName = userName;
            _passWord = passWord;
            var loginCredential = new Dictionary<string, string>();
            loginCredential.Add("Username", _userName);
            loginCredential.Add("Password", _passWord);
            loginCredential.Add("grant_type", "password");
            loginCredential.Add("client_id", IdentityServer3OpenIdAuthConstants.ClientId);
            loginCredential.Add("client_secret", _clientSecret);
            loginCredential.Add("scope", IdentityServer3OpenIdAuthConstants.Scope);
            var loginUrl = IdentityServer3OpenIdAuthConstants.AccessTokenUrl; 
            var signInResponse = await PostForm(loginUrl, loginCredential);
            if (signInResponse != null && signInResponse.StatusCode == HttpStatusCode.OK)
            {
                var text = signInResponse.Content.ReadAsStringAsync().Result;

                // Parse the response
                var data = text.Contains("{") ? WebEx.JsonDecode(text) : WebEx.FormDecode(text);


                if (data.ContainsKey("access_token"))
                {
                    //
                    // We found an access_token
                    //
                    _authenticationResults.Add("access_token", data["access_token"]);
                    _isAuthenticated = true;
                }
                if (data.ContainsKey("id_token"))
                {
                    //
                    // We found an id_token
                    //
                    _authenticationResults.Add("id_token", data["id_token"]);
                    _isAuthenticated = true;
                }
                if (data.ContainsKey("refresh_token"))
                {
                    //
                    // We found an refresh_token
                    //
                    _authenticationResults.Add("refresh_token", data["refresh_token"]);
                }

            }
            return _authenticationResults;
        }



        protected string ToFormBody(IDictionary<string, string> coll)
        {
            var sb = new StringBuilder();
            foreach (var item in coll)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.AppendFormat("{0}={1}", item.Key, item.Value);
            }
            return sb.ToString();
        }
        public async Task<int> RequestRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshToken = _authenticationResults["refresh_token"];
            }
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

            var results= await RequestAccessTokenAsync(queryValues);
            if (results.ContainsKey("access_token"))
            {
                //
                // We found an access_token
                //
                _authenticationResults.Remove("access_token");
                _authenticationResults.Add("access_token", results["access_token"]);
                _isAuthenticated = true;
            }
            if (results.ContainsKey("id_token"))
            {
                //
                // We found an id_token
                //
                _authenticationResults.Remove("id_token");
                _authenticationResults.Add("id_token", results["id_token"]);
                _isAuthenticated = true;
            }
            if (results.ContainsKey("refresh_token"))
            {
                //
                // We found an refresh_token
                //
                _authenticationResults.Remove("refresh_token");
                _authenticationResults.Add("refresh_token", results["refresh_token"]);
            }

            return int.Parse(results["expires_in"]);
        }
        protected async Task<IDictionary<string, string>> RequestAccessTokenAsync(IDictionary<string, string> queryValues)
        {
            var query = queryValues.FormEncode();
            var httpClient = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, IdentityServer3OpenIdAuthConstants.AccessTokenUrl);
            request.Content = new StringContent(query, Encoding.UTF8, "application/x-www-form-urlencoded");
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

        protected async Task<HttpResponseMessage> PostForm(string url, IDictionary<string, string>  value, bool includeCsrf = true)
        {


            try
            {
                var body = ToFormBody(value);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                
                var response = await _client.SendAsync(request);
                return response;
            }
            catch (Exception e)
            {
                return null;
            }
        }





        private string _clientId;
        private string _clientSecret;
        private string _scope;
        private bool _isAuthenticated;
        private HttpClient _client;
        private string _userName;
        private string _passWord;
        private IDictionary<string, string> _authenticationResults = new Dictionary<string, string>();

    }
}
