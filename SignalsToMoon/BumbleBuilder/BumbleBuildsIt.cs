using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Communications.Core.Models;
using InfrastructureForMoon.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RepositoriesForMoon.Interfaces;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BumbleBuilder
{
    public class BumbleBuildsIt
    {

        private ServiceCollection _serviceCollection;
        private IDynamoClient _dynamoClient;
        private ISQSClient _sqsClient;
        private IDarkSideRepository _darkSideRepository;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public BumbleBuildsIt()
        {
            //This is done here to avoid dispose of the services after the lambda is done
            _serviceCollection = ServiceConfigurations.ConfigureServices();
        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SNS event object and can be used 
        /// to respond to SNS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SNSEvent evnt, ILambdaContext context)
        {
            try //This was aded to understand the actual failure otherwise it was spitting out global handler errors
            {
                //With this pattern each lambda invocation will get a new ServiceProvider and dispose of it when finished.
                using (ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider())
                {
                    // entry to run app.
                    _dynamoClient = serviceProvider.GetService<IDynamoClient>();
                    _sqsClient = serviceProvider.GetService<ISQSClient>();
                    _darkSideRepository = serviceProvider.GetService<IDarkSideRepository>();

                    //Both Operation 1 & Operation 2 can be processed in parallel

                    //Operation 1 : Check for failures in SQS
                        //1. Pull Messages from SQS
                        //2. Process Them and shove them in dynamo
                 

                    //Operation 2 : Check for the SNS notifications                    
                    foreach (var record in evnt.Records)
                    {
                        await ProcessRecordAsync(record, context);
                    }
                }
            }catch(Exception e)
            {
                context.Logger.Log($"Error: {e.Message}");
                context.Logger.Log(e.StackTrace);            
            }
        }
        private async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
        {

            var snsMessage = JsonSerializer.Deserialize(record.Sns.Message, typeof(SNSMessage)) as SNSMessage;

            context.Logger.LogLine($"Custom Message Id : {snsMessage.Id}");
            context.Logger.LogLine($"Who sent the message : {snsMessage.Tag}");
            context.Logger.LogLine($"Bumble read the message from earth as: {JsonSerializer.Serialize(snsMessage.Data)}");

            //Shove it in Dynamo
            await _darkSideRepository.PutItemAsync(snsMessage);
            context.Logger.LogLine($"Shoved it in Dynamo");

            // TODO: Do interesting work based on the new message
            await Task.CompletedTask;
        }
    }
}
