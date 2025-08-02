using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Domain;
using ProductManagement.Enum;
using ProductManagement.Mappings;
using ProductManagement.Models;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IMixpanelService _mixpanelService;


        public ProductController(IProductService productService, IMapper mapper, IMixpanelService mixpanelService)
        {
            _productService = productService;
            _mapper = mapper;
            _mixpanelService = mixpanelService;
        }

        // This is a Role-based authorization attribute.
        // It allows access only to users who have the specified roles ("user" or "admin").
        // It's simple and coarse-grained — based on assigned roles only.
        //[CustomAuthorization(Roles = "user,admin")]

        // This is a Permission-based authorization attribute.
        // It provides fine-grained control by checking if the user has a specific permission ("productView").
        // It's typically used in systems where roles are mapped to a list of permissions.
        [HasPermission(PermissionsEnum.productView)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<ProductDto>> GetPageProducts([FromQuery] int limit = 0, [FromQuery] int skip = 0)
        {
            try
            {
                var products = await _productService.GetAllpageProductAsync(limit, skip);
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Ok(productDtos);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            bool success = await _mixpanelService.TrackEventAsync("Backend", product);
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        //[CustomAuthorization(Roles = "admin")]
        [HasPermission(PermissionsEnum.productCreate)]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] UpsertProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(productDto);
            var createdProduct = await _productService.CreateProductAsync(product);
            var createdProductDto = _mapper.Map<ProductDto>(createdProduct);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProductDto.Id }, createdProductDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpsertProductDto productDto)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var updatedProduct = await _productService.UpdateProductAsync(_mapper.Map(productDto, product));
            var updatedProductDto = _mapper.Map<ProductDto>(updatedProduct);

            return Ok(updatedProductDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Search(
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _productService.SearchAsync(name, minPrice, maxPrice, pageNumber, pageSize);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }
    }
}
