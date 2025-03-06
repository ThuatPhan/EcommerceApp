using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Presentation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ILogger<ProductVariantController> _logger;

        public ProductVariantController(IProductVariantService productVariantService, ILogger<ProductVariantController> logger)
        {
            _productVariantService = productVariantService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductVariantDTO>>> AddProductVariant([FromForm] ProductVariantRequestDTO requestDTO)
        {
            try
            {
                var result = await _productVariantService.AddProductVariantAsync(requestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<ProductVariantDTO>.Failure(result.Message));
                }
                return CreatedAtAction(nameof(GetProductVariant), new { result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating product variant");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductVariantDTO>>> GetProductVariant([FromRoute] int id)
        {
            try
            {
                var result = await _productVariantService.GetProductVariantByIdAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<ProductVariantDTO>.Failure(result.Message));
                }
                return Ok(ApiResponse<ProductVariantDTO>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving product variant");
            }
        }

    }
}
