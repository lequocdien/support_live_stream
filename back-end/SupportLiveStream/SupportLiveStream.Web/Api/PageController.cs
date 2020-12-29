using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/page")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;
        private readonly IAccountService _accountService;
        private readonly IFacebookService _facebookService;

        public PageController(IPageService pageService, IAccountService accountService, IFacebookService facebookService)
        {
            _pageService = pageService;
            _accountService = accountService;
            _facebookService = facebookService;
        }

        [Authorize]
        [Route("")]
        public IActionResult GetPageInfo()
        {
            try
            {
                object username = String.Empty;
                if (!HttpContext.Items.TryGetValue("username", out username))
                {
                    return BadRequest();
                }
                else
                {
                    List<PageModel> lstPage = new List<PageModel>();
                    var account = _accountService.FindAsync(() => Builders<AccountModel>.Filter.Eq("Username", username)).Result.FirstOrDefault();
                    if (account != null)
                    {
                        foreach (var item in account.PageTokens)
                        {
                            var page = _pageService.FindAsync(() => Builders<PageModel>.Filter.Eq("PageId", item.ProfileId)).Result.FirstOrDefault();
                            if (page != null)
                            {
                                lstPage.Add(page);
                            }
                        }
                    }
                    return Ok(lstPage);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [Route("badword")]
        [HttpPost]
        public async Task<IActionResult> UpdateBadWord(UpdateBadWordReq model)
        {
            if (String.IsNullOrEmpty(model.PageId) || String.IsNullOrEmpty(model.VideoId) || model.BadWords == null)
            {
                return BadRequest("'pageId' && 'videoId' && 'BadWords' is not valid.");
            }
            try
            {
                await _pageService.UpdateOneAsync(() => Builders<PageModel>.Filter.Eq("PageId", model.PageId) & Builders<PageModel>.Filter.ElemMatch(e => e.Videos, Builders<VideoModel>.Filter.Eq("VideoId", model.VideoId)), () => Builders<PageModel>.Update.Set(v => v.Videos[-1].BadWords, model.BadWords).Set(v => v.Videos[-1].IsDeleteWhenDetectedBadWord, model.IsDelete));
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

        [Authorize]
        [Route("goodword")]
        [HttpPost]
        public async Task<IActionResult> UpdateGoodWord(UpdateGoodWordReq model)
        {
            if (String.IsNullOrEmpty(model.PageId) || String.IsNullOrEmpty(model.VideoId) || model.GoodWords == null)
            {
                return BadRequest("'pageId' && 'videoId' && 'BadWords' is not valid.");
            }
            try
            {
                await _pageService.UpdateOneAsync(() => Builders<PageModel>.Filter.Eq("PageId", model.PageId) & Builders<PageModel>.Filter.ElemMatch(e => e.Videos, Builders<VideoModel>.Filter.Eq("VideoId", model.VideoId)), () => Builders<PageModel>.Update.Set(v => v.Videos[-1].GoodWords, model.GoodWords));
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

        [Authorize]
        [Route("block")]
        [HttpGet]
        public async Task<IActionResult> GetListBlock(string pageId)
        {
            try
            {
                object username = String.Empty;
                if (!HttpContext.Items.TryGetValue("username", out username))
                {
                    return BadRequest();
                }

                var accountFilter = Builders<AccountModel>.Filter.Eq("Username", username) & Builders<AccountModel>.Filter.ElemMatch(e => e.PageTokens, Builders<PageTokenModel>.Filter.Eq("ProfileId", pageId));
                var account = _accountService.FindAsync(() => accountFilter).Result.FirstOrDefault();
                var token = account.PageTokens[0].AccessToken;

                var data = await _facebookService.GetListBlock(token);
                var resData = JsonConvert.SerializeObject(data);
                return Ok(resData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [Route("block")]
        [HttpPut]
        public async Task<IActionResult> BlockUser(string pageId, string blockId)
        {
            if(String.IsNullOrEmpty(pageId) || String.IsNullOrEmpty(blockId))
            {
                return BadRequest();
            }

            try
            {
                object username = String.Empty;
                if (!HttpContext.Items.TryGetValue("username", out username))
                {
                    return BadRequest();
                }

                var accountFilter = Builders<AccountModel>.Filter.Eq("Username", username) & Builders<AccountModel>.Filter.ElemMatch(e => e.PageTokens, Builders<PageTokenModel>.Filter.Eq("ProfileId", pageId));
                var account = _accountService.FindAsync(() => accountFilter).Result.FirstOrDefault();
                var token = account.PageTokens[0].AccessToken;

                bool isSuccess = await _facebookService.BlockUser(blockId, token);
                return Ok(new
                {
                    isSuccess
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [Route("block")]
        [HttpDelete]
        public async Task<IActionResult> UnblockUser(string pageId, string blockId)
        {
            if (String.IsNullOrEmpty(pageId) || String.IsNullOrEmpty(blockId))
            {
                return BadRequest();
            }

            try
            {
                object username = String.Empty;
                if (!HttpContext.Items.TryGetValue("username", out username))
                {
                    return BadRequest();
                }

                var accountFilter = Builders<AccountModel>.Filter.Eq("Username", username) & Builders<AccountModel>.Filter.ElemMatch(e => e.PageTokens, Builders<PageTokenModel>.Filter.Eq("ProfileId", pageId));
                var account = _accountService.FindAsync(() => accountFilter).Result.FirstOrDefault();
                var token = account.PageTokens[0].AccessToken;

                bool isSuccess = await _facebookService.UnblockUser(blockId, token);
                return Ok(new
                {
                    isSuccess
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
