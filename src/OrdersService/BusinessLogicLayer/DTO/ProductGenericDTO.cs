namespace OrdersService.BusinessLogicLayer.DTO;

public record ProductGenericDTO(Guid Id, string? ProductName, string? Category, double UnitPrice, int QuantityInStock);
