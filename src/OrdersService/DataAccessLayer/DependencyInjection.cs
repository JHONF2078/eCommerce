using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrdersService.DataAccessLayer.RepositoryContracts;
using OrdersMicroservie.DataAccessLayer.Repositories;

namespace OrdersService.DataAccessLayer
{
    public static  class DependencyInjection
    {

        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
           
            //TO DO Add data access layer  services into the Ioc container 
           string connectionStringTemplate = configuration.GetConnectionString("MongoDB")!;

            string connectionString = connectionStringTemplate
                .Replace("$MONGODB_HOST", Environment.GetEnvironmentVariable("MONGODB_HOST"))
                .Replace("$MONGODB_PORT", Environment.GetEnvironmentVariable("MONGODB_PORT"));
          
            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

            //SP service provider
            services.AddScoped<IMongoDatabase>(sp =>
            {              
                IMongoClient client = sp.GetRequiredService<IMongoClient>();               
                return client.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_DATABASE"));
            });

            services.AddScoped<IOrdersRepository, OrdersRepository>();

            return services;
        }
    }
}
