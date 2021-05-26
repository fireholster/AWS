using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Amazon.Runtime;
using Amazon;
using System.Text.Json;
using Communications.Core.Models;

namespace OptimusPrime.Controllers
{
    [Route("api/[controller]")]
    public class PrimeController : ControllerBase
    {
        private readonly IAmazonSimpleNotificationService _sns;

        private const string Topic = "NaturalSatellites";

        public PrimeController(IAmazonSimpleNotificationService sns)
        {
            _sns = sns;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Testing 1", "Testing 2" };
        }

        [HttpGet]
        [Route("sendMessageToMoon")]
        public async Task<IActionResult> PublishMessageToSNS()
        {            

            //Build the Message
            var messageFromEarth = new SNSMessage
            {
                Id = Guid.NewGuid().ToString(),
                Tag = "Optimus",
                PublishedDate = DateTime.UtcNow,
                Data = new { status = "almost-there", currentDistanceFromMoon = $"{new Random().Next(1, 11)} km" }
            };

            //Publish a Message to SNS Topics

            //Move this to configuration or Dyanmo 
            //root: bikrant

            var client = new AmazonSimpleNotificationServiceClient(
                                        DoNotCheckIn.Constants.AwsAccessKey,
                                        DoNotCheckIn.Constants.AwsSecretKey,
                                        RegionEndpoint.USEast2);

            var topicToPublish = await client.CreateTopicAsync(Topic);

            var request = new PublishRequest
            {
                //TopicArn = "arn:aws:sns:us-east-2:446430088413:NaturalSatellites",
                TopicArn = topicToPublish.TopicArn,
                Message = JsonSerializer.Serialize(messageFromEarth) //Decide the contract
            };

            var response = await client.PublishAsync(request);

            //Couldn't get this to work with DI
            //var response = await _sns.PublishAsync(request);            

            return new ObjectResult(
                new
                {
                    response = response,
                    request = request
                });
        }
    }
}
