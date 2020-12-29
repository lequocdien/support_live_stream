namespace SupportLiveStream.Web
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }

        public string Data { get; set; }

        public ResponseModel(int statusCode, string data)
        {
            StatusCode = statusCode;
            Data = data;
        }
    }
}
