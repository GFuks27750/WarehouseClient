using Microsoft.AspNetCore.Mvc;
using WarehouseClient.Services;

namespace WarehouseClient.Controllers;
[ApiController]
[Route("api/[controller]")]
public class WarehouseController(IDbService db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProduct(int id)
    {
        var result = await db.GetProductDTOAsync(id);
        if (result is null) return NotFound($"Product with id: {id} does not exist");
        return Ok(result);
    }
}