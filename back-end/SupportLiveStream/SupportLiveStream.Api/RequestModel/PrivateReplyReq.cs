using Newtonsoft.Json;
using System.Collections.Generic;

namespace SupportLiveStream.Api.RequestModel
{
    public class PrivateReplyReq
    {
        [JsonProperty("recipient")]
        public RecipientPrivateReply Recipient { get; set; }

        [JsonProperty("message")]
        public MessagePrivateReply Message { get; set; }
    }

    public class RecipientPrivateReply
    {
        [JsonProperty("comment_id")]
        public string CommentId { get; set; }
    }

    public class MessagePrivateReply
    {
        [JsonProperty("attachment")]
        public AttachmentPrivateReply Attachment { get; set; }
    }

    public class AttachmentPrivateReply
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("payload")]
        public PayloadPrivateReply Payload { get; set; }
    }

    public class PayloadPrivateReply
    {
        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("buttons")]
        public List<ButtonPrivateReply> Buttons { get; set; }
    }

    public class ButtonPrivateReply
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
