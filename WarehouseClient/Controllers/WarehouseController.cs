using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Warehouse.DTOs;
using Warehouse.Services;

namespace Warehouse.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController:ControllerBase
    {
        private readonly IDbService _dbService;

        public WarehouseController(IDbService service)
        {
            _dbService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsWarehouse()
        {
            return Ok(await _dbService.GetWarehouseProducts());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductWarehouse(CreateProductWarehouseRequest request)
        {
            return Ok(await _dbService.CreateProductWarehouse(request));
        }
    }
}