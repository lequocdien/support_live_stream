using MongoDB.Driver;

namespace SupportLiveStream.Data
{
    public interface IMongoContext
    {
        IMongoDatabase Database { get; }
    }

    public class MongoContext : IMongoContext
    {
        private MongoClient _mongoClient { get; }

        public IMongoDatabase Database { get; }

        public MongoContext()
        {
            //if (_mongoClient == null)
            //{
            //    _mongoClient = new MongoClient("mongodb://localhost:27017");
            //}
            //Database = _mongoClient.GetDatabase("support_live_stream");
            if (_mongoClient == null)
            {
                _mongoClient = new MongoClient("mongodb+srv://admin:0000@cluster0.7gnk1.mongodb.net/test");
            }
            
            Database = _mongoClient.GetDatabase("SupportLiveStream");
        }
    }
}
