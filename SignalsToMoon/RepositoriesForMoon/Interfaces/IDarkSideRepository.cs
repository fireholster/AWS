using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesForMoon.Interfaces
{
    public interface IDarkSideRepository
    {
        Task PutItemAsync(dynamic message);
        Task<Document> GetItemByIdAsync(string id = "", string data = "");

        Task<List<Document>> ScanItemsAsync(ScanOperationConfig config);

        Task UpdateItemAsync(Document doc);
    }
}
