using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SupportLiveStream.Model
{
    public abstract class MongoModelBase<TDocument> : IMongoModelBase
        where TDocument : class, IMongoModelBase
    {
        [BsonId]
        [BsonElement("_id")]
        [JsonProperty("Id")]
        public ObjectId Id { get; set; }

        public MongoModelBase()
        {
            //Id = ObjectId.GenerateNewId();
        }
    }
}
