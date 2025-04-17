using ProductsService.DataAccessLayer.RepositoryContracts;
using ProductsService.DataAccessLayer.Context;
using ProductsService.DataAccessLayer.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ProductsService.DataAccessLayer.Repositories
{
    //where T : class, IEntity  significa que Solo voy a permitir que T sea una clase que implemente la interfaz IEntity
    /// <summary>
    /// Generic repository for CRUD operations on entities of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a product asynchronously to the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> AddAsync(T entity)
        {
            // _dbContext.Set<T>() Dame el DbSet correspondiente al tipo T, es como decir _dbContext.Products
            //.Add(entity) marca la entidad como “para insertar” en el contexto.
            //Aún no se guarda en la base de datos. Solo se registra el cambio en memoria.
            _dbContext.Set<T>().Add(entity);
            //se ejecuta el INSERT en la base de datos
            //Revisa todos los cambios hechos en el contexto (Add, Update, Remove) y los sincroniza con la base de datos.
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Delete a product asynchronously from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingEntity = await SearchAsync(id);

            if (existingEntity == null)
            {
                return false;
            }

            _dbContext.Set<T>().Remove(existingEntity);
            int affectedRowsCount = await _dbContext.SaveChangesAsync();

            return affectedRowsCount > 0;
        }

        /// <summary>
        /// Get a product by condition asynchronously from the database.
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        public async Task<T?> GetSingleByConditionAsync(Expression<Func<T, bool>> conditionExpression)
        {
            var entity = await _dbContext.Set<T>().FirstOrDefaultAsync(conditionExpression);
            return entity;
        }

        /// <summary>
        /// Get all products  asynchronously from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await _dbContext.Set<T>().ToListAsync();
            return entities;
        }

        public Task<List<T>> GetAllAsync2()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get products by condition asynchronously from the database.
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllByConditionsAsync(Expression<Func<T, bool>> conditionExpression)
        {
            var entities = await _dbContext.Set<T>().Where(conditionExpression).ToListAsync();
            return entities;
        }

        //usamos T? (nullable genérico) para que el compilador sepa que puede ser null
        /// <summary>
        /// Search for a product by ID asynchronously in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<T?> SearchAsync(Guid id)
        {
            // Obtenemos el nombre de la clave primaria usando los metadatos del modelo de EF Core
            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            var pkProperty = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault();

            if (pkProperty == null)
                throw new InvalidOperationException($"No se pudo encontrar la clave primaria de la entidad {typeof(T).Name}");

            var pkName = pkProperty.Name;

            // Usamos EF.Property para construir dinámicamente la condición de búsqueda
            var entity = await _dbContext.Set<T>().FirstOrDefaultAsync(e =>
                EF.Property<Guid>(e, pkName).Equals(id));

            return entity;
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            // Obtenemos el nombre de la clave primaria
            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            var pkProperty = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault();

            if (pkProperty == null)
                throw new InvalidOperationException($"No se pudo encontrar la clave primaria de la entidad {typeof(T).Name}");

            var pkName = pkProperty.Name;
            var propertyInfo = typeof(T).GetProperty(pkName);

            if (propertyInfo == null)
                throw new InvalidOperationException($"La propiedad {pkName} no existe en {typeof(T).Name}");

            // Obtenemos el valor de la clave primaria desde la entidad pasada
            var idValue = propertyInfo.GetValue(entity);
            if (idValue == null)
                throw new InvalidOperationException("El valor de la clave primaria no puede ser null");

            // Buscamos la entidad original en la base de datos
            var existingEntity = await SearchAsync((Guid)idValue);

            if (existingEntity == null)
            {
                return null;
            }

            //asi como esta modifica todas las propiedades
            //Si entity no tiene todos los datos completos (por ejemplo, viene de un formulario parcial),
            //podrías terminar sobreescribiendo con valores por defecto (null, 0, etc.).
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

            await _dbContext.SaveChangesAsync();

            return existingEntity;
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            var pkProperty = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault();

            if (pkProperty == null)
                throw new InvalidOperationException($"No se pudo encontrar la clave primaria de la entidad {typeof(T).Name}");

            var pkName = pkProperty.Name;

            // Construye una consulta dinámica usando EF.Property
            return await _dbContext.Set<T>().FirstOrDefaultAsync(e =>
                EF.Property<object>(e, pkName).Equals(id));
        }
    }
}
