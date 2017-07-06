using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Autofac;
using System.Security.Claims;
using System.Web;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs.Internals;
using OlosBotApp.Utils;



namespace OlosBotApp
{

    /// Use the MultiCredentialProvider as credential provider for BotAuthentication
    [BotAuthentication(CredentialProviderType = typeof(MultiCredentialProvider))]

    public class MessagesController : ApiController
    {
        static MessagesController()
        {

            // Update the container to use the right MicorosftAppCredentials based on
            // Identity set by BotAuthentication
            var builder = new ContainerBuilder();

            builder.Register(c => ((ClaimsIdentity)HttpContext.Current.User.Identity).GetCredentialsFromClaims())
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.Update(Conversation.Container);
        }


        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            Utils.Log.Info("================== MessagesController::Post ================== ");
            if (activity.Type == ActivityTypes.Message)
            {
                Utils.Log.Info("[MessagesController::Post] Activity Message Received");
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                Utils.Log.Info("[MessagesController::Post] Activity ConversationUpdate Received");
                IConversationUpdateActivity update = activity;
                // resolve the connector client from the container to make sure that it is 
                // instantiated with the right MicrosoftAppCredentials
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (update.MembersAdded.Any())
                    {
                        var reply = activity.CreateReply();
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != activity.Recipient.Id)
                            {
                                reply.Text = $"Bem-vindo {newMember.Name}!";
                                await client.Conversations.ReplyToActivityAsync(reply);
                            }
                        }
                    }
                }
            }
            else
            {
                Utils.Log.Info("[MessagesController::Post] HandleSystemMessage Called");
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            Utils.Log.Info("================== MessagesController::HandleSystemMessage ================== ");
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}