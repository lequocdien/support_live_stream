using MongoDB.Driver;
using SupportLiveStream.Api;
using SupportLiveStream.Api.RequestModel;
using SupportLiveStream.Api.ResponseModel;
using SupportLiveStream.Data;
using SupportLiveStream.Model;
using SupportLiveStream.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SupportLiveStream.Service
{
    public interface IPipelineService
    {
        Task StreamingPipelineAsync(IVideoDetailService videoDetailService, string Id, string Token);
        Task<bool> OauthPiplineAsync(string strCode, string username = null);
    }

    public class PipelineService : IPipelineService
    {
        private IFacebookApi _facebookApi;
        private ISentimentNLPApi _sentimentNLPApi;
        private IAccountRepository _accountRepo;
        private IPageRepository _pageRepository;

        public PipelineService(IFacebookApi facebookApi, ISentimentNLPApi sentimentNLPApi, IAccountRepository accountRepository, IPageRepository pageRepository)
        {
            _facebookApi = facebookApi;
            _sentimentNLPApi = sentimentNLPApi;
            _accountRepo = accountRepository;
            _pageRepository = pageRepository;
        }

        public async Task<bool> OauthPiplineAsync(string strCode, string username)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(strCode))
            {
                return false;
            }
            try
            {
                PageInfoResponse objPageInfo;
                List<UserTokenModel> lstUserToken = new List<UserTokenModel>();
                List<PageTokenModel> lstPageToken = new List<PageTokenModel>();
                List<PageModel> lstPageInfo = new List<PageModel>();

                // 1. USER Token
                OauthResponse objUserToken = await _facebookApi.OauthAsync(strCode);
                // 1.1. USER Token Debug
                DebugTokenResponse objUserDebugToken = await _facebookApi.DebugTokenAsync(objUserToken.AccessToken);
                lstUserToken.Add(new UserTokenModel()
                {
                    AppId = objUserDebugToken.Data.AppId,
                    UserId = objUserDebugToken.Data.UserId,
                    Application = objUserDebugToken.Data.Application,
                    AccessToken = objUserToken.AccessToken,
                    IsValid = objUserDebugToken.Data.IsValid,
                    ExpiresAt = objUserDebugToken.Data.ExpiresAt,
                    IssuedAt = objUserDebugToken.Data.IssuedAt,
                    Scopes = objUserDebugToken.Data.Scopes
                });

                // 2. PAGE Token
                PageTokenResponse objPageDebugToken = await _facebookApi.GetPageTokenAsync(objUserToken.AccessToken);
                foreach (var item in objPageDebugToken.Data)
                {
                    // 2.1. PAGE Token Debug
                    var pageDebugToken = await _facebookApi.DebugTokenAsync(item.AccessToken);
                    lstPageToken.Add(new PageTokenModel()
                    {
                        AppId = pageDebugToken.Data.AppId,
                        UserId = pageDebugToken.Data.UserId,
                        Application = pageDebugToken.Data.Application,
                        ProfileId = pageDebugToken.Data.ProfileId,
                        AccessToken = item.AccessToken,
                        IsValid = pageDebugToken.Data.IsValid,
                        ExpiresAt = pageDebugToken.Data.ExpiresAt,
                        IssuedAt = pageDebugToken.Data.IssuedAt,
                        Scopes = pageDebugToken.Data.Scopes
                    });

                    // 2.2. Subscribe live_videos webhook
                    await _facebookApi.SubscribeLiveVideoAsync(item.AccessToken);

                    // 2.3. PAGE Info
                    objPageInfo = await _facebookApi.GetPageInfoAsync(item.AccessToken);
                    if (objPageInfo != null)
                    {
                        var lstCategory = new List<CategoryModel>();
                        foreach (var catergory in objPageInfo.Categories)
                        {
                            lstCategory.Add(new CategoryModel()
                            {
                                CategoryId = catergory.CategoryId,
                                CategoryName = catergory.CategoryName
                            });
                        }
                        lstPageInfo.Add(new PageModel()
                        {
                            PageId = objPageInfo.PageId,
                            PageName = objPageInfo.PageName,
                            Categories = lstCategory,
                            About = objPageInfo.About,
                            Address = objPageInfo.Address,
                            Link = objPageInfo.Link,
                            Emails = objPageInfo.Emails,
                            Website = objPageInfo.Website,
                            FanCount = objPageInfo.FanCount,
                            CheckinCount = objPageInfo.CheckinCount,
                            CanCheckin = objPageInfo.CanCheckin,
                            NewLikeCount = objPageInfo.NewLikeCount,
                            CanPost = objPageInfo.CanPost,
                            //Videos = new List<VideoModel>()
                        });
                    }
                }

                // 3. Insert Or Update Data
                AccountModel accountModel = new AccountModel();
                accountModel.UserTokens = lstUserToken;
                accountModel.PageTokens = lstPageToken;

                await _accountRepo.UpdateOneAsync(() => Builders<AccountModel>.Filter.Eq("Username", username), () => Builders<AccountModel>.Update.Set("UserTokens", accountModel.UserTokens).Set("PageTokens", accountModel.PageTokens));

                foreach (var item in lstPageInfo)
                {
                    var pageModel = _pageRepository.FindAsync(() => Builders<PageModel>.Filter.Eq("PageId", item.PageId)).Result.FirstOrDefault();
                    if (pageModel == null)
                    {
                        await _pageRepository.InsertOneAsync(item);
                    }
                    else
                    {
                        var update = Builders<PageModel>.Update
                            .Set("PageName", item.PageName)
                            .Set("Categories", item.Categories)
                            .Set("About", item.About)
                            .Set("Address", item.Address)
                            .Set("Link", item.Link)
                            .Set("Emails", item.Emails)
                            .Set("FanCount", item.FanCount)
                            .Set("NewLikeCount", item.NewLikeCount)
                            .Set("CheckinCount", item.CheckinCount)
                            .Set("CanCheckin", item.CanCheckin)
                            .Set("CanPost", item.CanPost)
                            .Set("CreatedTime", item.CreatedTime);
                        await _pageRepository.UpdateOneAsync(() => Builders<PageModel>.Filter.Eq("PageId", item.PageId), () => update);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task StreamingPipelineAsync(IVideoDetailService videoDetailService, string Id, string Token)
        {
            try
            {
                await _facebookApi.VideoStreamingAsync(Id, Token, async (body) =>
                {
                    PageInfoResponse objPageInfo = await _facebookApi.GetPageInfoAsync(Token);
                    var lstCategory = new List<CategoryModel>();
                    foreach (var catergory in objPageInfo.Categories)
                    {
                        lstCategory.Add(new CategoryModel()
                        {
                            CategoryId = catergory.CategoryId,
                            CategoryName = catergory.CategoryName
                        });
                    }
                    PageModel model = new PageModel()
                    {
                        PageId = objPageInfo.PageId,
                        PageName = objPageInfo.PageName,
                        Categories = lstCategory,
                        About = objPageInfo.About,
                        Address = objPageInfo.Address,
                        Link = objPageInfo.Link,
                        Emails = objPageInfo.Emails,
                        Website = objPageInfo.Website,
                        FanCount = objPageInfo.FanCount,
                        CheckinCount = objPageInfo.CheckinCount,
                        CanCheckin = objPageInfo.CanCheckin,
                        NewLikeCount = objPageInfo.NewLikeCount,
                        CanPost = objPageInfo.CanPost,
                        Videos = new List<VideoModel>()
                    };

                    var pagesModel = _pageRepository.FindAsync(() => Builders<PageModel>.Filter.Eq("PageId", objPageInfo.PageId)).Result.FirstOrDefault();
                    if (pagesModel == null)
                    {
                        await _pageRepository.InsertOneAsync(model);
                    }

                    List<string> lstPhone = new List<string>();
                    ReplyTemplateModel replyTemplate = new ReplyTemplateModel();
                    bool IsSendWhenDetectedPhone = false;
                    bool IsSendWhenDetectedGoodWord = false;
                    bool IsDeleteWhenDetectedBadWord = false;
                    bool IsHidden = false;
                    bool IsHiddenWhenNegSentiment = false;
                    bool IsSendWhenDetectedPhoneResult = false;
                    bool IsSendWhenDetectedGoodWordResult = false;
                    bool IsDeleteWhenDetectedBadWordResult = false;
                    bool IsHiddenResult = false;
                    Match objBadWord = null;
                    Match objGoodWord = null;
                    VideoDetailModel videoModel = null;

                    List<string> lstBadWord = null;
                    List<string> lstGoodWord = null;

                    if (pagesModel.Videos != null && pagesModel.Videos.Count > 0)
                    {
                        foreach (var video in pagesModel.Videos)
                        {
                            if (video.VideoId.Equals(Id))
                            {
                                lstBadWord = video.BadWords;
                                lstGoodWord = video.GoodWords;
                                IsSendWhenDetectedPhone = video.IsSendWhenDetectedPhone;
                                IsSendWhenDetectedGoodWord = video.IsSendWhenDetectedGoodWord;
                                IsDeleteWhenDetectedBadWord = video.IsDeleteWhenDetectedBadWord;
                                IsHidden = video.IsHidden;
                                IsHiddenWhenNegSentiment = video.IsHiddenWhenNegSentiment;
                                replyTemplate = video.ReplyTemplate;
                                replyTemplate.Recipient.CommentId = body.Id;
                                break;
                            }
                        }
                    }

                    //1. Detect phone number.
                    foreach (Match phone in Regex.Matches(body.Message.Trim(), @"0\d{9}"))
                    {
                        lstPhone.Add(phone.Value);
                    }
                    if (lstPhone != null && lstPhone.Count > 0)
                    {
                        if (IsSendWhenDetectedPhone == true)
                        {
                            IsSendWhenDetectedPhoneResult = await _facebookApi.SendPrivateReplyAsync(replyTemplate.ToModelReq(), Token);
                        }
                        if (IsHidden == true && IsHiddenResult == false)
                        {
                            //IsHiddenResult = await _facebookApi.HiddenCommentAsync(body.Id, Token, true);
                            IsHiddenResult = await _facebookApi.DeleteCommentAsync(body.Id, Token);
                        }
                    }

                    //2. Sentiment NLP
                    if (IsHiddenWhenNegSentiment && IsHiddenResult == false)
                    {
                        if (_sentimentNLPApi.SentimentNLP(body.Message.Trim()).Result)
                        {
                            //IsHiddenResult = await _facebookApi.HiddenCommentAsync(body.Id, Token, true);
                            IsHiddenResult = await _facebookApi.DeleteCommentAsync(body.Id, Token);
                        }
                    }

                    //3. Detect bad word and delete comment.
                    if (lstBadWord != null)
                    {
                        if (IsDeleteWhenDetectedBadWord == true)
                        {
                            foreach (string badWord in lstBadWord)
                            {
                                objBadWord = Regex.Match(body.Message.Trim().ToLower(), badWord.Trim().ToLower());
                                if (objBadWord.Success)
                                {
                                    IsDeleteWhenDetectedBadWordResult = await _facebookApi.DeleteCommentAsync(body.Id, Token);
                                }
                            }
                        }
                    }

                    //4. Detect good word and send private reply
                    if (lstGoodWord != null)
                    {
                        if (IsSendWhenDetectedGoodWord == true)
                        {
                            foreach (string goodWord in lstGoodWord)
                            {
                                objGoodWord = Regex.Match(body.Message.Trim().ToLower(), goodWord.Trim().ToLower());
                                if (objGoodWord.Success)
                                {
                                    IsSendWhenDetectedGoodWordResult = await _facebookApi.SendPrivateReplyAsync(replyTemplate.ToModelReq(), Token);
                                }
                            }
                        }
                    }

                    //5. Insert or Update new document.
                    videoModel = videoDetailService.FindAsync(() => Builders<VideoDetailModel>.Filter.Eq("FbId", body.From.Id)).Result.FirstOrDefault();
                    var commentModel = new CommentModel()
                    {
                        CommentId = body.Id,
                        Message = body.Message,
                        IsSendWhenDetectedPhoneResult = IsSendWhenDetectedPhoneResult,
                        IsSendWhenDetectedGoodWordResult = IsSendWhenDetectedGoodWordResult,
                        IsDeleteWhenDetectedBadWordResult = IsDeleteWhenDetectedBadWordResult,
                        IsHiddenResult = IsHiddenResult,
                        CreatedTime = body.CreatedTime
                    };
                    if (videoModel == null)
                    {
                        videoModel = new VideoDetailModel();
                        videoModel.FbId = body.From.Id;
                        videoModel.FbName = body.From.Name;
                        videoModel.Phones = lstPhone;
                        videoModel.Comments.Add(commentModel);

                        await videoDetailService.InsertOneAsync(videoModel);
                    }
                    else
                    {
                        if (lstPhone != null && lstPhone.Count > 0)
                        {
                            foreach (string phone in lstPhone)
                            {
                                videoModel.Phones.Add(phone);
                            }
                        }
                        videoModel.Comments.Add(commentModel);

                        await videoDetailService.UpdateOneAsync(() => Builders<VideoDetailModel>.Filter.Eq("FbId", body.From.Id), () => Builders<VideoDetailModel>.Update.Set("Comments", videoModel.Comments).Set("Phones", videoModel.Phones));
                    }
                });
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
