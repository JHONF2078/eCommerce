using ProductsService.DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace ProductsService.DataAccessLayer.RepositoryContracts
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Add a product asynchronously to the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the added product object or null if unsuccessful</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Delete a product asynchronously from the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns true if the deletion is successful, false otherwise.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Get a product by condition asynchronously from the database.
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns>Returns a single product or null if not found</returns>
        Task<T?> GetSingleByConditionAsync(Expression<Func<T, bool>> conditionExpression);


        // Ideal si solo necesitas recorrer los elemento, Solo necesitas leer o iterar.
        //Más flexible: no te amarras a una implementación específica.
        //Mejora la abstracción y hace que tu código sea más limpio y fácil de testear.
        /// <summary>
        /// Get all products  asynchronously from the database.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<T>> GetAllAsync();

        //Ideal si necesitas modificar, indexar o acceder a elementos de forma más dinámica.
        //Necesitas una lista completa para modificar, contar o acceder por índice.
        //Útil si sabes que el resultado será tratado como una lista.
        //productos[0] (esto da error si es IEnumerable), aqui funciona ok
        /// <summary>
        /// Get all products  asynchronously from the database as a list.
        /// </summary>
        /// <returns>Returning a collection of matching products</returns>
        public Task<List<T>> GetAllAsync2();

        /// <summary>
        /// Get products by condition asynchronously from the database.
        /// </summary>
        /// <param name="conditionExpression"></param>
        /// <returns>Returning a collection of matching products</returns>
        Task<IEnumerable<T>> GetAllByConditionsAsync(Expression<Func<T, bool>> conditionExpression);


        /// <summary>
        /// Update a product asynchronously in the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the updated product; or null if not found</returns>
        Task<T?> UpdateAsync(T entity);

        Task<T?> GetByIdAsync(object id);

    }
}
