using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SupportLiveStream.Model
{
    public class VideoDetailModel : MongoModelBase<VideoDetailModel>, IMongoModelBase
    {
        [BsonElement("FbId")]
        [JsonProperty("FbId")]
        public string FbId { get; set; }

        [BsonElement("FbName")]
        [JsonProperty("FbName")]
        public string FbName { get; set; }

        [BsonElement("Phones")]
        [JsonProperty("Phones")]
        public List<string> Phones { get; set; }

        [BsonElement("Comments")]
        [JsonProperty("Comments")]
        public List<CommentModel> Comments { get; set; }

        [BsonElement("CreatedTime")]
        [JsonProperty("CreatedTime")]
        public DateTime CreatedTime { get; set; }

        public VideoDetailModel()
        {
            Phones = new List<string>();
            Comments = new List<CommentModel>();
            CreatedTime = DateTime.UtcNow;
        }
    }

    public class CommentModel
    {
        [BsonElement("CommentId")]
        [JsonProperty("CommentId")]
        public string CommentId { get; set; }

        [BsonElement("Message")]
        [JsonProperty("Message")]
        public string Message { get; set; }

        [BsonElement("IsSendWhenDetectedPhoneResult")]
        [JsonProperty("IsSendWhenDetectedPhoneResult")]
        public bool IsSendWhenDetectedPhoneResult { get; set; }

        [BsonElement("IsSendWhenDetectedGoodWordResult")]
        [JsonProperty("IsSendWhenDetectedGoodWordResult")]
        public bool IsSendWhenDetectedGoodWordResult { get; set; }

        [BsonElement("IsDeleteWhenDetectedBadWordResult")]
        [JsonProperty("IsDeleteWhenDetectedBadWordResult")]
        public bool IsDeleteWhenDetectedBadWordResult { get; set; }

        [BsonElement("IsHiddenResult")]
        [JsonProperty("IsHiddenResult")]
        public bool IsHiddenResult { get; set; }

        [BsonElement("CreatedTime")]
        [JsonProperty("CreatedTime")]
        public DateTime CreatedTime { get; set; }
    }
}
