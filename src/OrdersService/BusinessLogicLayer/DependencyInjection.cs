using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersService.BusinessLogicLayer.Mappers;
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

            return services;
        }

    }
}
