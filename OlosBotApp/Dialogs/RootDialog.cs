using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.ConnectorEx;
using OlosBotApp.Utils;
using Olos.BotProtocol;
using System.Security.Claims;
using System.Web;

namespace OlosBotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;
        //Get a PartitionKey
        protected string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";
        protected string GatewayUrl = (ConfigurationManager.AppSettings["GatewayUrl"] != null) ? ConfigurationManager.AppSettings["GatewayUrl"] : "";

        //Get the AppEntity related to the current AppId
        protected AppEntity ObjAppEntity;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            //Patern vars
            var activity = await result as Activity;
            //Strore de Message data;
            string messageJson;

            try
            {
                var lc_appId = ((ClaimsIdentity)HttpContext.Current.User.Identity).GetCredentialsFromClaims().MicrosoftAppId;
                //var lc_appId = Microsoft.Bot.Connector.ClaimsIdentityEx.GetAppIdFromClaims((ClaimsIdentity)HttpContext.Current.User.Identity);

                //Check if ObjAppEntity have already created
                if (ObjAppEntity == null)
                {
                    //Check appId on AzureTable OlosBotCredentials and return the data
                    ObjAppEntity = Utils.AzureCloudStorageTable.getAppEntityData("OlosBotCredentials", PartitionKey, lc_appId);
                }


                Message ObjMessage = Message.ConvertToMessage(activity);
                ObjMessage.AppId = lc_appId;
                ObjMessage.GatewayUrl = GatewayUrl;
                messageJson = ObjMessage.GetJsonN();

                await context.PostAsync($"Message Count: {this.count++} \n\n appId: [{lc_appId}] \n\n ConversationReference:{messageJson} \n\n\n\n ");
                string responseFromServer = OlosFunctions.PostJson(ObjAppEntity.OlosEngineUri, messageJson);

                await context.PostAsync($"Message Count: {this.count++} \n\n appId: [{lc_appId}] \n\n ConversationReference:{messageJson} \n\n\n\n ================  \n\n\n\n {responseFromServer} ");
                //await context.PostAsync($"Message Count: {this.count++} \n\n appId: [{lc_appId}] \n\n ConversationReference:{messageJson} \n\n\n\n ");
                context.Wait(MessageReceivedAsync);
            }
            catch (WebException wex)
            {
                await context.PostAsync($"Por favor me desculpe, no momento estamos passando por algumas dificuldades técnicas.Retorne mais tarde e teremos o maior prazer em ajudá-lo com a sua solicitação.\n\n Execption:\n\n\n\n" + ObjAppEntity.OlosEngineUri + "\n\n\n\n" + wex.Message);
                context.Wait(MessageReceivedAsync);
            }

            /*
            context.Wait(MessageReceivedAsync);
            */
        }
    }
}