using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureForMoon.Interfaces
{
    public interface ISQSClient
    {
        void GetMessages();
        
    }
}
