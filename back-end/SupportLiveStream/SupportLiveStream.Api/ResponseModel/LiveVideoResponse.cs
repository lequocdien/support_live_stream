using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SupportLiveStream.Api.ResponseModel
{
    public class LiveVideoResponse
    {
        [JsonProperty("data")]
        public List<LiveVideoDataResp> Data { get; set; }
    }

    public class LiveVideoDataResp
    {
        [JsonProperty("id")]
        public string VideoId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("permalink_url")]
        public string PermalinkUrl { get; set; }

        [JsonProperty("live_views")]
        public string LiveViews { get; set; }

        [JsonProperty("creation_time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("video")]
        public VideoInsideResp Video { get; set; }
    }

    public class VideoInsideResp
    {
        [JsonProperty("id")]
        public string DisplayVideoId { get; set; }
    }
}
