using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Web;
using Olos.BotProtocol;
using OlosBotApp.Utils;
using System.Collections.Generic;


namespace OlosBotApp.Controllers
{
    [RoutePrefix("api/OlosBackStageActivity")]
    public class OlosActivityController : ApiController
    {
        private HttpContext httpContext;

        [AcceptVerbs("POST")]
        [Route("OlosSendMessage")]
        public async Task<HttpResponseMessage> OlosSendMessage(Message OlosMessage)
        {
            Utils.Log.Info("================== OlosActivityController::OlosSendMessage ================== ");
            Utils.Log.Warn("[OlosActivityController::OlosSendMessage] Message Received ", OlosMessage);

            httpContext = HttpContext.Current;
            try
            {
                KeyValuePair<string, string>[] v_Cretentials = OlosFunctions.getHttpCredentials(httpContext);

                Utils.Log.Info("[OlosActivityController::OlosSendMessage] Verifying credentials");

                //Verify Credentials
                if (OlosFunctions.verifyCredentials(v_Cretentials))
                {
                    if (!string.IsNullOrEmpty(OlosMessage.ConversationId))
                    {
                        Utils.Log.Info("[OlosActivityController::OlosSendMessage] Resume Called - Send message to bot");
                        await Resume(OlosMessage);

                        //Return Success Message
                        var http_return = new { Code = "SUC-001", Message = "OK" };
                        Utils.Log.Warn("[OlosActivityController::OlosSendMessage] Return success to caller", http_return);
                        return Request.CreateResponse(HttpStatusCode.OK, http_return);
                    }
                    else
                    {
                        //Return Error Message
                        var http_return = new { Code = "ERR-100", Message = "You must start a conversation first." };
                        Utils.Log.Warn("[OlosActivityController::OlosSendMessage] Return errror to caller", http_return);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, http_return);
                    }
                }
                else
                {
                    //Handle what happens if that isn't the case
                    Utils.Log.Error("[OlosActivityController::OlosSendMessage] Username or pssword incorrect", v_Cretentials);
                    throw new Exception("The authorization username or password incorrect.");
                }
            }
            catch (System.UriFormatException ex)
            {
                //Return Error Message
                var http_return = new { Code = "ERR-101", Message = "Connector can´t be created. Check the serviceUrl.", Detail = ex.Message};
                Utils.Log.Error("[OlosActivityController::OlosSendMessage] Connector can´t be created. Check the serviceUrl.", ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, http_return);
            }
            catch (Microsoft.Rest.HttpOperationException ex)
            {
                //Return Error Message
                var http_return = new { Code = "ERR-102", Message = "Conversation not found. Check the conversationId.", Detail = ex.Message };
                Utils.Log.Error("[OlosActivityController::OlosSendMessage] Conversation not found. Check the conversationId.", ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, http_return);
            }
            catch (Exception ex)
            {
                Utils.Log.Error("[OlosActivityController::OlosSendMessage] Error.", ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        //Create and send a message to user
        public static async Task Resume(Message OlosMessage)
        {
            
            Utils.Log.Info("================== OlosActivityController::Resume ================== ");
            Utils.Log.Info("[OlosActivityController::Resume] Creating connector.");
            //Create Connector
            var connector = new ConnectorClient(new Uri(OlosMessage.ServiceUrl), OlosMessage.AppId);

            Utils.Log.Info("[OlosActivityController::Resume] Creating activity.");
            IMessageActivity message = Activity.CreateMessageActivity();
            Utils.Log.Info("[OlosActivityController::Resume] Activity created.");

            if (!string.IsNullOrEmpty(OlosMessage.ConversationId) && !string.IsNullOrEmpty(OlosMessage.ChannelId))
            {
                Utils.Log.Info("[OlosActivityController::Resume] Activity validated.");
                message.ChannelId = OlosMessage.ChannelId;
            }
            else
            {
                Utils.Log.Info("[OlosActivityController::Resume] Activity rebuilt.");
                OlosMessage.ConversationId = (await connector.Conversations.CreateDirectConversationAsync(OlosMessage.From.ConvertToChannelAccount(), OlosMessage.To.ConvertToChannelAccount())).Id;
            }

            Utils.Log.Warn("[OlosActivityController::Resume] Sending message to user", (Activity)OlosMessage.ConvertToActivity());
            await connector.Conversations.SendToConversationAsync((Activity)OlosMessage.ConvertToActivity());
        }

    }
}
