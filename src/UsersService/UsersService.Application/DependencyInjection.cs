using UsersService.Application.DTO;
using UsersService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Interfaces.Services;
using FluentValidation;
using UsersService.Application.Validators;

namespace UsersService.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Extension method to add application services to the dependency injection container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //TO DO: Add services to the IoC container
            //Application services often include data access, caching and other low-level components.
            services.AddTransient<IUserService, UserService>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            return services;
        }
    }
}
