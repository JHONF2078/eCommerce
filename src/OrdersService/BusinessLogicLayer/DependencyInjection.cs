using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersService.BusinessLogicLayer.Mappers;
using OrdersService.BusinessLogicLayer.RabbitMQ;
using OrdersService.BusinessLogicLayer.ServiceContracts;
using OrdersService.BusinessLogicLayer.Services;
using OrdersService.BusinessLogicLayer.Validators;

namespace OrdersService.BusinessLogicLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {

            //TO DO: Add business logic layer services into the IoC container
            services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();

            services.AddAutoMapper(typeof(OrderAddRequestToOrderMappingProfile).Assembly);

            services.AddScoped<IOrdersService, OrderssService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{configuration["REDIS_HOST"]}:{configuration["REDIS_PORT"]}";
            });

            services.AddTransient<IRabbitMQProductNameUpdateConsumer, RabbitMQProductNameUpdateConsumer>();
            services.AddTransient<IRabbitMQProductDeletionConsumer, RabbitMQProductDeletionConsumer>();

            //Hosted service
            services.AddHostedService<RabbitMQProductNameUpdateHostedService>();
            services.AddHostedService<RabbitMQProductDeletionHostedService>();

            return services;
        }

    }
}
