using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Table;

namespace OlosBotApp.Utils

{
    /// <summary>
    /// A sample ICredentialProvider that is configured by multiple MicrosoftAppIds and MicrosoftAppPasswords
    /// </summary>
    public class MultiCredentialProvider : ICredentialProvider
    {
        protected bool AuthenticationEnabled = true;
        //Get a PartitionKey
        protected string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";
        //Get a UseLocalCredentials
        protected bool UseLocalCredentials = ((ConfigurationManager.AppSettings["UseLocalCredentials"] != null) ? ConfigurationManager.AppSettings["UseLocalCredentials"] : "false") == "false" ? false : true;
        //Get the AppEntity related to the current AppId
        protected AppEntity ObjAppEntity;

        public Task<bool> IsValidAppIdAsync(string appId)
        {
            //Get a PartitionKey
            //string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";
            //Check appId on AzureTable OlosBotCredentials and return the data
            //TableResult retrievedResult = Utils.AzureCloudStorageTable.getAppEntity("OlosBotCredentials", PartitionKey, appId);
            //AppEntity retrievedAppEntity = Utils.AzureCloudStorageTable.getAppEntityData("OlosBotCredentials", PartitionKey, appId);
            //return Task.FromResult(retrievedAppEntity.RowKey.Equals(appId));

            if (!UseLocalCredentials)
            {
                //Check if ObjAppEntity have already created
                if (ObjAppEntity == null)
                {
                    //Check appId on AzureTable OlosBotCredentials and return the data
                    ObjAppEntity = Utils.AzureCloudStorageTable.getAppEntityData("OlosBotCredentials", PartitionKey, appId);
                }
                return Task.FromResult(ObjAppEntity.RowKey.Equals(appId));
            } else
            {
                return Task.FromResult(ConfigurationManager.AppSettings["MicrosoftAppId"].Equals(appId));
            }

        }

        public Task<string> GetAppPasswordAsync(string appId)
        {
            //Get a PartitionKey
            //string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";
            //Check appId on AzureTable OlosBotCredentials and return the data
            //TableResult retrievedResult = Utils.AzureCloudStorageTable.getAppEntity("OlosBotCredentials", PartitionKey, appId);
            //AppEntity retrievedAppEntity = Utils.AzureCloudStorageTable.getAppEntityData("OlosBotCredentials", PartitionKey, appId);
            //return Task.FromResult(retrievedAppEntity.RowKey.Equals(appId) ? retrievedAppEntity.AppPassword : null);

            if (!UseLocalCredentials)
            {
                //Check if ObjAppEntity have already created
                if (ObjAppEntity == null)
                {
                    //Check appId on AzureTable OlosBotCredentials and return the data
                    ObjAppEntity = Utils.AzureCloudStorageTable.getAppEntityData("OlosBotCredentials", PartitionKey, appId);
                }
                return Task.FromResult(ObjAppEntity.RowKey.Equals(appId) ? ObjAppEntity.AppPassword : null);
            } else
            {
                return Task.FromResult(ConfigurationManager.AppSettings["MicrosoftAppId"].Equals(appId) ? ConfigurationManager.AppSettings["MicrosoftAppPassword"] : null);
            }
        }

        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(!AuthenticationEnabled);
        }
    }
}