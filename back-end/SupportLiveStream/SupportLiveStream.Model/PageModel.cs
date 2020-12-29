using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SupportLiveStream.Model
{
    public class PageModel : IMongoModelBase
    {
        public ObjectId Id { get; set; }
        public string PageId { get; set; }
        public string PageName { get; set; }
        public List<CategoryModel> Categories { get; set; }
        public string About { get; set; }
        public string Address { get; set; }
        public string Link { get; set; }
        public List<string> Emails { get; set; }
        public string Website { get; set; }
        public int FanCount { get; set; }
        public int NewLikeCount { get; set; }
        public int CheckinCount { get; set; }
        public bool CanCheckin { get; set; }
        public bool CanPost { get; set; }
        public List<VideoModel> Videos { get; set; }
        public DateTime CreatedTime { get; set; }

        public PageModel()
        {
            PageId = String.Empty;
            PageName = String.Empty;
            Categories = new List<CategoryModel>();
            About = String.Empty;
            Address = String.Empty;
            Link = String.Empty;
            Emails = new List<string>();
            Website = String.Empty;
            FanCount = 0;
            NewLikeCount = 0;
            CheckinCount = 0;
            CanCheckin = false;
            CanPost = false;
            Videos = new List<VideoModel>();
            CreatedTime = DateTime.UtcNow;
        }
    }

    public class CategoryModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        public CategoryModel()
        {
            CategoryId = String.Empty;
            CategoryName = String.Empty;
        }
    }

    public class VideoModel
    {
        [JsonProperty("VideoId")]
        public string VideoId { get; set; }

        [JsonProperty("VideoInside")]
        public VideoInsideModel Video { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("PermalinkUrl")]
        public string PermalinkUrl { get; set; }

        [JsonProperty("LiveViews")]
        public string LiveViews { get; set; }

        [JsonProperty("IsSendWhenDetectedPhone")]
        public bool IsSendWhenDetectedPhone { get; set; }

        [JsonProperty("IsSendWhenDetectedGoodWord")]
        public bool IsSendWhenDetectedGoodWord { get; set; }

        [JsonProperty("IsDeleteWhenDetectedBadWord")]
        public bool IsDeleteWhenDetectedBadWord { get; set; }

        [JsonProperty("IsHiddenWhenNegSentiment")]
        public bool IsHiddenWhenNegSentiment { get; set; }

        [JsonProperty("IsHidden")]
        public bool IsHidden { get; set; }

        [JsonProperty("GoodWords")]
        public List<string> GoodWords { get; set; }

        [JsonProperty("BadWords")]
        public List<string> BadWords { get; set; }

        [JsonProperty("ReplyTemplate")]
        public ReplyTemplateModel ReplyTemplate { get; set; }

        [JsonProperty("CreationTime")]
        public DateTime CreationTime { get; set; }

        public VideoModel()
        {
            VideoId = String.Empty;
            Video = new VideoInsideModel()
            {
                DisplayVideoId = String.Empty
            };
            Status = "Unknown";
            Title = String.Empty;
            Description = String.Empty;
            PermalinkUrl = String.Empty;
            LiveViews = String.Empty;
            IsSendWhenDetectedPhone = false;
            IsSendWhenDetectedGoodWord = false;
            IsDeleteWhenDetectedBadWord = false;
            IsHidden = false;
            GoodWords = new List<string>();
            BadWords = new List<string>();
            ReplyTemplate = new ReplyTemplateModel();
            CreationTime = DateTime.UtcNow; 
        }
    }

    public class VideoInsideModel
    {
        public string DisplayVideoId { get; set; }
    }

    public class ReplyTemplateModel
    {
        [JsonProperty("recipient")]
        public RecipientModel Recipient { get; set; }

        [JsonProperty("message")]
        public MessageModel Message { get; set; }

        public ReplyTemplateModel()
        {
            Recipient = new RecipientModel();
            Message = new MessageModel();
        }
    }

    public class RecipientModel
    {
        [JsonProperty("comment_id")]
        public string CommentId { get; set; }

        public RecipientModel()
        {
            CommentId = String.Empty;
        }
    }

    public class MessageModel
    {
        [JsonProperty("attachment")]
        public AttachmentModel Attachment { get; set; }

        public MessageModel()
        {
            Attachment = new AttachmentModel();
        }
    }

    public class AttachmentModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("payload")]
        public PayloadModel Payload { get; set; }

        public AttachmentModel()
        {
            Type = "template";
            Payload = new PayloadModel();
        }
    }

    public class PayloadModel
    {
        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("buttons")]
        public List<ButtonModel> Buttons { get; set; }

        public PayloadModel()
        {
            TemplateType = "button";
            Text = "Chào anh/chị, anh/chị vui lòng cung cấp số điện thoại và địa chỉ nhận hàng.";
            Buttons = new List<ButtonModel>()
            {
                new ButtonModel()
            };
        }
    }

    public class ButtonModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        public ButtonModel()
        {
            Type = "web_url";
            Url = "https://github.com/lequocdien";
            Title = "Đi đến website";
        }
    }
}
