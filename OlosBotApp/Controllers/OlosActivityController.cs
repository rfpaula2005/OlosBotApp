using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<HttpResponseMessage> OlosSendMessage(OlosActivityModel Mensagem)
        {

            try
            {
                if (!string.IsNullOrEmpty(OlosActivityModel.botId))
                {
                    await Resume(OlosActivityModel.conversationId, OlosActivityModel.channelId); //We don't need to wait for this, just want to start the interruption here

                    var http_return = new { Code = "001", Message = "Sucesso." };
                    return Request.CreateResponse(HttpStatusCode.OK, http_return);
                }
                else
                {
                    var http_return = new { Code = "100", Message = "Você precisa iniciar uma conversa com o bot primeiro." + OlosActivityModel.botId + "->" + OlosActivityModel.channelId};
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


        public static async Task Resume(string conversationId, string channelId)
        {
            //var userAccount = new ChannelAccount(OlosActivityModel.userId, OlosActivityModel.userName);
            //var botAccount = new ChannelAccount(OlosActivityModel.botId, OlosActivityModel.botName);
            var botAccount = new ChannelAccount(OlosActivityModel.botId, OlosActivityModel.botName);
            var userAccount = new ChannelAccount(OlosActivityModel.userId, OlosActivityModel.userName);
            var connector = new ConnectorClient(new Uri(OlosActivityModel.serviceUrl));

            IMessageActivity message = Activity.CreateMessageActivity();

            if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
            {
                message.ChannelId = channelId;
            }
            else
            {
                OlosActivityModel.conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
            }

            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: OlosActivityModel.conversationId);
            message.Text = "Hello, this is a notification";
            message.Locale = "pt-BR";
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }


    }
}
