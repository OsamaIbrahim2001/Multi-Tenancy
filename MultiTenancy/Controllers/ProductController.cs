
using MultiTenancy.Dtos;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        [HttpGet(template: "{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateProductDto dto)
        {
            Product product
                = new()
                {
                    Name = dto.Name,
                    Descriptions = dto.Descriptions,
                    Rate = dto.Rate
                };
            var createdProduct = await _productService.CreateAsync(product);
            return Ok(createdProduct);
        }
    }
}
