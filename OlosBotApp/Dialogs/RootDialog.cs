using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using OlosBotApp.Models;

namespace OlosBotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //We need to keep this data so we know who to send the message to. Assume this would be stored somewhere, e.g. an Azure Table
            OlosActivityModel.userId = activity.Recipient.Id;
            OlosActivityModel.userName = activity.Recipient.Name;
            OlosActivityModel.botId = activity.From.Id;
            OlosActivityModel.botName = activity.From.Name;
            OlosActivityModel.serviceUrl = activity.ServiceUrl;
            OlosActivityModel.channelId = activity.ChannelId;
            OlosActivityModel.conversationId = activity.Conversation.Id;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"Você enviou {activity.Text} [{length}] caracteres");

            context.Wait(MessageReceivedAsync);
        }
    }
}