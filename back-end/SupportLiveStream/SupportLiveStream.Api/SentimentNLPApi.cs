using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupportLiveStream.Api
{
    public interface ISentimentNLPApi
    {
        Task<bool> SentimentNLP(string message);
    }

    public class SentimentNLPApi : ISentimentNLPApi
    {
        public async Task<bool> SentimentNLP(string message)
        {
            bool isNeg = false;
            try
            {
                string Url = String.Format("http://103.116.104.156:8081/api/predict?msg={0}", message);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    if (body.Equals("1"))
                    {
                        isNeg = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isNeg;
        }
    }
}
