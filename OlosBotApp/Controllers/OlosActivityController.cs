using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OlosBotApp.Models;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Olos.BotProtocol;

namespace OlosBotApp.Controllers
{
    [RoutePrefix("api/OlosBackStageActivity")]
    public class OlosActivityController : ApiController
    {
        [AcceptVerbs("POST")]
        [Route("OlosSendMessage")]
        //public async Task<HttpResponseMessage> OlosSendMessage(OlosActivityModel Mensagem)
        public async Task<HttpResponseMessage> OlosSendMessage(Message OlosMessage)
        {
            try
            {
                if (!string.IsNullOrEmpty(OlosMessage.ConversationId))
                    {

                    await Resume(OlosMessage); //We don't need to wait for this, just want to start the interruption here

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
        public static async Task Resume(Message OlosMessage2)
        {
            //Create ChannelsAccounts
            //var botAccount = new ChannelAccount(OlosMessage2.botId, OlosMessage2.botName);
            //var userAccount = new ChannelAccount(OlosMessage2.userId, OlosMessage2.userName);
            //Create Connector

            var connector = new ConnectorClient(new Uri(OlosMessage2.ServiceUrl));

            IMessageActivity message = Activity.CreateMessageActivity();

            if (!string.IsNullOrEmpty(OlosMessage2.ConversationId) && !string.IsNullOrEmpty(OlosMessage2.ChannelId))
            {
                message.ChannelId = OlosMessage2.ChannelId;
            }
            else
            {
                OlosMessage2.ConversationId = (await connector.Conversations.CreateDirectConversationAsync(OlosMessage2.From.ConvertToChannelAccount(), OlosMessage2.To.ConvertToChannelAccount())).Id;
            }

            await connector.Conversations.SendToConversationAsync((Activity)OlosMessage2.ConvertToActivity());


            //Setup a message
            //message.From = botAccount;
            //message.Recipient = userAccount;
            //message.Conversation = new ConversationAccount(id: OlosMessage2.conversationId);
            //message.Text = OlosMessage2.text;
            //message.Locale = "pt-BR";
            //await connector.Conversations.SendToConversationAsync((Activity)message);
        }

    }
}
