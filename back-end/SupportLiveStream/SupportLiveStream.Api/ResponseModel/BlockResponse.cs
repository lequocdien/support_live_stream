using Newtonsoft.Json;
using System.Collections.Generic;

namespace SupportLiveStream.Api.ResponseModel
{
    public class BlockResponse
    {
        [JsonProperty("data")]
        public List<DataBlockResp> Data { get; set; }
    }

    public class DataBlockResp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
