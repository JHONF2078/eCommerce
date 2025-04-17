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
            //TO DO: Add Data Access Layer services into the IoC container
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //! significa que esperamos que no retorne null
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!);
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
