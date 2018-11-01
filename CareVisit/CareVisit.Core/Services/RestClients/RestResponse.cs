
namespace CareVisit.Core.Services.RestClients
{
    public class RestResponse
    {
        public RestResponse()
        {
        }

        public string Error { get; set; }

        public string Message { get; set; }

        public bool HasError
        {
            get
            {
                return string.IsNullOrEmpty(Error);
            }
        }
    }

    public class RestResponse<T>
    {
        public RestResponse(T result)
        {
            Result = result;
        }

        public string Error { get; set; }

        public string Message { get; set; }

        public T Result { get; set; }

        public bool HasError
        {
            get
            {
                return string.IsNullOrEmpty(Error);
            }
        }
    }

}
