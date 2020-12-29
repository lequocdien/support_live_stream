using SupportLiveStream.Api.RequestModel;
using SupportLiveStream.Model;
using System.Collections.Generic;

namespace SupportLiveStream.Service.Commons
{
    public static class Extension
    {
        public static PrivateReplyReq ToModelReq(this ReplyTemplateModel replyTemplate)
        {
            var listOfButton = new List<ButtonPrivateReply>();
            foreach (var button in replyTemplate.Message.Attachment.Payload.Buttons)
            {
                listOfButton.Add(new ButtonPrivateReply()
                {
                    Type = button.Type,
                    Url = button.Url,
                    Title = button.Title
                });
            }

            var payload = new PayloadPrivateReply()
            {
                TemplateType = replyTemplate.Message.Attachment.Payload.TemplateType,
                Text = replyTemplate.Message.Attachment.Payload.Text,
                Buttons = listOfButton
            };

            var attachment = new AttachmentPrivateReply()
            {
                Type = replyTemplate.Message.Attachment.Type,
                Payload = payload
            };

            var message = new MessagePrivateReply()
            {
                Attachment = attachment
            };

            var recipient = new RecipientPrivateReply()
            {
                CommentId = replyTemplate.Recipient.CommentId
            };

            var privateReplyReq = new PrivateReplyReq()
            {
                Recipient = recipient,
                Message = message
            };

            return privateReplyReq;
        }
    }
}
