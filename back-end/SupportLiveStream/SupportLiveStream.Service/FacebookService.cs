using SupportLiveStream.Api;
using SupportLiveStream.Api.ResponseModel;
using System;
using System.Threading.Tasks;

namespace SupportLiveStream.Service
{
    public interface IFacebookService
    {
        Task StreamingAsync(string strId, string strToken);
        Task UpdateCommentAsync(string strId, string strToken);
        Task<bool> SubscribeLiveVideoAsync(string strToken);
        Task<PageInfoResponse> GetPageInfoAsync(string strPageToken);
        Task<LiveVideoResponse> GetLiveVideoOfPageAsync(string strPageToken);
        Task<BlockResponse> GetListBlock(string strToken);
        Task<bool> BlockUser(string fbId, string strToken);
        Task<bool> UnblockUser(string fbId, string strToken);
    }

    public class FacebookService : IFacebookService
    {
        private IFacebookApi _facebookApi;

        public FacebookService(IFacebookApi facebookApi)
        {
            _facebookApi = facebookApi;
        }

        public Task<bool> SubscribeLiveVideoAsync(string strToken)
        {
            return _facebookApi.SubscribeLiveVideoAsync(strToken);
        }

        public Task StreamingAsync(string strId, string strToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCommentAsync(string strId, string strToken)
        {
            throw new NotImplementedException();
        }

        public Task<PageInfoResponse> GetPageInfoAsync(string strPageToken)
        {
            return _facebookApi.GetPageInfoAsync(strPageToken);
        }

        public Task<LiveVideoResponse> GetLiveVideoOfPageAsync(string strPageToken)
        {
            return _facebookApi.GetLiveVideoOfPageAsync(strPageToken);
        }

        public async Task<BlockResponse> GetListBlock(string strToken)
        {
            return await _facebookApi.GetListBlock(strToken);
        }

        public async Task<bool> BlockUser(string fbId, string strToken)
        {
            return await _facebookApi.BlockUser(fbId, strToken);
        }

        public async Task<bool> UnblockUser(string fbId, string strToken)
        {
            return await _facebookApi.UnblockUser(fbId, strToken);
        }
    }
}
