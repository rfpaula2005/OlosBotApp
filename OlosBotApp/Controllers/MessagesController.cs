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
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;
using System.Configuration;




namespace OlosBotApp
{

    public class AppEntity : TableEntity
    {
        public string AppPassword { get; set; }
        public string BotId { get; set; }
        public string OlosEngineUri { get; set; }

        public AppEntity(string AppId, string AppPassword)
        {
            this.PartitionKey = "BotCredential";
            this.RowKey = AppId;
            this.AppPassword = AppPassword;
        }

        public AppEntity() { }
    }
    /// <summary>
    /// A sample ICredentialProvider that is configured by multiple MicrosoftAppIds and MicrosoftAppPasswords
    /// </summary>
    public class MultiCredentialProvider : ICredentialProvider
    {
        protected bool AuthenticationEnabled = true;
        public Task<bool> IsValidAppIdAsync(string appId)
        {
            // Set storage Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table named "OlosBotCredentials"
            CloudTable table = tableClient.GetTableReference("OlosBotCredentials");

            // Create a new customer entity.
            AppEntity botApp1 = new AppEntity(ConfigurationManager.AppSettings["MicrosoftAppId"], ConfigurationManager.AppSettings["MicrosoftAppPassword"]);
            botApp1.BotId = ConfigurationManager.AppSettings["BotId"];
            botApp1.OlosEngineUri = "https://xxx.yyy.com.br";

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(botApp1);
            
            // Execute the insert operation.
            table.Execute(insertOperation);
          

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<AppEntity>("BotCredential", appId);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            return Task.FromResult(((AppEntity)retrievedResult.Result).RowKey.Equals(appId));
        }

        public Task<string> GetAppPasswordAsync(string appId)
        {
            // Set storage Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table named "OlosBotCredentials"
            CloudTable table = tableClient.GetTableReference("OlosBotCredentials");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<AppEntity>("BotCredential", appId);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            return Task.FromResult(((AppEntity)retrievedResult.Result).RowKey.Equals(appId) ? ((AppEntity)retrievedResult.Result).AppPassword : null);
        }

        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(!AuthenticationEnabled);
        }
    }

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
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
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