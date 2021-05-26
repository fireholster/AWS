using Amazon.SQS;
using Amazon.SQS.Model;
using InfrastructureForMoon.Factories;
using InfrastructureForMoon.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureForMoon.Clients
{
    public class SQSClient : ISQSClient
    {
        private const string StatusUnknown = "UNKNOWN";
        private const string StatusActive = "ACTIVE";
        private readonly IAmazonSQS _amazonSQS;

        public SQSClient(IAmazonSQS amazonSQS)
        {
            //_amazonSQS = amazonSQS;
            _amazonSQS = SQSClientFactory.CreateClient(new Models.AppConfig()
            {
                AwsAccessKey = DoNotCheckIn.Constants.AwsAccessKey,
                AwsSecretKey = DoNotCheckIn.Constants.AwsSecretKey,
                AwsRegion = DoNotCheckIn.Constants.AwsRegion
            });
        }

        public void GetMessages()
        {
            throw new NotImplementedException();
        }

        private async Task<string> GetQueueUrl(string queueName)
        {
            //Cache It
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentException("Queue name should not be blank.");
            }

            try
            {
                var response = await _amazonSQS.GetQueueUrlAsync(queueName);
                return response.QueueUrl;
            }
            catch (QueueDoesNotExistException ex)
            {
                throw new InvalidOperationException($"Could not retrieve the URL for the queue '{queueName}' as it does not exist or you do not have access to it.", ex);
            }
        }

    }
}
