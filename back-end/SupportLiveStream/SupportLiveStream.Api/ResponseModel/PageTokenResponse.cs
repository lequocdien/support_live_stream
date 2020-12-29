using Newtonsoft.Json;
using System.Collections.Generic;

namespace SupportLiveStream.Api.ResponseModel
{
    public class PageTokenResponse
    {
        [JsonProperty("data")]
        public List<PageTokenData> Data { get; set; }
    }

    public class PageTokenData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
