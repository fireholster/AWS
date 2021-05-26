using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureForMoon.Models
{
    public class AppConfig
    {
        public string AwsRegion { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
    }
}
