using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Collections.Generic;

namespace OlosBotApp.Utils
{
    //Allow the reuse of Entity on the RootDialog class
    [Serializable]
    public class AppEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string AppPassword { get; set; }
        public string BotId { get; set; }
        public string OlosEngineUri { get; set; }
        public bool ativo { get; set; }


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


    //Used to get data form TableEntity
    public class TElementAppEntity : TableEntity
    {
        public string AppPassword { get; set; }
        public string BotId { get; set; }
        public string OlosEngineUri { get; set; }
        public bool ativo { get; set; }

        public TElementAppEntity(string AppId, string AppPassword)
        {
            this.PartitionKey = "BotCredential";
            this.RowKey = AppId;
            this.AppPassword = AppPassword;
        }

        public TElementAppEntity(string PartitionKey, string AppId, string AppPassword)
        {
            this.PartitionKey = PartitionKey;
            this.RowKey = AppId;
            this.AppPassword = AppPassword;
        }

        public TElementAppEntity() { }

        public AppEntity ConvertToAppEntity()
        {
            AppEntity ObjAppEntity = new AppEntity();
            ObjAppEntity.PartitionKey = this.PartitionKey;
            ObjAppEntity.RowKey = this.RowKey;
            ObjAppEntity.AppPassword = this.AppPassword;
            ObjAppEntity.BotId = this.BotId;
            ObjAppEntity.OlosEngineUri = this.OlosEngineUri;
            ObjAppEntity.ativo = this.ativo;

            return ObjAppEntity;
        }
    }


    public class AzureCloudStorageTable
    {

        public static CloudTable getCloudTable(string strCloudTableName)
        {
            // Set storage Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            // Get a reference to a table named "OlosBotCredentials"
            CloudTable table = tableClient.GetTableReference(strCloudTableName);

            return table;
        }

        public static CloudTable createCloudTable(string strCloudTableName)
        {
            // Get a reference to a strCloudTableName table
            CloudTable table = getCloudTable(strCloudTableName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            return table;
        }


        public static TableResult getAppEntity(string strCloudTableName, string PartitionKey, string RowKey)
        {
            // Get a reference to a strCloudTableName table
            CloudTable table = getCloudTable(strCloudTableName);

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TElementAppEntity>(PartitionKey, RowKey);
            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            return retrievedResult;
        }

        public static AppEntity getAppEntityData(string strCloudTableName, string PartitionKey, string RowKey)
        {
            // Get a reference to a strCloudTableName table
            CloudTable table = getCloudTable(strCloudTableName);

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TElementAppEntity>(PartitionKey, RowKey);
            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            // Convert to AppEntity 
            AppEntity retrievedAppEntity = ((TElementAppEntity)retrievedResult.Result).ConvertToAppEntity();

            return retrievedAppEntity;
        }



        public static void insAppEntity(string strCloudTableName, string PartitionKey, string RowKey, string appPasword, string botId, string OlosEngineUri, bool ativo)
        {
            // Get a reference to a strCloudTableName table
            CloudTable table = getCloudTable(strCloudTableName);

            // Create a new customer entity.
            TElementAppEntity appBot = new TElementAppEntity(PartitionKey, RowKey, appPasword);
            appBot.BotId = botId;
            appBot.OlosEngineUri = OlosEngineUri;
            appBot.ativo = ativo;

            // Create the TableOperation object that inserts the appBot entity.
            TableOperation insertOperation = TableOperation.Insert(appBot);
            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        public static void updAppEntity(string strCloudTableName, string PartitionKey, string RowKey, string appPasword, string botId, string OlosEngineUri, bool ativo)
        {

            TableResult retrievedResult = getAppEntity(strCloudTableName, PartitionKey, RowKey);

            // Assign the result to a CustomerEntity object.
            TElementAppEntity updateEntity = (TElementAppEntity)retrievedResult.Result;

            if (updateEntity != null)
            {
                // Change data.
                updateEntity.AppPassword = appPasword;
                updateEntity.BotId = botId;
                updateEntity.OlosEngineUri = OlosEngineUri;
                updateEntity.ativo = ativo;

                // Create the Replace TableOperation.
                TableOperation updateOperation = TableOperation.Replace(updateEntity);
                // Get table
                CloudTable table = getCloudTable(strCloudTableName);
                // Execute the operation.
                table.Execute(updateOperation);
            }
            else
            {
                //Console.WriteLine("Entity could not be retrieved.");
            }

        }


        public static void delAppEntity(string strCloudTableName, string PartitionKey, string RowKey)
        {

            TableResult retrievedResult = getAppEntity(strCloudTableName, PartitionKey, RowKey);

            // Assign the result to a CustomerEntity object.
            TElementAppEntity deleteEntity = (TElementAppEntity)retrievedResult.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Get table
                CloudTable table = getCloudTable(strCloudTableName);
                // Execute the operation.
                table.Execute(deleteOperation);
                //Console.WriteLine("Entity deleted.");
            }
            else
            {
                //Console.WriteLine("Could not retrieve the entity.");
            }
        }



        public static List<AppEntity> getAllEntities(string strCloudTableName, string PartitionKey)
        {
            // Get a reference to a strCloudTableName table
            CloudTable table = getCloudTable(strCloudTableName);

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<TElementAppEntity> query = new TableQuery<TElementAppEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey));

            // Print the fields for each customer.
            // Create a new customer entity.
            List<AppEntity> appEntityList = new List<AppEntity>();
            foreach (TElementAppEntity entity in table.ExecuteQuery(query))
            {
                AppEntity item = entity.ConvertToAppEntity();
                appEntityList.Add(item);
            }
            return appEntityList;
        }

    }


}




