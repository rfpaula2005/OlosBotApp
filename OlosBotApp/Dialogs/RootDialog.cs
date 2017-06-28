using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using OlosBotApp.Models;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;

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
            var activity = await result as Activity;

            ConnectorClient connector;
            DateTime dt_messageReceivedInicio;
            DateTime dt_messageReceivedFim;

            string http_code;
            //We need to keep this data so we know who to send the message to. Assume this would be stored somewhere, e.g. an Azure Table
            OlosActivityModel.userId = activity.Recipient.Id;
            OlosActivityModel.userName = activity.Recipient.Name;
            OlosActivityModel.botId = activity.From.Id;
            OlosActivityModel.botName = activity.From.Name;
            OlosActivityModel.serviceUrl = activity.ServiceUrl;
            OlosActivityModel.channelId = activity.ChannelId;
            OlosActivityModel.conversationId = activity.Conversation.Id;

            //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            //var temp = ((Microsoft.Bot.Connector.MicrosoftAppCredentials)connector.Credentials).MicrosoftAppId;

            try
            {
                dt_messageReceivedInicio = DateTime.Now;
                string pattern = "(http_code=)([0-9][0-9][0-9])";
                http_code = (Regex.Match(activity.Text, pattern, RegexOptions.IgnoreCase)).Value;
                string uri = "https://olosrepeaterfunction.azurewebsites.net/api/HttpTriggerCSharp1?code=ylw6l1SXaU6SqAae/4ee/Vq6fjNU6lYBXMdWTWeWPL8gznaLgHgaMA==&message=" + activity.Text + "&" + http_code;
                Task<string> getStringTask = Functions.AccessTheWebAsync(uri);
                string responseFromServer = await getStringTask;
                dt_messageReceivedFim = DateTime.Now;
                // return our reply to the user
                int length = (activity.Text ?? string.Empty).Length;
                await context.PostAsync($"Você enviou {activity.Text} [{length}] caracteres");

                connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var lc_appId = ((Microsoft.Bot.Connector.MicrosoftAppCredentials)connector.Credentials).MicrosoftAppId;
                var lc_appPass = ((Microsoft.Bot.Connector.MicrosoftAppCredentials)connector.Credentials).MicrosoftAppPassword;

                await context.PostAsync($"Message Count: {this.count++} \n\n appId: [{lc_appId}] \n\n userInfo:{activity.From.Id},{activity.From.Name} \n\n botInfo:{activity.Recipient.Id},{activity.Recipient.Name} \n\n ServiceUrl:{activity.ServiceUrl} \n\n ChannelId:{activity.ChannelId} \n\n Conversation:{activity.Conversation.Id} \n\n Duração: {(dt_messageReceivedInicio - dt_messageReceivedInicio).TotalSeconds} segundos \n\n {responseFromServer} ");
                context.Wait(MessageReceivedAsync);
            }
            catch (WebException wex)
            {
                await context.PostAsync($"Por favor me desculpe, no momento estamos passando por algumas dificuldades t�cnicas.Retorne mais tarde e teremos o maior prazer em ajudá-lo com a sua solicita��o.\n\n Execption:\n\n" + wex.Message);
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