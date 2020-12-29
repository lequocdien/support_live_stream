using SupportLiveStream.Model;
namespace SupportLiveStream.Data
{
    public interface IAccountRepository : IRepository<AccountModel>
    {

    }

    public class AccountRepository : MongoRepository<AccountModel>, IAccountRepository
    {
        public AccountRepository(IMongoContext context) : base(context, "ACCOUNT")
        {

        }
    }
}
