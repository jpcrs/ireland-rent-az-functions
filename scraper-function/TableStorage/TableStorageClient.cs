using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using scraper_function.Model;

namespace scraper_function.TableStorage
{
    public class TableStorageClient
    {
        private string _connectionString;
        private CloudTable _table;

        public TableStorageClient(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _table = tableClient.GetTableReference(tableName);
            _table.CreateIfNotExistsAsync();
        }

        public bool CheckIfInCache(string url)
        {
            var hashedUrl = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
            TableOperation retrieveOperation = TableOperation.Retrieve<DaftEntity>(hashedUrl, hashedUrl);

            TableResult retrievedResult = _table.Execute(retrieveOperation);
            if (retrievedResult.Result == null)
            {
                InsertRent(hashedUrl);
                return false;
            }

            return true;
        }

        internal void InsertRent(string url)
        {
            var entity = new DaftEntity()
            {
                PartitionKey = url,
                RowKey = url
            };

            TableOperation insertOperation = TableOperation.Insert(entity);

            _table.ExecuteAsync(insertOperation);
        }
    }
}
