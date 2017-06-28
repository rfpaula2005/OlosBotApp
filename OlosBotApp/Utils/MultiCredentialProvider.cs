using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace OlosBotApp.Utils

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
}