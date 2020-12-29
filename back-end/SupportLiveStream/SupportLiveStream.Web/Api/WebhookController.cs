using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SupportLiveStream.Api.ResponseModel;
using SupportLiveStream.Model;
using SupportLiveStream.Service;
using SupportLiveStream.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SupportLiveStream.Web.Api
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IPipelineService _pipelineService;
        private readonly IFacebookService _facebookService;
        private readonly IAccountService _accountService;
        private readonly IPageService _pageService;

        public WebhookController(IPipelineService pipelineService, IFacebookService facebookService, IAccountService accountService, IPageService pageService)
        {
            _pipelineService = pipelineService;
            _facebookService = facebookService;
            _accountService = accountService;
            _pageService = pageService;
        }

        [HttpGet]
        public string Get()
        {
            var challenge = Request.Query["hub.challenge"];
            return challenge;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromServices] Func<string, IVideoDetailService> videoService, LiveVideoReq model)
        {
            try
            {
                if (model.Entry[0].Changes[0].Value.Status.Equals("live"))
                {
                    var pageId = model.Entry[0].Id;
                    var objAccount = _accountService.FindAsync(() => Builders<AccountModel>.Filter.Eq("PageTokens.ProfileId", pageId)).Result.FirstOrDefault();
                    if (objAccount != null)
                    {
                        var strPageToken = String.Empty;
                        foreach (var pageToken in objAccount.PageTokens)
                        {
                            if (pageToken.ProfileId.Equals(pageId))
                            {
                                strPageToken = pageToken.AccessToken;
                                break;
                            }
                        }

                        if (!String.IsNullOrEmpty(strPageToken))
                        {
                            var objVideos = await _facebookService.GetLiveVideoOfPageAsync(strPageToken);

                            if (objVideos != null)
                            {
                                LiveVideoDataResp objLiveVideo = null;
                                foreach (var video in objVideos.Data)
                                {
                                    if (video.Status.Equals("LIVE"))
                                    {
                                        objLiveVideo = video;
                                        break;
                                    }
                                }

                                if (objLiveVideo != null)
                                {
                                    var objPageModel = _pageService.FindAsync(() => Builders<PageModel>.Filter.Eq("PageId", pageId)).Result.FirstOrDefault();

                                    if (objPageModel != null && objPageModel.Videos != null)
                                    {
                                        bool isExist = false;
                                        foreach (var video in objPageModel.Videos)
                                        {
                                            if (objLiveVideo.VideoId.Equals(video.VideoId))
                                            {
                                                isExist = true;
                                                break;
                                            }
                                        }

                                        if (isExist == false)
                                        {
                                            var lst = objPageModel.Videos;
                                            lst.Add(new VideoModel()
                                            {
                                                VideoId = objLiveVideo.VideoId,
                                                Video = new VideoInsideModel()
                                                {
                                                    DisplayVideoId = objLiveVideo.Video.DisplayVideoId
                                                },
                                                Status = objLiveVideo.Status,
                                                Title = objLiveVideo.Title,
                                                Description = objLiveVideo.Description,
                                                PermalinkUrl = objLiveVideo.PermalinkUrl,
                                                LiveViews = objLiveVideo.LiveViews,
                                                IsSendWhenDetectedPhone = false,
                                                IsDeleteWhenDetectedBadWord = false,
                                                IsSendWhenDetectedGoodWord = false,
                                                IsHidden = false,
                                                GoodWords = new System.Collections.Generic.List<string>(),
                                                BadWords = new System.Collections.Generic.List<string>(),
                                                CreationTime = objLiveVideo.CreationTime,
                                            });
                                            await _pageService.UpdateOneAsync(() => Builders<PageModel>.Filter.Eq("PageId", pageId), () => Builders<PageModel>.Update.Set("Videos", lst));
                                            await _pipelineService.StreamingPipelineAsync(videoService(objLiveVideo.VideoId), objLiveVideo.VideoId, strPageToken);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (model.Entry[0].Changes[0].Value.Status.Equals("live_stopped"))
                {
                    var pageId = model.Entry[0].Id;
                    var objPage = _pageService.FindAsync(() => Builders<PageModel>.Filter.Eq("PageId", pageId)).Result.FirstOrDefault();
                    if (objPage != null && objPage.Videos != null && objPage.Videos[objPage.Videos.Count - 1].Status.Equals("LIVE"))
                    {
                        var videoId = objPage.Videos[objPage.Videos.Count - 1].VideoId;
                        await _pageService.UpdateOneAsync(() => Builders<PageModel>.Filter.Eq("PageId", pageId) & Builders<PageModel>.Filter.ElemMatch(e => e.Videos, Builders<VideoModel>.Filter.Eq("VideoId", videoId)), () => Builders<PageModel>.Update.Set(e => e.Videos[-1].Status, "VOD"));
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }
    }
}
