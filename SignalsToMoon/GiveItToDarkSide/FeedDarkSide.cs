
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.SQS;
using InfrastructureForMoon.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RepositoriesForMoon.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GiveItToDarkSide
{
    public class FeedDarkSide
    {
        private ServiceCollection _serviceCollection;
        private IDynamoClient _dynamoClient;
       
        private IDarkSideRepository _darkSideRepository;

        public FeedDarkSide()
        {
            _serviceCollection = ServiceConfigurations.ConfigureServices();
        }

        public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            context.Logger.LogLine("Processing1 Data For DarkSide.");
            
            try
            {
                //With this pattern each lambda invocation will get a new ServiceProvider and dispose of it when finished.
                using (ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider())
                {
                    _dynamoClient = serviceProvider.GetService<IDynamoClient>();                
                    _darkSideRepository = serviceProvider.GetService<IDarkSideRepository>();


                    ScanFilter filter = new ScanFilter();
                    filter.AddCondition("Status", ScanOperator.Equal, new DynamoDBEntry[] { "ReadyForDarkSide" });
                    ScanOperationConfig config = new ScanOperationConfig
                    {
                        Filter = filter
                    };

                    //Something to use if you want to query it
                    //QueryOperationConfig config = new QueryOperationConfig();
                    //config.Filter = new QueryFilter();
                    //config.Filter.AddCondition("Id", QueryOperator.BeginsWith, new DynamoDBEntry[] { "" });
                    //config.Filter.AddCondition("Status", QueryOperator.Equal, new DynamoDBEntry[] { "ReadyForDarkSide" });
                   // config.AttributesToGet = new List<string> { "Id", "Status", "Data" };

                    var dataToProcess = await _darkSideRepository.ScanItemsAsync(config);
                    context.Logger.LogLine($"Document Count {dataToProcess.Count}");

                    foreach (var doc in dataToProcess)
                    {
                        context.Logger.LogLine($"Consumed: {doc["Id"]}, {doc["Data"]}");

                        //Update the record in Dynamo
                        doc["Status"] = "ConsumedByDarkSide";
                        doc["LastUpdatedDate"] = DateTime.UtcNow.ToString();

                        await _darkSideRepository.UpdateItemAsync(doc);
                    }

                    //Write processed data Into S3



                }
            }catch(Exception e)
            {
                Console.WriteLine(e);
                context.Logger.Log($"Error: {e.Message}");
                context.Logger.Log(e.StackTrace);
            }

        }
    }
}