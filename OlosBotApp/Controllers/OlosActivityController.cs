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
                    var http_return = new { Code = "001", Message = "Sucesso." };
                    return Request.CreateResponse(HttpStatusCode.OK, http_return);
                }
                else
                {
                    //Return Error Message
                    var http_return = new { Code = "100", Message = "Você precisa iniciar uma conversa com o bot primeiro."};
                    return Request.CreateResponse(HttpStatusCode.OK, http_return);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

            /*
            try
            {
                var http_return = new  {Code = "001", Message = "Sucesso."};
                return Request.CreateResponse(HttpStatusCode.OK, http_return);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            */

            //var resp = new HttpResponseMessage(HttpStatusCode.OK);
            //resp.Content = new StringContent("Usuário cadastrado com sucesso!", System.Text.Encoding.UTF8, @"text/html");
            //return resp;
            //Implementar aqui
            //return "Usuário cadastrado com sucesso!";
        }


        public static async Task Resume(OlosActivityModel OlosActivity)
        {

            var botAccount = new ChannelAccount(OlosActivity.botId, OlosActivity.botName);
            var userAccount = new ChannelAccount(OlosActivity.userId, OlosActivity.userName);
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

            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: OlosActivity.conversationId);
            message.Text = "Hello, this is a notification";
            message.Locale = "pt-BR";
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }

    }
}
