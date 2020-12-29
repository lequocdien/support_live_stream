using MongoDB.Bson;

namespace SupportLiveStream.Model
{
    public interface IMongoModelBase
    {
        ObjectId Id { get; set; }
    }
}
