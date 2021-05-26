using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using InfrastructureForMoon.Factories;
using InfrastructureForMoon.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureForMoon.Clients
{
    public class DynamoClient : IDynamoClient
    {
        private const string StatusUnknown = "UNKNOWN";
        private const string StatusActive = "ACTIVE";
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private readonly DynamoDBClientFactory _clientFactory;
        
        public DynamoClient()
        {
            //_dynamoDBClient = dynamoDBClient;
            _dynamoDBClient =  DynamoDBClientFactory.CreateClient(new Models.AppConfig()
            {
                AwsAccessKey = DoNotCheckIn.Constants.AwsAccessKey,
                AwsSecretKey = DoNotCheckIn.Constants.AwsSecretKey,
                AwsRegion = DoNotCheckIn.Constants.AwsRegion
            });
        }

        public async Task CreateTableAsync(CreateTableRequest createTableRequest)
        {
            var status = await GetTableStatusAsync(createTableRequest.TableName);

            if (status != StatusUnknown)
            {
                return;
            }

            await _dynamoDBClient.CreateTableAsync(createTableRequest);

            await WaitUntilTableReady(createTableRequest.TableName);
        }

        public async Task PutItemAsync(PutItemRequest putItemRequest)
        {
            await _dynamoDBClient.PutItemAsync(putItemRequest);
        }

        public Table LoadTable(string tableName)
        {
            return Table.LoadTable(_dynamoDBClient, tableName);        
        }

        private async Task<string> GetTableStatusAsync(string tableName)
        {
            try
            {
                var response = await _dynamoDBClient.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = tableName
                });
                return response?.Table.TableStatus;
            }
            catch (ResourceNotFoundException)
            {
                return StatusUnknown;
            }
        }

        private async Task WaitUntilTableReady(string tableName)
        {
            var status = await GetTableStatusAsync(tableName);
            for (var i = 0; i < 10 && status != StatusActive; ++i)
            {
                await Task.Delay(500);
                status = await GetTableStatusAsync(tableName);
            }
        }
    }
}
