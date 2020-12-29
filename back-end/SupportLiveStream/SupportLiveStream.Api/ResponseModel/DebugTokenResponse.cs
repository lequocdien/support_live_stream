using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace SupportLiveStream.Api.ResponseModel
{
    public class DebugTokenResponse
    {
        [JsonProperty("data")]
        public DebugTokenData Data { get; set; }
    }

    public class DebugTokenData
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("type")]
        public TypeToken Type { get; set; }

        [JsonProperty("application")]
        public string Application { get; set; }
        
        [JsonProperty("profile_id")]
        public string ProfileId { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("expires_at")]
        public int ExpiresAt { get; set; }

        [JsonProperty("issued_at")]
        public int IssuedAt { get; set; }

        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }
    }

    public enum TypeToken
    {
        [Description("USER")]
        USER = 0,

        [Description("PAGE")]
        PAGE = 1
    }
}
