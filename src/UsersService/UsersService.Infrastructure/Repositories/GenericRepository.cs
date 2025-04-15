using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Security.Principal;
using Dapper;
using UsersService.Application.Interfaces.Repository;
using UsersService.Domain.Entities;
using UsersService.Infrastructure.DbContext;

namespace UsersService.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DapperDbContext _dbContext;

        public GenericRepository(DapperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> AddAsync(T entity)
        {
            if (entity is not IGenericEntity<Guid> guidEntity)
                throw new InvalidOperationException("Entity must implement IGenericEntity<Guid>");

            // Asignar ID si es vacío
            if (guidEntity.GetId() == Guid.Empty)
            {
                guidEntity.SetId(Guid.NewGuid());
            }

            string tableName = guidEntity.GetTableName();
            var properties = typeof(T).GetProperties();

            var columnNames = properties.Select(p => $"\"{p.Name}\"").ToList();     // "UserID", "Email", etc.
            var parameterNames = properties.Select(p => $"@{p.Name}").ToList();     // @UserID, @Email, etc.

            string query = $"INSERT INTO public.\"{tableName}\" ({string.Join(", ", columnNames)}) " +
                           $"VALUES ({string.Join(", ", parameterNames)})";

            int affectedRows = await _dbContext.DbConnection.ExecuteAsync(query, entity);

            return affectedRows > 0 ? entity : null;
        }
        
    }
}
