using ProductsService.BusinessLogicLayer.ServiceContracts;

namespace ProductsService.BusinessLogicLayer.DTO
{
    public record ProductUpdateRequest(
        Guid ProductID,
        string ProductName,
        CategoryOptions Category,
        double? UnitPrice, 
        int? QuantityInStock
       ) : IRequestWithId<Guid>
    {
        public Guid Id => ProductID;
        public ProductUpdateRequest() : this(default, default, default, default, default)
        {
        }
    }
}