using Microsoft.AspNetCore.Mvc;
using Model;
using DAL;

namespace MyProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDB _productDb;

        public ProductsController(ProductDB productDb)
        {
            _productDb = productDb;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("נתוני המוצר אינם תקינים.");
            }

            int rowsAffected = await _productDb.InsertProductAsync(product);

            if (rowsAffected == 0)
            {
                return StatusCode(500, "אירעה שגיאה בעת שמירת המוצר.");
            }

            return Ok(product);
        }
    }
}