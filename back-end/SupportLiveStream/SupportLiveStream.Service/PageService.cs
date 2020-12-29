using MongoDB.Driver;
using SupportLiveStream.Data;
using SupportLiveStream.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupportLiveStream.Service
{
    public interface IPageService
    {
        Task InsertOneAsync(PageModel model);
        Task InsertManyAsync(List<PageModel> lst);
        Task DeleteOneAsync(FilterDefinition<PageModel> filter);
        Task<IEnumerable<PageModel>> FindAsync(Func<FilterDefinition<PageModel>> funcFilter = null, Func<SortDefinition<PageModel>> funcSort = null);
        Task UpdateOneAsync(Func<FilterDefinition<PageModel>> funcFilter, Func<UpdateDefinition<PageModel>> funcUpdate);
    }

    public class PageService : IPageService
    {
        private IPageRepository _pageRepository = null;

        public PageService(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task DeleteOneAsync(FilterDefinition<PageModel> filter)
        {
            await _pageRepository.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<PageModel>> FindAsync(Func<FilterDefinition<PageModel>> funcFilter = null, Func<SortDefinition<PageModel>> funcSort = null)
        {
            return await _pageRepository.FindAsync(funcFilter, funcSort);
        }

        public async Task InsertManyAsync(List<PageModel> lst)
        {
            await _pageRepository.InsertManyAsync(lst);
        }

        public async Task InsertOneAsync(PageModel model)
        {
            await _pageRepository.InsertOneAsync(model);
        }

        public async Task UpdateOneAsync(Func<FilterDefinition<PageModel>> funcFilter, Func<UpdateDefinition<PageModel>> funcUpdate)
        {
            await _pageRepository.UpdateOneAsync(funcFilter, funcUpdate);
        }
    }
}
