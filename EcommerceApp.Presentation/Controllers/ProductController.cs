using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> AddProduct([FromForm] ProductRequestDTO requestDTO)
        {
            try
            {
                var result = await _productService.AddProductAsync(requestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<ProductDTO>.Failure(result.Message));
                }
                return CreatedAtAction(nameof(GetProduct), new { result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating product");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> GetProduct([FromRoute] int id)
        {
            try
            {
                var result = await _productService.GetProductByIdAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<ProductDTO>.Failure(result.Message));
                }
                return Ok(ApiResponse<ProductDTO>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving product");
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDTO>>>> GetProducts(
            [FromQuery] int pageNumber, int pageSize
        )
        {
            try
            {
                var result = await _productService.GetProductsAsync(pageNumber, pageSize);
                return Ok(ApiResponse<PagedResult<ProductDTO>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving products");
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse<List<ProductDTO>>>> GetProductsOfCategory([FromRoute] int categoryId)
        {
            try
            {
                var result = await _productService.GetProductsOfCategoryAsync(categoryId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<ProductDTO>>.Failure(result.Message));
                }
                return Ok(ApiResponse<List<ProductDTO>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving products of category");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<ProductDTO>>>> SearchProducts([FromQuery] string keyword)
        {
            try
            {
                var result = await _productService.SearchProductAsync(keyword);
                return Ok(ApiResponse<List<ProductDTO>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving products of category");
            }
        }
    }
}
