using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost("Add-item")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CartItemDTO>>> AddCartItem([FromBody] CartItemRequestDTO requestDTO)
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _cartService.AddCartItemAsync(userId, requestDTO);
                return Created("", ApiResponse<CartItemDTO>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when adding cart item");
            }
        }

        [HttpPut("Update-item-quantity")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CartItemDTO>>> UpdateCartItemQuantity([FromBody] CartItemRequestDTO requestDTO)
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _cartService.UpdateCartItemQuantityAsync(userId, requestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<CartItemDTO>.Failure(result.Message));
                }
                return Ok(ApiResponse<CartItemDTO>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when updating cart item");
            }
        }

        [HttpGet("Count-item")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> GetCartItemCount()
        {
            try
            {
                var result = await _cartService.GetCartItemCountAsync(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                return Ok(ApiResponse<int>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving cart item count");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<CartItemDTO>>>> GetCartOfUser()
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _cartService.GetCartOfUserAsync(userId);
                return Ok(ApiResponse<List<CartItemDTO>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving cart");
            }
        }

        [HttpDelete("Delete-item")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCartItem([FromQuery] int productId, int? variantId)
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _cartService.DeleteCartItemAsync(userId, productId, variantId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.Failure(result.Message));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when deleting cart item");
            }
        }
    }
}
