using MongoDB.Driver;
using SupportLiveStream.Data;
using SupportLiveStream.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupportLiveStream.Service
{
    public interface IAccountService
    {
        Task InsertOneAsync(AccountModel model);
        Task InsertManyAsync(List<AccountModel> lst);
        Task DeleteOneAsync(FilterDefinition<AccountModel> filter);
        Task<IEnumerable<AccountModel>> FindAsync(Func<FilterDefinition<AccountModel>> funcFilter = null, Func<SortDefinition<AccountModel>> funcSort = null);
        Task UpdateOneAsync(Func<FilterDefinition<AccountModel>> funcFilter, Func<UpdateDefinition<AccountModel>> funcUpdate);
    }

    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task DeleteOneAsync(FilterDefinition<AccountModel> filter)
        {
            await _accountRepository.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<AccountModel>> FindAsync(Func<FilterDefinition<AccountModel>> funcFilter = null, Func<SortDefinition<AccountModel>> funcSort = null)
        {
            return await _accountRepository.FindAsync(funcFilter, funcSort);
        }

        public async Task InsertManyAsync(List<AccountModel> lst)
        {
            await _accountRepository.InsertManyAsync(lst);
        }

        public async Task InsertOneAsync(AccountModel model)
        {
            await _accountRepository.InsertOneAsync(model);
        }

        public async Task UpdateOneAsync(Func<FilterDefinition<AccountModel>> funcFilter, Func<UpdateDefinition<AccountModel>> funcUpdate)
        {
            await _accountRepository.UpdateOneAsync(funcFilter, funcUpdate);
        }
    }
}
