using System;
using System.Collections.Generic;

namespace SupportLiveStream.Model
{
    public class AccountModel : MongoModelBase<AccountModel>, IMongoModelBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public List<UserTokenModel> UserTokens { get; set; }
        public List<PageTokenModel> PageTokens { get; set; }
        public DateTime CreatedTime { get; set; }
        public AccountModel()
        {
            IsActive = true;
            UserTokens = new List<UserTokenModel>();
            PageTokens = new List<PageTokenModel>();
            CreatedTime = DateTime.Now;
        }
    }

    public class UserTokenModel
    {
        public string AppId { get; set; }
        public string UserId { get; set; }
        public string Application { get; set; }
        public string AccessToken { get; set; }
        public bool IsValid { get; set; }
        public int ExpiresAt { get; set; }
        public int IssuedAt { get; set; }
        public List<string> Scopes { get; set; }
        public DateTime CreatedTime { get; set; }
        public UserTokenModel()
        {
            Scopes = new List<string>();
            CreatedTime = DateTime.Now;
        }
    }

    public class PageTokenModel
    {
        public string AppId { get; set; }
        public string UserId { get; set; }
        public string Application { get; set; }
        public string ProfileId { get; set; }
        public string AccessToken { get; set; }
        public bool IsValid { get; set; }
        public int ExpiresAt { get; set; }
        public int IssuedAt { get; set; }
        public List<string> Scopes { get; set; }
        public DateTime CreatedTime { get; set; }
        public PageTokenModel()
        {
            Scopes = new List<string>();
            CreatedTime = DateTime.Now;
        }
    }
}
