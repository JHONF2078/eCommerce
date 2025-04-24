using ProductsService.BusinessLogicLayer.DTO;
using ProductsService.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.BusinessLogicLayer.ServiceContracts
{
    public interface IProductService
    {
        /// <summary>
        /// retrieves the list of entities from the entities repository
        /// </summary>
        /// <returns>Return list of entitiesResponse Objects</returns>
        Task<List<ProductResponse?>> GetAllAsync();

        /// <summary>
        /// retrieves the list of entities from the entites repository
        /// </summary>
        /// <param name="conditionExpresion"></param>
        /// <returns>Return matching ebtities</returns>
        Task<List<ProductResponse?>> GetAllByCondition(Expression<Func<Product, bool>> conditionExpresion);

        /// <summary>
        /// returns a single entiry that matches whit given  condition
        /// </summary>
        /// <param name="conditionExpresion">Expression that represents the condition to check</param>
        /// <returns>Return matching entity or null</returns>
        Task<ProductResponse?> GetSingleByCondition(Expression<Func<Product, bool>> conditionExpresion);

        /// <summary>
        /// Adds (inserts product into the table using entites repository)
        /// </summary>
        /// <param name="entityRequest">entity to insert</param>
        /// <returns>Products after inserting or null if unsuccessful</returns>
        Task<ProductResponse?> AddAsync(ProductAddRequest entityRequest);

        /// <summary>
        /// Updates the existing entity bases on the ProductId
        /// </summary>
        /// <param name="entityRequest">entiry  data to  update</param>
        /// <returns>Retirns product object after sucessful; otherwise null</returns>
        Task<ProductResponse> UpdateAsync(ProductUpdateRequest entityRequest);

        /// <summary>
        /// Deletes an existing entity based on product id
        /// </summary>
        /// <param name="id">entity id to searc and delete</param>
        /// <returns>Returns true if the deletion is sucessful; utherwise false</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
