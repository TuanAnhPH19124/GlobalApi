using GlobalApi.DataTransfer;
using GlobalApi.IRepositories;
using GlobalApi.Models;
using GlobalApi.Services;
using GlobalApi.Ultilities;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GlobalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ICacheService _cacheService;
        private readonly IProductRepository _productRepository;

        public ProductsController(ILogger<ProductsController> logger, ICacheService cacheService, IProductRepository productRepository)
        {
            _logger = logger;
            _cacheService = cacheService;
            _productRepository = productRepository;
        }


        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("products");

            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = _productRepository.SelectAll();
            foreach (var item in cacheData)
            {
                item.Photo = GetImages(item.Id).FirstOrDefault();
            }
            //set expiry time
            var expiryTime = DateTimeOffset.Now.AddSeconds(60);
            _cacheService.SetData<IEnumerable<Product>>("products", cacheData, expiryTime);
            return Ok(cacheData);
        }

        [HttpGet("get/{id}")]
        public IActionResult GetById(string id)
        {
            var cacheData = _cacheService.GetData<Product>($"products:{id}");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }

            cacheData = _productRepository.SelectById(id);
            cacheData.Photo = GetImages(cacheData.Id).FirstOrDefault();
            var expiryTime = DateTimeOffset.Now.AddSeconds(60);
            _cacheService.SetData<Product>($"products:{id}", cacheData, expiryTime);
            return Ok(cacheData);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("insert")]
        public IActionResult Insert([FromForm] InsertProductRequestDto productDto)
        {
            var newProduct = productDto.Adapt<Product>();

            if (productDto.Image != null)
            {
                UploadService.UploadImages(productDto.Image, newProduct.Id);
                newProduct.Photo = GetImages(newProduct.Id).FirstOrDefault();
            }
            var addedObj = _productRepository.Insert(newProduct);

            var expiryTime = DateTimeOffset.Now.AddSeconds(60);
            _cacheService.SetData<Product>($"products{newProduct.Id}", newProduct, expiryTime);
            return Ok(newProduct);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var exist = _productRepository.SelectById(id);
            if (exist != null)
            {
                _productRepository.Remove(id);
                _cacheService.RemoveData("products");

                return NoContent();
            }
            return NotFound();
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(string id, [FromForm] UpdateProductRequestDto product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var jsConvertData = System.Text.Json.JsonSerializer.Deserialize<Product>(product.Housing, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    _productRepository.Update(jsConvertData!);
                    return Ok();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return BadRequest(product);

        }

        [NonAction]
        private IEnumerable<string> GetImages(string Id)
        {
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", Id);
            if (Directory.Exists(uploadFolder))
            {
                var imageFiles = Directory.GetFiles(uploadFolder);
                var imageUrls = imageFiles.Select(file =>
                {
                    var imageName = Path.GetFileName(file);
                    return Url.Link("GetImage", new { id = Id, imageName = imageName });
                });
                return imageUrls;
            }
            return new List<string>(){"https://loremflickr.com/640/480/business"};
        }
    }
}
