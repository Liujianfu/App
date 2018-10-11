namespace EvvMobile.RestfulWebService.Infrastructure.Common
{
    #region HttpWebApiResponse
    public class HttpWebApiResponse
    {
        public HttpWebApiResponse()
        {
            ResponseStatus = ResponseStatuses.Success;
        }
        public string ResponseStatus { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsFalied { get { return ResponseStatus != ResponseStatuses.Success; } }
    }
    #endregion

    #region HttpWebApiResponse<T>
    public class HttpWebApiResponse<T>
    {
        public HttpWebApiResponse()
        {
            ResponseStatus = ResponseStatuses.Success;
        }

        public HttpWebApiResponse(T modelObjet)
        {
            ModelObject = modelObjet;
            ResponseStatus = ResponseStatuses.Success;
        }

        public T ModelObject { get; set; }

        public string ResponseStatus { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsFalied { get { return ResponseStatus != ResponseStatuses.Success; } }
    }
    #endregion

} 
   
