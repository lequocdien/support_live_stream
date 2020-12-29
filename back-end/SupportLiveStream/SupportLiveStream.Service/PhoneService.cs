using SupportLiveStream.Data;
using SupportLiveStream.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupportLiveStream.Service
{
    public interface IPhoneService
    {
        Task<PhoneModel> AddAsync(PhoneModel commentModel);
        Task<IEnumerable<PhoneModel>> GetAllAsync();
        Task<PhoneModel> GetByIdAsync(string id);
    }

    public class PhoneService : IPhoneService
    {
        private IPhoneRepository _phoneRepository = null;

        public PhoneService(Func<string, IPhoneRepository> func, string liveId)
        {
            _phoneRepository = func(liveId);
        }

        public async Task<PhoneModel> AddAsync(PhoneModel phoneModel)
        {
            return await _phoneRepository.AddAsync(phoneModel);
        }

        public async Task<IEnumerable<PhoneModel>> GetAllAsync()
        {
            return await _phoneRepository.GetAllAsync();
        }

        public Task<PhoneModel> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
