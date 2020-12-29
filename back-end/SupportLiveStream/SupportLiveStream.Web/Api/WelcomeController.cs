using Microsoft.AspNetCore.Mvc;

namespace SupportLiveStream.Web.Api
{
    [Route("api")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        [HttpGet]
        [Route("policy")]
        public string Welcome()
        {
            return "This is page policy.";
        }
    }
}
