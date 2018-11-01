using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CareVisit.Core.Exceptions;

namespace CareVisit.Core.Services.RestClients
{
    public class RestClient : IRestClient
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public RestClient()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        public async Task<TResult> GetAsync<TResult>(string uri, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();
            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));
            return result;
        }

        public async Task<RestResponse<T>> GetDataAsync<T>(string uri, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();
            var restResponse = await Task.Run(() =>
                JsonConvert.DeserializeObject<RestResponse<T>>(serialized, _serializerSettings));
            if (restResponse.HasError)
            {
                throw new HttpRequestExceptionEx(HttpStatusCode.InternalServerError, restResponse.Error);
            }
            return restResponse;
        }

        public async Task<TResult> PostAsync<TResult>(string uri, object data, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);
   
            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();
            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));
            return result;
        }

        public async Task<RestResponse<T>> PostDataAsync<T>(string uri, object data, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);

            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();
            var restResponse = await Task.Run(() =>
                JsonConvert.DeserializeObject<RestResponse<T>>(serialized, _serializerSettings));
            if (restResponse.HasError)
            {
                throw new HttpRequestExceptionEx(HttpStatusCode.InternalServerError, restResponse.Error);
            }
            return restResponse;
        }

        #region Private Helpers
        private HttpClient CreateHttpClient(string token = "")
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return httpClient;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(content);
                }
                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        } 
        #endregion
    }
}
