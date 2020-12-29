using SupportLiveStream.Api;
using SupportLiveStream.Api.RequestModel;
using System;
using System.Threading.Tasks;

namespace SupportLiveStream.Service
{
    public interface ICommentService
    {
        Task<bool> SendPrivateReplyAsync(PrivateReplyReq body, string token);

        Task<bool> DeleteCommentAsync(string commentId, string token);
        
        Task<bool> HiddenCommentAsync(string commentId, string token, bool isHidden = true);
    }

    public class CommentService : ICommentService
    {
        private readonly IFacebookApi _facebookApi;

        public CommentService(IFacebookApi facebookApi)
        {
            _facebookApi = facebookApi;
        }

        public async Task<bool> SendPrivateReplyAsync(PrivateReplyReq body, string token)
        {
            try
            {
                return await _facebookApi.SendPrivateReplyAsync(body, token);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCommentAsync(string commentId, string token)
        {
            try
            {
                return await _facebookApi.DeleteCommentAsync(commentId, token);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> HiddenCommentAsync(string commentId, string token, bool isHidden = true)
        {
            try
            {
                return await _facebookApi.HiddenCommentAsync(commentId, token, isHidden);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
