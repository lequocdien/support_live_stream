using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupportLiveStream.Web.Models
{
    public class UpdateGoodWordReq
    {
        [Required]
        public string PageId { get; set; }

        [Required]
        public string VideoId { get; set; }

        [Required]
        public bool IsSend { get; set; }

        [Required]
        public List<string> GoodWords { get; set; }
    }
}
