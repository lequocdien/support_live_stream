using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using SupportLiveStream.Model;
using SupportLiveStream.Service;
using SupportLiveStream.Web.AppSettings;
using SupportLiveStream.Web.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SupportLiveStream.Web.Api
{
    [Route("api/oauth")]
    [ApiController]
    public class OauthController : ControllerBase
    {
        private readonly IPipelineService _pipelineService;
        private readonly IAccountService _accountService;
        private readonly JwtConfig _jwtConfig;

        public OauthController(IPipelineService pipelineService, IAccountService accountService, IOptions<JwtConfig> options)
        {
            _pipelineService = pipelineService;
            _accountService = accountService;
            _jwtConfig = options.Value;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AuthenticateReq model)
        {
            try
            {
                if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password))
                {
                    return BadRequest(new
                    {
                        message = "Tên đăng nhập, mật khẩu không hợp lệ."
                    });
                }
                else
                {
                    var lst = await _accountService.FindAsync(() => Builders<AccountModel>.Filter.Eq("Username", model.Username) & Builders<AccountModel>.Filter.Eq("Password", model.Password));
                    if (lst == null || lst.Count() == 0)
                    {
                        return BadRequest(new
                        {
                            message = "Tài khoản hoặc mật khẩu không đúng."
                        });
                    }

                    AccountModel account = lst.Where(i => i.IsActive == true).FirstOrDefault();
                    string token = GenerateJwtToken(account.Username, account.Role);

                    PageTokenModel pageToken = null;
                    if (account.PageTokens == null || account.PageTokens.Count == 0)
                    {
                        return Ok(new
                        {
                            UserName = account.Username,
                            Role = account.Role,
                            IsReady = false,
                            AccessToken = token
                        });
                    }

                    pageToken = account.PageTokens.FirstOrDefault();
                    return Ok(new
                    {
                        UserName = account.Username,
                        Role = account.Role,
                        IsReady = pageToken == null ? false : pageToken.IsValid,
                        AccessToken = token
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("fb-login")]
        public async Task<IActionResult> LoginFacebook()
        {
            try
            {
                string state = Request.Query["state"];
                string code = Request.Query["code"];
                if (!String.IsNullOrEmpty(code))
                {
                    StateResp objState = JsonConvert.DeserializeObject<StateResp>(state);
                    if (!String.IsNullOrEmpty(objState.Username))
                    {
                        var isSuccess = await _pipelineService.OauthPiplineAsync(code, objState.Username);
                        if (isSuccess)
                        {
                            return Redirect("http://localhost:3000/loginfb/success?isSuccess=true");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Redirect("http://localhost:3000/loginfb/success?isSuccess=false");
        }

        #region Utils
        private string GenerateJwtToken(string username, string role)
        {
            try
            {
                if (String.IsNullOrEmpty(username))
                {
                    return null;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                        new[] {
                            new Claim("usr", username),
                            new Claim("role", role)
                        }
                    ),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class StateResp
    {
        public string Username { get; set; }
    }
}
