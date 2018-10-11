using System.Net.Http;

namespace EvvMobile.RestfulWebService.Infrastructure.Extensions
{
    public interface IHttpClientManager
    {
        HttpClient GetOrAdd(string baseUrl);
        void Clear();
    }
}
