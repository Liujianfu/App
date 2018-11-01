
using System.Threading.Tasks;

namespace CareVisit.Core.Services.RestClients
{
    public interface IRestClient
    {
        Task<TResult> GetAsync<TResult>(string uri, string token = "");

        Task<TResult> PostAsync<TResult>(string uri, object data, string token = "");

        Task<RestResponse<T>> GetDataAsync<T>(string uri, string token = "");

        Task<RestResponse<T>> PostDataAsync<T>(string uri, object data, string token = "");
    }
}
