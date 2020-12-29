using SupportLiveStream.Model;
using System;

namespace SupportLiveStream.Data
{
    public interface IVideoDetailRepository : IRepository<VideoDetailModel>
    {

    }

    public class VideoDetailRepository : MongoRepository<VideoDetailModel>, IVideoDetailRepository
    {
        public VideoDetailRepository(IMongoContext context, string videoId) : base(context, String.Format("VideoDetail_{0}", videoId))
        {

        }
    }
}
