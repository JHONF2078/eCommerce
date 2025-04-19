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
            if (guidEntity.Id == Guid.Empty)
            {
                guidEntity.Id = Guid.NewGuid();
            }

            string tableName = guidEntity.GetTableName();
            //Cuando usas GetProperties(), obtienes todas las propiedades públicas. Algunas de ellas podrían:
            //Ser de navegación (relaciones con otras tablas).
            //Ser virtual (EF las usa para lazy loading).
            //No tener set/get válidos.
            // public virtual ICollection<Role> Roles { get; set; } // relación (NO queremos insertar esto)
            var properties = typeof(T)
             .GetProperties()
             .Where(p => p.CanRead && p.CanWrite && !p.GetMethod.IsVirtual)
             .ToList();

            var columnNames = properties.Select(p => $"\"{p.Name}\"").ToList();     // "UserID", "Email", etc.
            var parameterNames = properties.Select(p => $"@{p.Name}").ToList();     // @UserID, @Email, etc.

            string query = $"INSERT INTO public.\"{tableName}\" ({string.Join(", ", columnNames)}) " +
                           $"VALUES ({string.Join(", ", parameterNames)})";

            int affectedRows = await _dbContext.DbConnection.ExecuteAsync(query, entity);

            return affectedRows > 0 ? entity : null;
        }

        public async Task<T?> GetByIdAsync(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;

            // Verificar que la entidad implemente IGenericEntity<Guid>
            if (Activator.CreateInstance<T>() is not IGenericEntity<Guid> tempEntity)
                throw new InvalidOperationException("T must implement IGenericEntity<Guid>");

            string tableName = tempEntity.GetTableName();

            // Buscar propiedad real que representa la clave primaria (la que contiene el valor actual de Id)
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p =>
                    p.CanRead &&
                    p.PropertyType == typeof(Guid) &&
                    p.GetValue(tempEntity)?.Equals(tempEntity.Id) == true
                );

            if (keyProperty == null)
                throw new InvalidOperationException($"No se encontró propiedad de clave primaria para la entidad {typeof(T).Name}");

            string keyColumn = keyProperty.Name;

            string query = $"SELECT * FROM public.\"{tableName}\" WHERE \"{keyColumn}\" = @Id";

            var result = await _dbContext.DbConnection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });

            return result;
        }
    }
}
