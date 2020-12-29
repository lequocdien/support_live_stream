using Newtonsoft.Json;
using SupportLiveStream.Api.RequestModel;
using SupportLiveStream.Api.ResponseModel;
using SupportLiveStream.Common;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SupportLiveStream.Api
{
    public interface IFacebookApi
    {
        Task<bool> SubscribeLiveVideoAsync(string strToken);
        Task<OauthResponse> OauthAsync(string strCode);
        Task<DebugTokenResponse> DebugTokenAsync(string strToken);
        Task<PageTokenResponse> GetPageTokenAsync(string strUserToken);
        Task<PageInfoResponse> GetPageInfoAsync(string strPageToken);
        Task<LiveVideoResponse> GetLiveVideoOfPageAsync(string strPageToken);
        Task<LiveVideoDataResp> GetStatusLiveVideo(string strVideoId, string strPageToken);
        Task VideoStreamingAsync(string strId, string strToken, Action<StreamingResponse> action);
        Task<bool> DeleteCommentAsync(string commentId, string strToken);
        Task<bool> HiddenCommentAsync(string commentId, string strToken, bool isHidden = true);
        Task<bool> SendPrivateReplyAsync(PrivateReplyReq objBody, string strToken);
        Task<BlockResponse> GetListBlock(string strToken);
        Task<bool> BlockUser(string fbId, string strToken);
        Task<bool> UnblockUser(string fbId, string strToken);
    }

    public class FacebookApi : IFacebookApi
    {
        public async Task<bool> SubscribeLiveVideoAsync(string strToken)
        {
            try
            {
                string Url = GetUrlSubscribeLiveVideos(strToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //string body = await responseMessage.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<OauthResponse> OauthAsync(string strCode)
        {
            OauthResponse tokenResponse = null;
            try
            {
                string Url = GetUrlUserToken("840201613381836", "https://lequocdien.tk/api/oauth/fb-login", "c4c199b3b77b551bed31d26de9995efc", strCode);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    tokenResponse = JsonConvert.DeserializeObject<OauthResponse>(body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tokenResponse;
        }

        public async Task<DebugTokenResponse> DebugTokenAsync(string strToken)
        {
            DebugTokenResponse objDebugToken = null;
            try
            {
                string Url = GetUrlDebugToken(strToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    objDebugToken = JsonConvert.DeserializeObject<DebugTokenResponse>(body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objDebugToken;
        }

        public async Task<PageTokenResponse> GetPageTokenAsync(string strUserToken)
        {
            PageTokenResponse objPageToken = null;
            try
            {
                string Url = GetUrlPageToken(strUserToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    objPageToken = JsonConvert.DeserializeObject<PageTokenResponse>(body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPageToken;
        }

        public async Task<LiveVideoResponse> GetLiveVideoOfPageAsync(string strPageToken)
        {
            LiveVideoResponse objVideoInfo = null;
            try
            {
                string Url = GetUrlLiveVideoOfPage(strPageToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    objVideoInfo = JsonConvert.DeserializeObject<LiveVideoResponse>(body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objVideoInfo;
        }

        public async Task<LiveVideoDataResp> GetStatusLiveVideo(string strVideoId, string strPageToken)
        {
            LiveVideoDataResp objVideoInfo = null;
            try
            {
                string Url = GetUrlStatusLiveVideo(strVideoId, strPageToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    objVideoInfo = JsonConvert.DeserializeObject<LiveVideoDataResp>(body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objVideoInfo;
        }

        public async Task<PageInfoResponse> GetPageInfoAsync(string strPageToken)
        {
            PageInfoResponse objPageInfo = null;
            try
            {
                string Url = GetUrlPageInfo(strPageToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    objPageInfo = JsonConvert.DeserializeObject<PageInfoResponse>(body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objPageInfo;
        }

        public async Task VideoStreamingAsync(string strId, string strToken, Action<StreamingResponse> action)
        {
            try
            {
                string url = GetUrlStreaming(strId, strToken);
                string strResBody = String.Empty;
                StreamingResponse streamingResponse = null;

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                Stream stream = await client.GetStreamAsync(url);
                StreamReader reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    strResBody = Utilis.ConvertToUnicode(reader.ReadLine());
                    if (strResBody.Contains("no_data") || strResBody.Contains("has_emoji"))
                    {
                        LiveVideoDataResp objVideoInfo = await GetStatusLiveVideo(strId, strToken);
                        if (objVideoInfo == null || !objVideoInfo.Status.Equals("LIVE"))
                        {
                            break;
                        }
                        continue;
                    }
                    streamingResponse = JsonConvert.DeserializeObject<StreamingResponse>(strResBody);
                    action(streamingResponse);
                }
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

        public async Task<bool> DeleteCommentAsync(string commentId, string strToken)
        {
            try
            {
                string Url = GetUrlDeleteOrUpdateComment(commentId, strToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.DeleteAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<bool> HiddenCommentAsync(string commentId, string strToken, bool isHidden = true)
        {
            try
            {
                string Url = GetUrlHiddenComment(commentId, strToken, isHidden);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.PostAsync(Url, null);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    if (body.Contains("true"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<bool> SendPrivateReplyAsync(PrivateReplyReq objBody, string strToken)
        {
            try
            {
                string Url = GetUrlSendPriveReply(strToken);
                var jsonBody = new StringContent(JsonConvert.SerializeObject(objBody), Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.PostAsync(Url, jsonBody);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<BlockResponse> GetListBlock(string strToken)
        {
            BlockResponse respData = null;
            try
            {
                string Url = GetUrlListBlock(strToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.GetAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = await responseMessage.Content.ReadAsStringAsync();
                    respData = JsonConvert.DeserializeObject<BlockResponse>(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return respData;
        }

        public async Task<bool> BlockUser(string fbId, string strToken)
        {
            try
            {
                string Url = GetUrlBlock(fbId, strToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.PostAsync(Url, null);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Contains("true"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UnblockUser(string fbId, string strToken)
        {
            try
            {
                string Url = GetUrlBlock(fbId, strToken);
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = await client.DeleteAsync(Url);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = await responseMessage.Content.ReadAsStringAsync();
                    if (result.Contains("true"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Utilis
        private string GetUrlSubscribeLiveVideos(string strToken)
        {
            if (String.IsNullOrEmpty(strToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/me/subscribed_apps?subscribed_fields=live_videos&access_token={0}", strToken);
        }

        private string GetUrlDebugToken(string strToken)
        {
            if (String.IsNullOrEmpty(strToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/debug_token?fields=app_id,user_id,profile_id,type,application,is_valid,expires_at,issued_at,scopes&fields=app_id,type,application,expires_at,is_valid,issued_at,profile_id,scopes&input_token={0}&access_token={1}", strToken, strToken);
        }

        private string GetUrlUserToken(string clientId, string redirectUri, string clientSecret, string code)
        {
            if (String.IsNullOrEmpty(clientId) || String.IsNullOrEmpty(redirectUri) || String.IsNullOrEmpty(clientSecret) || String.IsNullOrEmpty(code))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/v8.0/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}", clientId.Trim(), redirectUri.Trim(), clientSecret.Trim(), code.Trim());
        }

        private string GetUrlPageToken(string strToken)
        {
            if (String.IsNullOrEmpty(strToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/me/accounts?fields=id,name,access_token&access_token={0}", strToken.Trim());
        }

        private string GetUrlPageInfo(string strToken)
        {
            if (String.IsNullOrEmpty(strToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/me?fields=id,name,category_list,about,link,phone,emails,website,fan_count,new_like_count,checkins,can_checkin,can_post,single_line_address&access_token={0}", strToken.Trim());
        }

        private string GetUrlLiveVideoOfPage(string strPageToken)
        {
            if (String.IsNullOrEmpty(strPageToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/me/live_videos?fields=id,status,title,description,permalink_url,live_views,creation_time,video&access_token={0}", strPageToken.Trim());
        }

        private string GetUrlStatusLiveVideo(string strVideoId, string strPageToken)
        {
            if (String.IsNullOrEmpty(strVideoId) || String.IsNullOrEmpty(strPageToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/{0}?fields=id,status,title,description,permalink_url,live_views,creation_time,video&access_token={1}", strVideoId.Trim(), strPageToken.Trim());
        }

        private string GetUrlStreaming(string liveId, string token)
        {
            if (String.IsNullOrEmpty(liveId) || String.IsNullOrEmpty(token))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://streaming-graph.facebook.com/{0}/live_comments?access_token={1}&comment_rate=one_per_two_seconds&fields=from{{name,id}},created_time,message", liveId.Trim(), token.Trim());
        }

        private string GetUrlDeleteOrUpdateComment(string commentId, string token)
        {
            if (String.IsNullOrEmpty(commentId) || String.IsNullOrEmpty(token))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/v8.0/{0}?access_token={1}", commentId.Trim(), token.Trim());
        }

        private string GetUrlHiddenComment(string commentId, string token, bool isHidden = true)
        {
            if (String.IsNullOrEmpty(commentId) || String.IsNullOrEmpty(token))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/v8.0/{0}?is_hidden={1}&access_token={2}", commentId.Trim(), isHidden, token.Trim());
        }

        private string GetUrlSendPriveReply(string strToken)
        {
            if (String.IsNullOrEmpty(strToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/v8.0/me/messages?access_token={0}", strToken.Trim());
        }

        private string GetUrlListBlock(string strToken)
        {
            if (String.IsNullOrEmpty(strToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/me/blocked?access_token={0}", strToken.Trim());
        }

        private string GetUrlBlock(string fbId, string strToken)
        {
            if (String.IsNullOrEmpty(strToken))
            {
                throw new Exception("Invalid input");
            }
            return String.Format("https://graph.facebook.com/me/blocked?psid={0}&access_token={1}", fbId, strToken.Trim());
        }
        #endregion
    }
}
