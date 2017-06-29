using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OlosBotApp.Models;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace OlosBotApp.Controllers
{
    [RoutePrefix("api/OlosBackStageActivity")]
    public class OlosActivityController : ApiController
    {
        [AcceptVerbs("POST")]
        [Route("OlosSendMessage")]
        //public async Task<HttpResponseMessage> OlosSendMessage(OlosActivityModel Mensagem)
        public async Task<HttpResponseMessage> OlosSendMessage(OlosActivityModel OlosActivity)
        {
            try
            {
                if (!string.IsNullOrEmpty(OlosActivity.conversationId))
                    {

                    await Resume(OlosActivity); //We don't need to wait for this, just want to start the interruption here

                    //Return Success Message
                    var http_return = new { Code = "SUC-001", Message = "OK" };
                    return Request.CreateResponse(HttpStatusCode.OK, http_return);
                }
                else
                {
                    //Return Error Message
                    var http_return = new { Code = "ERR-100", Message = "You must start a conversation first."};
                    return Request.CreateResponse(HttpStatusCode.BadRequest, http_return);
                }
            }
            catch (System.UriFormatException ex)
            {
                //Return Error Message
                var http_return = new { Code = "ERR-101", Message = "Connector can´t be created. Check the serviceUrl.", Detail = ex.Message};
                return Request.CreateResponse(HttpStatusCode.BadRequest, http_return);
            }
            catch (Microsoft.Rest.HttpOperationException ex)
            {
                //Return Error Message
                var http_return = new { Code = "ERR-102", Message = "Conversation not found. Check the conversationId.", Detail = ex.Message };
                return Request.CreateResponse(HttpStatusCode.BadRequest, http_return);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        //Create and send a message to user
        public static async Task Resume(OlosActivityModel OlosActivity)
        {
            //Create ChannelsAccounts
            var botAccount = new ChannelAccount(OlosActivity.botId, OlosActivity.botName);
            var userAccount = new ChannelAccount(OlosActivity.userId, OlosActivity.userName);
            //Create Connector
            var connector = new ConnectorClient(new Uri(OlosActivity.serviceUrl));

            IMessageActivity message = Activity.CreateMessageActivity();

            if (!string.IsNullOrEmpty(OlosActivity.conversationId) && !string.IsNullOrEmpty(OlosActivity.channelId))
            {
                message.ChannelId = OlosActivity.channelId;
            }
            else
            {
                OlosActivity.conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
            }

            //Setup a message
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: OlosActivity.conversationId);
            message.Text = OlosActivity.text;
            message.Locale = "pt-BR";
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }

    }
}
