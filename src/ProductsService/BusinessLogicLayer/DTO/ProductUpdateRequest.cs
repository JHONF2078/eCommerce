using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.DataAccessLayer.EntitiesContracts;

namespace ProductsService.BusinessLogicLayer.DTO
{
    public record ProductUpdateRequest(
        Guid ProductID,
        string ProductName,
        CategoryOptions Category,
        double? UnitPrice, 
        int? QuantityInStock
       ) : IEntity<Guid>
    {
        public Guid Id => ProductID;
        public ProductUpdateRequest() : this(default, default, default, default, default)
        {
        }
    }
}