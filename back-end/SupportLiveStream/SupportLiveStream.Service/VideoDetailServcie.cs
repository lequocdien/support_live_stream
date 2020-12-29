using MongoDB.Driver;
using SupportLiveStream.Data;
using SupportLiveStream.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupportLiveStream.Service
{
    public interface IVideoDetailService
    {
        Task InsertOneAsync(VideoDetailModel model);
        Task InsertManyAsync(List<VideoDetailModel> lst);
        Task DeleteOneAsync(FilterDefinition<VideoDetailModel> filter);
        Task<IEnumerable<VideoDetailModel>> FindAsync(Func<FilterDefinition<VideoDetailModel>> funcFilter = null, Func<SortDefinition<VideoDetailModel>> funcSort = null);
        Task UpdateOneAsync(Func<FilterDefinition<VideoDetailModel>> funcFilter, Func<UpdateDefinition<VideoDetailModel>> funcUpdate);
    }

    public class VideoDetailServcie : IVideoDetailService
    {
        private IVideoDetailRepository _videoDetailRepo = null;

        public VideoDetailServcie(Func<string, IVideoDetailRepository> func, string collectionId)
        {
            _videoDetailRepo = func(collectionId);
        }

        public async Task DeleteOneAsync(FilterDefinition<VideoDetailModel> filter)
        {
            await _videoDetailRepo.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<VideoDetailModel>> FindAsync(Func<FilterDefinition<VideoDetailModel>> funcFilter = null, Func<SortDefinition<VideoDetailModel>> funcSort = null)
        {
            return await _videoDetailRepo.FindAsync(funcFilter, funcSort);
        }

        public async Task InsertManyAsync(List<VideoDetailModel> lst)
        {
            await _videoDetailRepo.InsertManyAsync(lst);
        }

        public async Task InsertOneAsync(VideoDetailModel model)
        {
            await _videoDetailRepo.InsertOneAsync(model);
        }

        public async Task UpdateOneAsync(Func<FilterDefinition<VideoDetailModel>> funcFilter, Func<UpdateDefinition<VideoDetailModel>> funcUpdate)
        {
            await _videoDetailRepo.UpdateOneAsync(funcFilter, funcUpdate);
        }
    }
}
