using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CareVisit.Core.Helpers
{
    public class IdentityServerResourceOwerAuthenticator
    {
        private string AuthClientId = "A8375B66";//should get from back end
        private string AuthClientSecret = "A32D8C3CBE9A";//should get from back end
        private string AuthScope = "openid offline_access";//offline_access is for Identity Server, to get refresh token
        private string AccessTokenUrl = "https://jianfuliult4.fei.local/Identity/connect/token";

        public IdentityServerResourceOwerAuthenticator(string clientId, string clientSecret, string scope)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId must be provided", nameof(clientId));
            }
            this.AuthClientId = clientId;
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("clientSecret must be provided", nameof(clientSecret));
            }
            this.AuthClientSecret = clientSecret;
            this.AuthScope = scope ?? "";

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
            get { return this.AuthClientId; }
        }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        public string ClientSecret
        {
            get { return this.AuthClientSecret; }
        }

        /// <summary>
        /// Gets the authorization scope.
        /// </summary>
        /// <value>The authorization scope.</value>
        public string Scope
        {
            get { return this.AuthScope; }
        }



        public LoginToken Tokens
        {
            get { return _authenticationResult; }
        }

        public async Task<LoginToken> LoginForResourceOwner(string userName, string passWord)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("userName must be provided", "userName");
            }
            if (string.IsNullOrEmpty(passWord))
            {
                throw new ArgumentException("passWord must be provided", "passWord");
            }

            _userName = userName;
            _passWord = passWord;
            var loginCredential = new Dictionary<string, string>();
            loginCredential.Add("Username", _userName);
            loginCredential.Add("Password", _passWord);
            loginCredential.Add("grant_type", "password");
            loginCredential.Add("client_id", ClientId);
            loginCredential.Add("client_secret", ClientSecret);
            loginCredential.Add("scope", Scope);
            var loginUrl = AccessTokenUrl; 
            var signInResponse = await PostForm(loginUrl, loginCredential);
            if (signInResponse != null && signInResponse.IsSuccessStatusCode)
            {
                var text = signInResponse.Content.ReadAsStringAsync().Result;

                // Parse the response
                var data = JsonConvert.DeserializeObject<LoginToken>(text);
                _authenticationResult = data;

                if (!string.IsNullOrWhiteSpace(data.access_token))
                {
                    //
                    // We found an access_token
                    //
                    _isAuthenticated = true;
                }
                if (!string.IsNullOrWhiteSpace(data.id_token))
                {
                    //
                    // We found an id_token
                    //

                    _isAuthenticated = true;
                }


            }
            return _authenticationResult;
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
                refreshToken = _authenticationResult.refresh_token;
            }
            var queryValues = new Dictionary<string, string>
            {
                {"refresh_token", refreshToken},
                {"client_id", ClientId},
                {"grant_type", "refresh_token"}
            };

            if (!string.IsNullOrEmpty(ClientSecret))
            {
                queryValues["client_secret"] = ClientSecret;
            }

            var results= await RequestAccessTokenAsync(queryValues);

            if(results!=null){
                _authenticationResult = results;
                if (!string.IsNullOrWhiteSpace(results.access_token))
                {
                    //
                    // We found an access_token
                    //

                    _isAuthenticated = true;
                }
                if (!string.IsNullOrWhiteSpace(results.id_token))
                {
                    //
                    // We found an id_token
                    //

                    _isAuthenticated = true;
                }


                return results.expires_in;
            }
            else
            {
                _isAuthenticated = false;
            }
            return 0;
        }
        protected async Task<LoginToken> RequestAccessTokenAsync(IDictionary<string, string> queryValues)
        {

            var signInResponse = await PostForm(AccessTokenUrl, queryValues);
            if (signInResponse != null && signInResponse.IsSuccessStatusCode)
            {
                var text = signInResponse.Content.ReadAsStringAsync().Result;

                // Parse the response
                var data = JsonConvert.DeserializeObject<LoginToken>(text);
                if (!string.IsNullOrWhiteSpace(data.error))
                {
                    //throw new Exception("Error authenticating: " + data.error);
                }
                else if (!string.IsNullOrWhiteSpace(data.access_token))
                {
                    return data;
                }
                else
                {
                    //throw new Exception("Expected access_token in access token response, but did not receive one.");
                }
            }
            return null;
        }

        protected async Task<HttpResponseMessage> PostForm(string url, IDictionary<string, string>  value, bool includeCsrf = true)
        {


            try
            {
                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var body = ToFormBody(value);
                 
                var response = await httpClient.PostAsync(AccessTokenUrl, new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }






        private bool _isAuthenticated;

        private string _userName;
        private string _passWord;
        private LoginToken _authenticationResult=new LoginToken();

    }
}
