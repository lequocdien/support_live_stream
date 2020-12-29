using Newtonsoft.Json;
using System;

namespace SupportLiveStream.Api.ResponseModel
{
    public class StreamingResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
    }

    public class From
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
