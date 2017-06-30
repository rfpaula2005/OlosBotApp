using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using OlosBotApp.Models;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.ConnectorEx;
using OlosBotApp.Utils;
using Olos.BotProtocol;
//using System.Web;
//using System.Security.Claims;


namespace OlosBotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            //Patern vars
            var activity = await result as Activity;

            //Olos Add Vars
            //Store de reference to user and conversation (must be storage in a permanent repository)
            //ConversationReference conversationReference = activity.ToConversationReference();
            //string str_conversationReference = JsonConvert.SerializeObject(conversationReference);
            string str_conversationReference;
            ConnectorClient connector;
            string http_code;

            //We need to keep this data so we know who to send the message to. Assume this would be stored somewhere, e.g. an Azure Table
            //OlosActivityModel.userId = activity.Recipient.Id;

            //connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            try
            {
                //Get a PartitionKey
                string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";

                connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var lc_appId = ((Microsoft.Bot.Connector.MicrosoftAppCredentials)connector.Credentials).MicrosoftAppId;
                var lc_appPass = ((Microsoft.Bot.Connector.MicrosoftAppCredentials)connector.Credentials).MicrosoftAppPassword;

                //Check appId on AzureTable OlosBotCredentials and return the data
                //Tentar remover este acesso da Table
                AppEntity retrievedAppEntity = Utils.AzureCloudStorageTable.getAppEntityData("OlosBotCredentials", PartitionKey, lc_appId);

                Message ObjMessage = Message.ConvertToMessage(activity);
                ObjMessage.AppId = lc_appId;
                str_conversationReference = ObjMessage.GetJson();

                string pattern = "(http_code=)([0-9][0-9][0-9])";
                http_code = (Regex.Match(activity.Text, pattern, RegexOptions.IgnoreCase)).Value;
                string uri = retrievedAppEntity.OlosEngineUri + "&message=" + activity.Text + "&conversationreference=" + str_conversationReference + "&" + http_code;
                Task<string> getStringTask = OlosFunctions.AccessTheWebAsync(uri);
                string responseFromServer = await getStringTask;

                // return our reply to the user
                //int length = (activity.Text ?? string.Empty).Length;
                //await context.PostAsync($"Você enviou {activity.Text} [{length}] caracteres");


                //string x = Microsoft.Bot.Connector.ClaimsIdentityEx.GetAppIdFromClaims((ClaimsIdentity)HttpContext.Current.User.Identity);
                //x = Microsoft.Bot.Connector.ClaimsIdentityEx.GetAppPasswordFromClaims((ClaimsIdentity)HttpContext.Current.User.Identity);


                await context.PostAsync($"Message Count: {this.count++} \n\n appId: [{lc_appId}] \n\n ConversationReference:{str_conversationReference} \n\n {responseFromServer} ");
                context.Wait(MessageReceivedAsync);
            }
            catch (WebException wex)
            {
                await context.PostAsync($"Por favor me desculpe, no momento estamos passando por algumas dificuldades técnicas.Retorne mais tarde e teremos o maior prazer em ajudá-lo com a sua solicitação.\n\n Execption:\n\n" + wex.Message);
                context.Wait(MessageReceivedAsync);
            }


            /*
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"Você enviou {activity.Text} [{length}] caracteres");

            context.Wait(MessageReceivedAsync);
            */
        }
    }
}