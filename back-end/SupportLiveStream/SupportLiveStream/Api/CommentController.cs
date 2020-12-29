using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupportLiveStream.DAL;

namespace SupportLiveStream.Api
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        [HttpGet]
        [Route("live-comment")]
        public string GetLiveComment()
        {
            TestConnect testConnect = new TestConnect();
            testConnect.AddDocument();
            return "Ok";
        }
    }
}
