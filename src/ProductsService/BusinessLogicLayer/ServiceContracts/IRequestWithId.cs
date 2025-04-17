namespace ProductsService.BusinessLogicLayer.ServiceContracts
{
    public interface IRequestWithId<TId>
    {
        TId Id { get; }
    }
}
