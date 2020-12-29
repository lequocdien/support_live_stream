using SupportLiveStream.Model;

namespace SupportLiveStream.Data
{
    public interface IPhoneRepository : IRepository<PhoneModel>
    {

    }

    public class PhoneRepository : MongoRepository<PhoneModel>, IPhoneRepository
    {
        public PhoneRepository(IMongoContext context, string liveId) : base(context, liveId)
        {

        }
    }
}
