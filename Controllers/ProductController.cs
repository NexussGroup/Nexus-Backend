using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
    public class ProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; } 
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto productDto, IFormFile? image)
        {
            Console.WriteLine($"Preço recebido: {productDto.Price}");

            if (productDto.Price.HasValue)
            {
                decimal price = decimal.Parse(productDto.Price.Value.ToString());
                productDto.Price = Math.Round(price, 2);

                Console.WriteLine($"Preço convertido e arredondado: {productDto.Price}");
            }

            if (image != null)
            {
                var imagePath = Path.Combine("uploads", $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}");

                if (!Directory.Exists("uploads"))
                {
                    Directory.CreateDirectory("uploads");
                }

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                productDto.ImageUrl = imagePath;
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price ?? 0, // Se o preço for nulo, define como 0
                Category = productDto.Category,
                ImageUrl = productDto.ImageUrl
            };

            await _productService.CreateProductAsync(product);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromForm] ProductDto updatedProduct, IFormFile? image)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (image != null)
            {
                var imagePath = Path.Combine("uploads", $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}");

                if (!Directory.Exists("uploads"))
                {
                    Directory.CreateDirectory("uploads");
                }

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                updatedProduct.ImageUrl = imagePath;
            }

            product.Name = updatedProduct.Name ?? product.Name;
            product.Description = updatedProduct.Description ?? product.Description;
            product.Price = updatedProduct.Price ?? product.Price;
            product.Category = updatedProduct.Category ?? product.Category;
            product.ImageUrl = updatedProduct.ImageUrl ?? product.ImageUrl;

            await _productService.UpdateProductAsync(id, product);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
