using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

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

        public AppEntity(string PartitionKey, string AppId, string AppPassword)
        {
            this.PartitionKey = PartitionKey;
            this.RowKey = AppId;
            this.AppPassword = AppPassword;
        }

        public AppEntity() { }
    }

    public class AzureCloudStorageTable
    {

        public static CloudTable createCloudTable(string strCloudTableName)
        {
            // Set storage Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Set storage Table PartionKey
            string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table named "OlosBotCredentials"
            CloudTable table = tableClient.GetTableReference(strCloudTableName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            return table;
        }

        public static CloudTable getCloudTable(string strCloudTableName)
        {
            // Set storage Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Set storage Table PartionKey
            string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table named "OlosBotCredentials"
            CloudTable table = tableClient.GetTableReference(strCloudTableName);

            return table;
        }

        public static TableResult getAppEntity(string strCloudTableName, string PartitionKey, string RowKey)
        {
            // Set storage Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table named "OlosBotCredentials"
            CloudTable table = tableClient.GetTableReference(strCloudTableName);
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<AppEntity>(PartitionKey, RowKey);
            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            return retrievedResult;
        }

        public static void insAppEntity(string strCloudTableName, string PartitionKey, string RowKey, string appPasword, string botId, string OlosEngineUri)
        {
            // Set storage Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table named "OlosBotCredentials"
            CloudTable table = tableClient.GetTableReference(strCloudTableName);

            // Create a new customer entity.
            AppEntity appBot = new AppEntity(PartitionKey, RowKey, appPasword);
            appBot.BotId = botId;
            appBot.OlosEngineUri = OlosEngineUri;

            // Create the TableOperation object that inserts the appBot entity.
            TableOperation insertOperation = TableOperation.Insert(appBot);
            // Execute the insert operation.
            table.Execute(insertOperation);
        }

    }


}




