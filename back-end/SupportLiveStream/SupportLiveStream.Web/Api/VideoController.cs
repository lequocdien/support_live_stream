using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using SupportLiveStream.Model;
using SupportLiveStream.Service;
using SupportLiveStream.Web.Helpers;
using SupportLiveStream.Web.Models;

namespace SupportLiveStream.Web.Api
{
    [Route("api/video")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IPageService _pageService;
        private IVideoDetailService _videoDetailService;

        public VideoController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetVideo(string pageId)
        {
            try
            {
                if (String.IsNullOrEmpty(pageId))
                {
                    return BadRequest("The pageId field is required.");
                }
                var lst = _pageService.FindAsync(() => Builders<PageModel>.Filter.Eq("PageId", pageId)).Result.FirstOrDefault();
                if (lst != null)
                {
                    return Ok(lst.Videos.OrderByDescending(v => v.CreationTime));
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [Route("config")]
        [HttpPost]
        public async Task<IActionResult> ConfigVideo(ConfigVideoReq model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Invalid input."
                    });
                }

                var filter = Builders<PageModel>.Filter.Eq("PageId", model.PageId) & Builders<PageModel>.Filter.ElemMatch(e => e.Videos, Builders<VideoModel>.Filter.Eq("VideoId", model.VideoId));
                var update = Builders<PageModel>.Update
                    .Set(v => v.Videos[-1].IsSendWhenDetectedPhone, model.IsSendWhenDetectedPhone)
                    .Set(v => v.Videos[-1].IsSendWhenDetectedGoodWord, model.IsSendWhenDetectedGoodWord)
                    .Set(v => v.Videos[-1].IsDeleteWhenDetectedBadWord, model.IsDeleteWhenDetectedBadWord)
                    .Set(v => v.Videos[-1].IsHiddenWhenNegSentiment, model.IsHiddenWhenNegSentiment)
                    .Set(v => v.Videos[-1].IsHidden, model.IsHidden)
                    .Set(v => v.Videos[-1].GoodWords, model.GoodWords)
                    .Set(v => v.Videos[-1].BadWords, model.BadWords)
                    .Set(v => v.Videos[-1].ReplyTemplate.Message.Attachment.Payload.Text, model.ReplyMessage);

                await _pageService.UpdateOneAsync(() =>
                {
                    return filter;
                }, () =>
                {
                    return update;
                });

                return Ok(new
                {
                    message = "Success."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
