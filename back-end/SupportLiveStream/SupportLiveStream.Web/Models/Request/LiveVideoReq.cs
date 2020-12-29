using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace SupportLiveStream.Web.Models
{
    public class LiveVideoReq
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("entry")]
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("changes")]
        public List<Change> Changes { get; set; }
    }

    public class Change
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("value")]
        public Value Value { get; set; }
    }

    public class Value
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
