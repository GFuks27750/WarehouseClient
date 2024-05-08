using System.ComponentModel.DataAnnotations;
using Warehouse.Models;

namespace Warehouse.DTOs
{
    public record ProductWarehouseDTO(int IdProductWarehouse, int IdWarehouse, int IdProduct, int IdOrder, int Amount, double Price, DateTime CreatedAt)
    {
        public ProductWarehouseDTO(ProductWarehouse productWarehouse) : this(productWarehouse.IdProductWarehouse,
            productWarehouse.IdWarehouse,
            productWarehouse.IdProduct, productWarehouse.IdOrder, productWarehouse.Amount, productWarehouse.Price,
            productWarehouse.CreatedAt)
        {
            
        }
        
    }

    public record CreateProductWarehouseRequest(
        [Required] int IdWarehouse,
        [Required] int IdProduct,
        [Required] int Amount,
        [Required] DateTime CreatedAt
    );

    public record CreateProductWarehouseResponse(int IdProductWarehouse, int IdWarehouse, int IdProduct, int IdOrder, int Amount, Decimal Price, DateTime CreatedAt)
    {
        public CreateProductWarehouseResponse(int IdProductWarehouse, int IdOrder, Decimal Price, CreateProductWarehouseRequest request):this(IdProductWarehouse, 
            request.IdWarehouse, request.IdProduct, IdOrder, request.Amount, Price, request.CreatedAt) {}
    };
}