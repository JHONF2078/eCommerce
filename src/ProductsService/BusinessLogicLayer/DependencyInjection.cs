using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.BusinessLogicLayer.Mappers;
using ProductsService.BusinessLogicLayer.RabbitMQ;
using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.BusinessLogicLayer.Services;
using ProductsService.BusinessLogicLayer.Validators;

namespace ProductsService.BusinessLogicLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            //TO DO: Add Business Logic Layer services into the IoC container

            //! Add AutoMapper to the IoC container
            //! We are using the assembly of the ProductToProductResponseMappingProfile class to scan for all mapping profiles
            //then AutoMapper will automatically register all profiles in the assembly
            services.AddAutoMapper(typeof(ProductToProductResponseMappingProfile).Assembly);

            services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();

            //services.AddScoped(typeof(IGenericService<,,,>), typeof(GenericService<,,,>));
          
            services.AddScoped<IProductService, ProductService>();


            services.AddTransient<IRabbitMQPublisher, RabbitMQPublisher>();

            return services;
        }
    }
}