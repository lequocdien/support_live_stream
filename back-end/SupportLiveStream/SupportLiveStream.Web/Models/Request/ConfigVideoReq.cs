using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SupportLiveStream.Web.Models
{
    public class ConfigVideoReq
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The pageId field is required.")]
        public string PageId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The videoId field is required.")]
        public string VideoId { get; set; }

        [Required]
        public bool IsSendWhenDetectedPhone { get; set; }

        [Required]
        public bool IsSendWhenDetectedGoodWord { get; set; } 
        
        [Required]
        public bool IsHiddenWhenNegSentiment { get; set; }

        [Required]
        public bool IsHidden { get; set; }

        [Required]
        public bool IsDeleteWhenDetectedBadWord { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The badWords field is required.")]
        public List<string> BadWords { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The goodWords field is required.")]
        public List<string> GoodWords { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The replyMessage field is required.")]
        public string ReplyMessage { get; set; }
    }
}
