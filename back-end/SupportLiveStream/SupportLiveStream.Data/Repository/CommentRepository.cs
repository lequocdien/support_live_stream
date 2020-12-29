using MongoDB.Driver;
using SupportLiveStream.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupportLiveStream.Data
{
    public interface ICommentRepository : IRepository<CommentModel>
    {

    }

    public class CommentRepository : MongoRepository<CommentModel>, ICommentRepository
    {
        public CommentRepository(IMongoContext context, string collectionId) : base(context, collectionId)
        {

        }

        public override async Task<IEnumerable<CommentModel>> GetAllAsync()
        {
            var res = _collection.Find<CommentModel>(Builders<CommentModel>.Filter.Empty).SortByDescending(e => e.CreatedTime).ToList();
            return res;
        }
    }
}
