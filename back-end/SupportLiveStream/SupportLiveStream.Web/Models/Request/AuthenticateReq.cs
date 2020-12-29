using System.ComponentModel.DataAnnotations;

namespace SupportLiveStream.Web.Models
{
    public class AuthenticateReq
    {
        [Required]
        public string Username
        {
            get; set;
        }

        [Required]
        public string Password
        {
            get; set;
        }
    }
}
