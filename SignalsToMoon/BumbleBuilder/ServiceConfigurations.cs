using InfrastructureForMoon.Clients;
using InfrastructureForMoon.Factories;
using InfrastructureForMoon.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RepositoriesForMoon;
using RepositoriesForMoon.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BumbleBuilder
{
    public static class ServiceConfigurations
    {
        public static ServiceCollection ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<DynamoDBClientFactory>();
            serviceCollection.AddTransient<IDynamoClient, DynamoClient>();
            serviceCollection.AddTransient<SQSClientFactory>();
            serviceCollection.AddTransient<IDarkSideRepository, DarkSideRepository>();       

            return serviceCollection;
        }
    }
}
