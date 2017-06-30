using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Olos.BotProtocol
{
    [Serializable]
    public class Message
    {
        public string AppId { get; set; }

        public string MessageType { get; set; }

        public string ChannelId { get; set; }

        public string ConversationId { get; set; }

        public string Timestamp { get; set; }

        public Account From { get; set; }

        public Account To { get; set; }

        public string Text { get; set; }

        public string Locale { get; set; }

        public string TextFormat { get; set; } // markdown ou xml

        public string ServiceUrl { get; set; }

        public string GatewayUrl { get; set; }

        public string ReplayToId { get; set; }

        public List<Attach> Attachies { get; set; }

        public List<SuggestionAction> SugestAction { get; set; }

        public string GetJson()
        {
            string json = new JavaScriptSerializer().Serialize(this);
            return json;
        }


        public static Message SetJson(string json)
        {
            Message Obj = new JavaScriptSerializer().Deserialize<Message>(json);
            return Obj;
        }

        public Activity ConvertToActivity()
        {
            Activity activity = new Activity();
            activity.Type = "message";
            activity.ReplyToId = this.ReplayToId;
            activity.ChannelId = this.ChannelId;
            activity.Conversation = new ConversationAccount(null, this.ConversationId, null);        
            activity.Locale = this.Locale;           
            activity.ServiceUrl = this.ServiceUrl;           
            activity.Text = this.Text;
            activity.TextFormat = this.TextFormat;
            activity.From = this.From.ConvertToChannelAccount();
            activity.Recipient = this.To.ConvertToChannelAccount();
            if(this.SugestAction != null)
            {
                activity.SuggestedActions = BotProtocol.SuggestionAction.ConvertToSuggestedActions(this.SugestAction);
            }

            if (this.Attachies != null)
            {
                activity.Attachments = Attach.ConvertToAttachment(this.Attachies);
            }

            activity.Timestamp = Convert.ToDateTime(this.Timestamp);

            return activity;
        }

        public static Message ConvertToMessage(Activity activity)
        {
            Message Obj = new Message();        
            
            Obj.AppId = "";
            Obj.ReplayToId = activity.ReplyToId;
            Obj.ChannelId = activity.ChannelId;
            Obj.ConversationId = activity.Conversation.Id;           
            Obj.GatewayUrl = "";
            Obj.Locale = activity.Locale;
            Obj.MessageType = activity.Type;
            Obj.ServiceUrl = activity.ServiceUrl;            
            Obj.Text = activity.Text;
            Obj.TextFormat = activity.TextFormat;
            Obj.Timestamp = activity.Timestamp.ToString();
            Obj.To = Account.ConvertToAccount(activity.Recipient);
            Obj.From = Account.ConvertToAccount(activity.From);

            if (activity.SuggestedActions != null)
            {
                Obj.SugestAction = SuggestionAction.ConvertToSuggestionAction(activity.SuggestedActions);
            }

            if (activity.Attachments.Count > 0)
            {
                List<Attachment> ListaA = (List<Attachment>)activity.Attachments;
                Obj.Attachies = Attach.ConvertToAttach(ListaA);
            }

            return Obj;
        }

    }
}
