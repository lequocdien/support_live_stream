using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SupportLiveStream.Model;
using SupportLiveStream.Service;
using SupportLiveStream.Web.Helpers;

namespace SupportLiveStream.Web.Api
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Route("register")]
        [HttpPut]
        public async Task<IActionResult> Register(AccountModel model)
        {
            if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.FirstName) || String.IsNullOrEmpty(model.LastName) || String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Address))
            {
                return BadRequest();
            }

            try
            {
                var data = new AccountModel()
                {
                    Role = "USER",
                    IsActive = true,
                    Username = model.Username,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Address = model.Address
                };
                await _accountService.InsertOneAsync(data);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("get")]
        [HttpGet]
        [AdminAuthorize]
        public async Task<IActionResult> Get()
        {
            string username = HttpContext.Items["username"].ToString();
            try
            {
                var lst = await _accountService.FindAsync();
                var res = lst.Where(i => !i.Username.Equals(username)).OrderBy(i => i.Username);
                return Ok(res);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("update")]
        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> UpdateByAdmin(string userName, string role, bool isActive = false)
        {
            if(String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(role))
            {
                return BadRequest();
            }

            try
            {
                userName = userName.Trim().ToLower();
                role = role.Trim().ToUpper();
                var filter = Builders<AccountModel>.Filter.Eq("Username", userName);
                var update = Builders<AccountModel>.Update.Set("Role", role).Set("IsActive", isActive);
                await _accountService.UpdateOneAsync(() => filter, () => update);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("delete")]
        [HttpDelete]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteAccount(string username, bool isActive)
        {
            if (String.IsNullOrEmpty(username))
            {
                return BadRequest();
            }

            try
            {
                var filter = Builders<AccountModel>.Filter.Eq("Username", username);
                var update = Builders<AccountModel>.Update.Set("Password", isActive);
                await _accountService.UpdateOneAsync(() => filter, () =>  update);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
