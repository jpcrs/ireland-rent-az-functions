using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
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

        public async Task<IElement> CheckIfInCache(IElement topico)
        {
            var url = topico.QuerySelector(".search_result_title_box a").GetAttribute("href").Replace("'", "");
            var hashedUrl = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
            TableOperation retrieveOperation = TableOperation.Retrieve<DaftEntity>(hashedUrl, hashedUrl);

            TableResult retrievedResult = await _table.ExecuteAsync(retrieveOperation);
            if (retrievedResult.Result == null)
            {
                await InsertRent(hashedUrl);
                return topico;
            }

            return null;
        }

        internal async Task InsertRent(string url)
        {
            var entity = new DaftEntity()
            {
                PartitionKey = url,
                RowKey = url
            };

            TableOperation insertOperation = TableOperation.Insert(entity);

            await _table.ExecuteAsync(insertOperation);
        }
    }
}
