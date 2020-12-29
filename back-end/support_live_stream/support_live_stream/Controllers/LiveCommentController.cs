using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using support_live_stream.Models;
using System.Collections.Generic;
using System.Linq;

namespace support_live_stream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveCommentController : ControllerBase
    {
        private static string TOKEN = "EAAAAZAw4FxQIBAEZAyRb377ZCJeJSsXZC0e9ShZAgeBCRuqa1l2nlEq0EQDja9UNVx6nB0TG5Eie4Ma6arGvN1u74sjGIkISZBohmCuwIqJMo14BIkxc9ZBgkqGPIhadwQ5JJ7HSwJ9IrOD0mGS7AEtbKbWPUTZAs8pZBEb5Tu5qE1gZDZD";
        private static string LIVE_ID = "2467128216928369";
        private static string URL = String.Format("https://streaming-graph.facebook.com/{0}/live_comments?access_token={1}&comment_rate=one_per_two_seconds&fields=from{{name,id}},message", LIVE_ID, TOKEN);
        private static StringBuilder m_objDataLS = new StringBuilder();

        public LiveCommentController()
        {

        }

        public async Task Get(string url_live = "1019327905211065", string token = "EAAAAZAw4FxQIBAEZAyRb377ZCJeJSsXZC0e9ShZAgeBCRuqa1l2nlEq0EQDja9UNVx6nB0TG5Eie4Ma6arGvN1u74sjGIkISZBohmCuwIqJMo14BIkxc9ZBgkqGPIhadwQ5JJ7HSwJ9IrOD0mGS7AEtbKbWPUTZAs8pZBEb5Tu5qE1gZDZD")
        {
            var response = Response;
            //response.Headers.Add("Content-Type", "text/event-stream");
            response.ContentType = "text/event-stream";

            //for (var i = 0; true; ++i)
            //{
            //    //await response.WriteAsync($"data: Controller {i} at {DateTime.Now}\r\r");
            //    await response.WriteAsync("data: Le Dien\r\r");

            //    response.Body.Flush();
            //    await Task.Delay(1000);
            //}

            //for (int i = 0; true; i++)
            //{
            //    await response.WriteAsync("data: Lê Diện\r\r");
            //    response.Body.Flush();
            //    await Task.Delay(2000);
            //}

            string strTemp = String.Empty;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                System.IO.Stream stream = await client.GetStreamAsync(GetUrlAPIGraph(url_live, token));
                //stream.ReadTimeout = Timeout.Infinite;
                StreamReader reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    strTemp = Utilis.Utilis.ConvertToUnicode(reader.ReadLine());
                    if (strTemp.Contains("no_data") || strTemp.Contains("has_emoji"))
                    {
                        continue;
                    }
                    m_objDataLS.Append(strTemp + ",");
                    await response.WriteAsync("data:[" + m_objDataLS.ToString() + "]\r\r", Encoding.UTF8);

                    response.Body.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(strTemp);
                //return NotFound();
            }
        }

        #region Ultils
        private string GetUrlAPIGraph(string x_strUrlLS, string x_strToken)
        {
            return String.Format("https://streaming-graph.facebook.com/{0}/live_comments?access_token={1}&comment_rate=one_per_two_seconds&fields=from{{name,id}},message", x_strUrlLS, x_strToken);
        }
        #endregion
    }
}
