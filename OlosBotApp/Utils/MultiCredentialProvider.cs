using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace OlosBotApp.Utils

{


    /// <summary>
    /// A sample ICredentialProvider that is configured by multiple MicrosoftAppIds and MicrosoftAppPasswords
    /// </summary>
    public class MultiCredentialProvider : ICredentialProvider
    {
        protected bool AuthenticationEnabled = true;

        public Task<bool> IsValidAppIdAsync(string appId)
        {
            //Get a PartitionKey
            string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";
            //Check appId on AzureTable OlosBotCredentials and return the data
            TableResult retrievedResult = Utils.AzureCloudStorageTable.getAppEntity("OlosBotCredentials", PartitionKey, appId);

            return Task.FromResult(((AppEntity)retrievedResult.Result).RowKey.Equals(appId));
        }

        public Task<string> GetAppPasswordAsync(string appId)
        {
            //Get a PartitionKey
            string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";
            //Check appId on AzureTable OlosBotCredentials and return the data
            TableResult retrievedResult = Utils.AzureCloudStorageTable.getAppEntity("OlosBotCredentials", PartitionKey, appId);

            return Task.FromResult(((AppEntity)retrievedResult.Result).RowKey.Equals(appId) ? ((AppEntity)retrievedResult.Result).AppPassword : null);
        }

        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(!AuthenticationEnabled);
        }
    }
}