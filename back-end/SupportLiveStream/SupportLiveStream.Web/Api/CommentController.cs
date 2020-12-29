using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using SupportLiveStream.Api.RequestModel;
using SupportLiveStream.Model;
using SupportLiveStream.Service;
using SupportLiveStream.Service.Commons;
using SupportLiveStream.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportLiveStream.Web.Api
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ICommentService _commentService;
        private IAccountService _accountService;
        private IPageService _pageService;
        private IVideoDetailService _videoDetailService;
        private bool _isLiving = true;

        public CommentController(ICommentService commentService, IAccountService accountService, IPageService pageService)
        {
            _commentService = commentService;
            _accountService = accountService;
            _pageService = pageService;
        }

        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task GetComment([FromServices] Func<string, IVideoDetailService> func, string Id)
        {
            HttpResponse response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            response.StatusCode = 200;
            IEnumerable<VideoDetailModel> lstVideoDetail = null;
            string body = String.Empty;

            if (String.IsNullOrEmpty(Id))
            {
                response.BadRequest("'id' is not valid.");
            }
            else
            {
                _videoDetailService = func(Id);
                try
                {
                    while (_isLiving && Response.HttpContext.RequestAborted.IsCancellationRequested == false)
                    {
                        lstVideoDetail = await _videoDetailService.FindAsync();
                        if(lstVideoDetail != null && lstVideoDetail.Count() > 0)
                        {
                            lstVideoDetail = lstVideoDetail.OrderBy(i => i.FbName);
                        }
                        body = JsonConvert.SerializeObject(lstVideoDetail);
                        await response.WriteAsync("data:" + JsonConvert.SerializeObject(new ResponseModel(200, body)) + "\r\r");
                        await response.Body.FlushAsync();
                        Thread.Sleep(1000);
                        var objPage = _pageService.FindAsync(() => Builders<PageModel>.Filter.Eq("Videos.VideoId", Id.Trim())).Result.FirstOrDefault();
                        if (objPage == null)
                        {
                            _isLiving = false;
                        }
                        else
                        {
                            foreach (var video in objPage.Videos)
                            {
                                if (video.VideoId.Equals(Id) && video.Status.Equals("LIVE") == false)
                                {
                                    _isLiving = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.InternalServer(ex.Message);
                    response.Body.Close();
                }

                response.Body.Close();
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteComment([FromServices] Func<string, IVideoDetailService> videoService, string videoId, string fbId, string commentId)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(videoId) || String.IsNullOrEmpty(fbId) || String.IsNullOrEmpty(commentId))
            {
                return BadRequest();
            }

            try
            {
                _videoDetailService = videoService(videoId);
                bool result = false;
                object username = string.Empty;
                if (HttpContext.Items.TryGetValue("username", out username))
                {
                    var accountFilter = Builders<AccountModel>.Filter.Eq("Username", username);
                    var account = _accountService.FindAsync(() => accountFilter).Result.FirstOrDefault();

                    if (account != null && account.PageTokens != null)
                    {
                        var pageToken = account.PageTokens.FirstOrDefault();
                        if (pageToken != null && pageToken.IsValid == true && !String.IsNullOrEmpty(pageToken.AccessToken))
                        {
                            result = await _commentService.DeleteCommentAsync(commentId, pageToken.AccessToken);

                            if (result)
                            {
                                var videoDetailFilter = Builders<VideoDetailModel>.Filter.Eq("FbId", fbId) & Builders<VideoDetailModel>.Filter.ElemMatch(e => e.Comments, Builders<CommentModel>.Filter.Eq("CommentId", commentId));
                                var videoDetailUpdate = Builders<VideoDetailModel>.Update.Set(c => c.Comments[-1].IsDeleteWhenDetectedBadWordResult, true);
                                await _videoDetailService.UpdateOneAsync(() => videoDetailFilter, () => videoDetailUpdate);
                            }
                        }
                    }
                }

                return Ok(new
                {
                    status = result ? 1 : 0,
                    message = result ? "Success." : "Failed."
                });
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("hidden")]
        public async Task<IActionResult> HiddenComment([FromServices] Func<string, IVideoDetailService> videoService, string videoId, string fbId, string commentId, bool isHidden = true)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(videoId) || String.IsNullOrEmpty(fbId) || String.IsNullOrEmpty(commentId))
            {
                return BadRequest();
            }

            try
            {
                _videoDetailService = videoService(videoId);
                bool result = false;
                object username = string.Empty;
                if (HttpContext.Items.TryGetValue("username", out username))
                {
                    var accountFilter = Builders<AccountModel>.Filter.Eq("Username", username);
                    var account = _accountService.FindAsync(() => accountFilter).Result.FirstOrDefault();

                    if (account != null && account.PageTokens != null)
                    {
                        var pageToken = account.PageTokens.FirstOrDefault();
                        if (pageToken != null && pageToken.IsValid == true && !String.IsNullOrEmpty(pageToken.AccessToken))
                        {
                            result = await _commentService.HiddenCommentAsync(commentId, pageToken.AccessToken, isHidden);

                            if (result)
                            {
                                var videoDetailFilter = Builders<VideoDetailModel>.Filter.Eq("FbId", fbId) & Builders<VideoDetailModel>.Filter.ElemMatch(e => e.Comments, Builders<CommentModel>.Filter.Eq("CommentId", commentId));
                                var videoDetailUpdate = Builders<VideoDetailModel>.Update.Set(c => c.Comments[-1].IsHiddenResult, isHidden);
                                await _videoDetailService.UpdateOneAsync(() => videoDetailFilter, () => videoDetailUpdate);
                            }
                        }
                    }
                }

                return Ok(new
                {
                    status = result ? 1 : 0,
                    message = result ? "Success." : "Failed."
                });
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendPrivateReplyAsync([FromServices] Func<string, IVideoDetailService> videoService, string pageId, string videoId, string fbId, string commentId)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(pageId) || String.IsNullOrEmpty(videoId) || String.IsNullOrEmpty(fbId) || String.IsNullOrEmpty(commentId))
            {
                return BadRequest();
            }

            try
            {
                _videoDetailService = videoService(videoId);
                bool result = false;
                object username = string.Empty;
                if (HttpContext.Items.TryGetValue("username", out username))
                {
                    var filter = Builders<AccountModel>.Filter.Eq("Username", username);
                    var account = _accountService.FindAsync(() => filter).Result.FirstOrDefault();

                    if (account != null && account.PageTokens != null)
                    {
                        var pageToken = account.PageTokens.FirstOrDefault();

                        var videoFilter = Builders<PageModel>.Filter.Eq("PageId", pageId) & Builders<PageModel>.Filter.ElemMatch(e => e.Videos, Builders<VideoModel>.Filter.Eq("VideoId", videoId));
                        var page = _pageService.FindAsync(() => videoFilter).Result.FirstOrDefault();
                        if (page == null || page.Videos == null || page.Videos.Count < 0)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }

                        var video = page.Videos.Where(i => i.VideoId.Equals(videoId.Trim())).FirstOrDefault();
                        var body = video.ReplyTemplate.ToModelReq();
                        body.Recipient.CommentId = commentId;

                        result = await _commentService.SendPrivateReplyAsync(body, pageToken.AccessToken);

                        if (result)
                        {
                            var videoDetailFilter = Builders<VideoDetailModel>.Filter.Eq("FbId", fbId) & Builders<VideoDetailModel>.Filter.ElemMatch(e => e.Comments, Builders<CommentModel>.Filter.Eq("CommentId", commentId));
                            var videoDetailUpdate = Builders<VideoDetailModel>.Update.Set(c => c.Comments[-1].IsSendWhenDetectedGoodWordResult, true);
                            await _videoDetailService.UpdateOneAsync(() => videoDetailFilter, () => videoDetailUpdate);
                        }
                    }
                }

                return Ok(new
                {
                    message = result ? "Success." : "Failed."
                });
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("export")]
        public async Task<IActionResult> ExportExcelData([FromServices] Func<string, IVideoDetailService> func, string Id)
        {
            if (String.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }

            try
            {
                _videoDetailService = func(Id);

                var lstComment = await _videoDetailService.FindAsync();
                if(lstComment != null && lstComment.Count() > 0)
                {
                    lstComment = lstComment.OrderBy(i => i.FbName);
                }
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Bình luận");
                    worksheet.Columns().AdjustToContents();
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Facebook ID";
                    worksheet.Cell(currentRow, 2).Value = "Tên Facebook";
                    worksheet.Cell(currentRow, 3).Value = "Số điện thoại";
                    worksheet.Cell(currentRow, 4).Value = "Bình luận";
                    worksheet.Cell(currentRow, 5).Value = "Trạng thái";
                    worksheet.Cell(currentRow, 6).Value = "Ngày tạo";
                    worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 6)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 6)).Style.Font.SetBold(true);
                    worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 6)).Style.Fill.SetBackgroundColor(XLColor.AshGrey);
                    worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 6)).SetAutoFilter(true);
                    worksheet.Columns("A", "F").AdjustToContents();

                    for (int i = 0; i < lstComment.Count(); i++)
                    {
                        var comment = lstComment.ElementAt(i);
                        currentRow = currentRow + 1;

                        //Facebook Id
                        worksheet.Cell(currentRow, 1).SetValue<string>(comment.FbId.ToString());
                        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow + comment.Comments.Count - 1, 1)).Merge();
                        worksheet.Cell(currentRow, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        //Tên Facebook
                        worksheet.Cell(currentRow, 2).Value = comment.FbName;
                        worksheet.Range(worksheet.Cell(currentRow, 2), worksheet.Cell(currentRow + comment.Comments.Count - 1, 2)).Merge();
                        worksheet.Cell(currentRow, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        if (i % 2 == 0)
                        {
                            worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow + comment.Comments.Count - 1, 6)).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        }
                        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow + comment.Comments.Count - 1, 6)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow + comment.Comments.Count - 1, 6)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow + comment.Comments.Count - 1, 6)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow + comment.Comments.Count - 1, 6)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow + comment.Comments.Count - 1, 6)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        //Số điện thoại
                        for (int j = 0; j < comment.Phones.Count; j++)
                        {
                            worksheet.Cell(currentRow + j, 3).SetValue<string>(comment.Phones[j]);
                        };

                        for (int j = 0; j < comment.Comments.Count; j++)
                        {
                            //Bình luận
                            worksheet.Cell(currentRow, 4).SetValue<string>(comment.Comments[j].Message);
                            if (comment.Comments[j].IsDeleteWhenDetectedBadWordResult)
                            {
                                //Trạng thái
                                worksheet.Cell(currentRow, 5).Value = "Đã xóa";
                            }
                            else if (comment.Comments[j].IsHiddenResult)
                            {
                                //Trạng thái
                                worksheet.Cell(currentRow, 5).Value = "Đã ẩn";
                            }
                            else if (comment.Comments[j].IsSendWhenDetectedPhoneResult || comment.Comments[j].IsSendWhenDetectedGoodWordResult)
                            {
                                //Trạng thái
                                worksheet.Cell(currentRow, 5).Value = "Đã gửi";
                            }

                            worksheet.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            //Ngày tạo
                            worksheet.Cell(currentRow, 6).DataType = XLDataType.DateTime;
                            worksheet.Cell(currentRow, 6).SetValue<DateTime>(comment.Comments[j].CreatedTime.ToLocalTime());
                            currentRow = currentRow + 1;
                        };
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            String.Format("binh_luan_{0}.xlsx", Id));
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
