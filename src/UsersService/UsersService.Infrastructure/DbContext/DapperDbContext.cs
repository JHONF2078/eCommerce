using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace UsersService.Infrastructure.DbContext
{
    public  class DapperDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _connection;
        public DapperDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            string? connectionString = _configuration.GetConnectionString("PostgresConnection");
            // Obtener la contraseña desde variable de entorno
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("La variable de entorno 'DB_PASSWORD' no está definida.");
            }

            // Construir la cadena completa de forma segura
            var connectionBuilder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Password = password
            };


            //create a new NpgsqlConnection with  the retrieved connection string
            _connection = new NpgsqlConnection(connectionBuilder.ConnectionString);
        }

        //get property with lambda expression
        public IDbConnection DbConnection => _connection;       

    }
}
