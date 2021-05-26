

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Amazon.Runtime;
using InfrastructureForMoon.Models;

namespace InfrastructureForMoon.Factories
{
    public class SQSClientFactory
    {
        public static AmazonSQSClient CreateClient(AppConfig appConfig)
        {
            var dynamoDbConfig = new AmazonSQSConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(appConfig.AwsRegion)
            };

            var awsCredentials = new InfrastructureForMoon.Creds.AwsCredentials(appConfig);

            return new AmazonSQSClient(awsCredentials, dynamoDbConfig);
        }
    }
}
