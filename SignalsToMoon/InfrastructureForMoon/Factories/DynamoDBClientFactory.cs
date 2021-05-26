

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using InfrastructureForMoon.Models;

namespace InfrastructureForMoon.Factories
{
    public class DynamoDBClientFactory
    {
        public static AmazonDynamoDBClient CreateClient(AppConfig appConfig)
        {
            var dynamoDbConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(appConfig.AwsRegion)
            };

            var awsCredentials = new InfrastructureForMoon.Creds.AwsCredentials(appConfig);

            return new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);
        }
    }
}
