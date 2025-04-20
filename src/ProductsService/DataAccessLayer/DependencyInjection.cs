using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.DataAccessLayer.Context;
using ProductsService.DataAccessLayer.Repositories;
using ProductsService.DataAccessLayer.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionStringTemplate = configuration.GetConnectionString("DefaultConnection")!;
            string connectionString = connectionStringTemplate
              .Replace("$MYSQL_HOST", Environment.GetEnvironmentVariable("MYSQL_HOST"))
              .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"))
              .Replace("$MYSQL_DATABASE", Environment.GetEnvironmentVariable("MYSQL_DATABASE"))
              .Replace("$MYSQL_PORT", Environment.GetEnvironmentVariable("MYSQL_PORT"))
              .Replace("$MYSQL_USER", Environment.GetEnvironmentVariable("MYSQL_USER"));

            //TO DO: Add Data Access Layer services into the IoC container
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseMySQL(connectionString);
            });

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            return services;
        }
    }
}
