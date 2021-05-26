using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using InfrastructureForMoon.Interfaces;
using RepositoriesForMoon.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesForMoon
{
    public class DarkSideRepository : IDarkSideRepository
    {
        private readonly IDynamoClient _dynamoClient;
        private readonly Table _darkSideTable;
        private readonly string TableName = "darkside_tab";

        public DarkSideRepository(IDynamoClient dynamoClient)
        {
            _dynamoClient = dynamoClient;
            //Get this out of here
            _darkSideTable = _dynamoClient.LoadTable(TableName);
        }
        public async Task<List<Document>> ScanItemsAsync(ScanOperationConfig config)
        {
            List<Document> docList = new List<Document>();
            Task<List<Document>> getNextBatch;

            // you can add scan conditions, or leave empty
            var searchResults = _darkSideTable.Scan(config);           

            do
            {
                try
                {
                    getNextBatch = searchResults.GetNextSetAsync();
                    var temp = await getNextBatch;
                    docList.AddRange(temp);
                }
                catch (Exception ex)
                {                    
                    return docList;
                }

            } while (!searchResults.IsDone);

            return docList;
        }
        public async Task<Document> GetItemByIdAsync(string id = "", string data = "")
        {
            return await _darkSideTable.GetItemAsync(new Primitive(id, false), new Primitive(data, false));
        }

        public async Task PutItemAsync(dynamic message)
        {
            Document darkSideDoc = new Document();

            darkSideDoc["Id"] = message.Id.ToString();
            darkSideDoc["Data"] = message.Data.ToString();
            darkSideDoc["LastUpdatedDate"] = message.PublishedDate.ToString();
            darkSideDoc["Status"] = "ReadyForDarkSide";

            await _darkSideTable.PutItemAsync(darkSideDoc);
        }

        public async Task UpdateItemAsync(Document doc)
        {
            await _darkSideTable.UpdateItemAsync(doc);
        }



    }
}
