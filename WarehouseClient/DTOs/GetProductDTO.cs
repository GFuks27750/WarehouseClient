namespace WarehouseClient.DTOs;

public record GetProductDTO(int IdProduct, string Name, string Description, decimal Price);