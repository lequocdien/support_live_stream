using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SupportLiveStream.Web.Helpers
{
    public static class HttpResponseExtension
    {
        public async static void Ok(this HttpResponse objResp, object data)
        {
            objResp.StatusCode = StatusCodes.Status200OK;
            await objResp.WriteAsync(JsonConvert.SerializeObject(data));
        }

        public async static void BadRequest(this HttpResponse objResp, object err)
        {
            objResp.StatusCode = StatusCodes.Status400BadRequest;
            await objResp.WriteAsync(JsonConvert.SerializeObject(err));
        }

        public async static void InternalServer(this HttpResponse objResp, object err)
        {
            objResp.StatusCode = StatusCodes.Status500InternalServerError;
            await objResp.WriteAsync(JsonConvert.SerializeObject(err));
        }
    }
}
