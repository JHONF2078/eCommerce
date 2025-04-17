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
    /// <typeparam name="TTEntity"></typeparam>
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : BaseEntity<TId>
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
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            // _dbContext.Set<T>() Dame el DbSet correspondiente al tipo T, es como decir _dbContext.Products
            //.Add(entity) marca la entidad como “para insertar” en el contexto.
            //Aún no se guarda en la base de datos. Solo se registra el cambio en memoria.
            _dbContext.Set<TEntity>().Add(entity);
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
        public async Task<bool> DeleteAsync(TId id)
        {
            //TEntity? existingEntity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id.Equals(id));
            TEntity? existingEntity = await GetByIdAsync(id);

            if (existingEntity == null)
            {
                return false;
            }

            _dbContext.Set<TEntity>().Remove(existingEntity);
            int affectedRowsCount = await _dbContext.SaveChangesAsync();

            return affectedRowsCount > 0;
        }

        /// <summary>
        /// Get a single entity that matches the given condition.  
        /// Úsalo para filtros distintos a la PK o cuando necesites Includes.
        /// </summary>
        public async Task<TEntity?> GetSingleByConditionAsync(
                Expression<Func<TEntity, bool>> conditionExpression,
                bool asNoTracking = true)
        {
            //LINQ a EF Core
            //Es una extensión LINQ
            //para llamar este metodo GetSingleByConditionAsync, tener las recomendaciones de este comentaria, la llamada se haria asi:
            //TEntity? existingEntity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id.Equals(entity.Id));
            //Si el método recibe una expresión (x => …) es LINQ.
            //Queryable<T>; acepta una expresión(Expression<Func<T, bool>>).El proveedor EF Core traduce la expresión a SQL.
            //cuando usar Necesitas Include o proyección (Select) junto con la búsqueda.
            //El filtro no es la PK sino otra columna (por ejemplo, Email, Slug, etc.).
            //Trabajas en consultas compuestas (ordenamientos, paginación, etc.).

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (asNoTracking)
                query = query.AsNoTracking();   // evita sobrecarga del Change Tracker en lecturas

           
            return await query.FirstOrDefaultAsync(conditionExpression);
        }

        /// <summary>
        /// Get all products  asynchronously from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var entities = await _dbContext.Set<TEntity>().ToListAsync();
            return entities;
        }

        public Task<List<TEntity>> GetAllAsync2()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get products by condition asynchronously from the database.
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetAllByConditionsAsync(Expression<Func<TEntity, bool>> conditionExpression)
        {
            var entities = await _dbContext.Set<TEntity>().Where(conditionExpression).ToListAsync();
            return entities;
        }
       

        public async Task<TEntity?> UpdateAsync(TEntity entity)
        {
            //SE PODDRIA USAR PERO ES MAS RECOMENDADO GetByIdAsync, por el tema de la caché
            //TEntity? existingEntity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id.Equals(entity.Id));

            TEntity? existingEntity = await GetByIdAsync(entity.Id);

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


        /// <summary>
        /// Get an entity by its primary key asynchronously (usa la caché del Change Tracker).
        /// </summary>
        public async Task<TEntity?> GetByIdAsync(TId id)
        {
            //No es linq. 
            //Si recibe valores concretos de clave y no usa expresión, no es LINQ; es un helper de EF Core.
            //Es un método propio de DbSet (no acepta expresión). Se especializa en recuperar por clave primaria.
            //FindAsync localiza la entidad por la clave primaria configurada en el modelo, sin que tengas que
            //saber cómo se llama la columna en la base de datos.
            //Si la entidad ya está siendo rastreada, FindAsync la devuelve de la caché y no dispara consulta SQL;
            //de lo contrario ejecuta un SELECT … WHERE[PK] = @id.
            //Si ya tienes el valor de la clave primaria y solo necesitas esa fila, usa FindAsync
            // Si la entidad ya está siendo rastreada, FindAsync la devuelve de caché
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }
    }
}
