using Org.BouncyCastle.Crypto;
using ProductsService.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.DataAccessLayer.RepositoryContracts
{
    public interface IProductsRepository
    {
        // Ideal si solo necesitas recorrer los elemento, Solo necesitas leer o iterar.
        //Más flexible: no te amarras a una implementación específica.
        //Mejora la abstracción y hace que tu código sea más limpio y fácil de testear.
        /// <summary>
        /// Get all products  asynchronously from the database.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Product>> GetProducts();

        //Ideal si necesitas modificar, indexar o acceder a elementos de forma más dinámica.
        //Necesitas una lista completa para modificar, contar o acceder por índice.
        //Útil si sabes que el resultado será tratado como una lista.
        //productos[0] (esto da error si es IEnumerable), aqui funciona ok
        /// <summary>
        /// Get all products  asynchronously from the database as a list.
        /// </summary>
        /// <returns></returns>
        public Task<List<Product>> GetProducts2();

        Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);
    }
}
