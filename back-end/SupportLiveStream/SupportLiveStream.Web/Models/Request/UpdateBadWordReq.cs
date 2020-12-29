using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupportLiveStream.Web.Models
{
    public class UpdateBadWordReq
    {
        [Required]
        public string PageId { get; set; }

        [Required]
        public string VideoId { get; set; }

        [Required]
        public bool IsDelete { get; set; }

        [Required]
        public List<string> BadWords { get; set; }
    }
}
