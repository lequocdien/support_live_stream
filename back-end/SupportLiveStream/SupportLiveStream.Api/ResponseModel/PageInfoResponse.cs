using Newtonsoft.Json;
using System.Collections.Generic;

namespace SupportLiveStream.Api.ResponseModel
{
    public class PageInfoResponse
    {
        [JsonProperty("id")]
        public string PageId { get; set; }

        [JsonProperty("name")]
        public string PageName { get; set; }

        [JsonProperty("category_list")]
        public List<CategoryResponse> Categories { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("single_line_address")]
        public string Address { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("emails")]
        public List<string> Emails { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("fan_count")]
        public int FanCount { get; set; }

        [JsonProperty("new_like_count")]
        public int NewLikeCount { get; set; }

        [JsonProperty("checkins")]
        public int CheckinCount { get; set; }

        [JsonProperty("can_checkin")]
        public bool CanCheckin { get; set; }

        [JsonProperty("can_post")]
        public bool CanPost { get; set; }
    }

    public class CategoryResponse
    {
        [JsonProperty("id")]
        public string CategoryId { get; set; }

        [JsonProperty("name")]
        public string CategoryName { get; set; }
    }
}
