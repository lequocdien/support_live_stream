using Newtonsoft.Json;
namespace SupportLiveStream.Api.ResponseModel
{
    public class OauthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
