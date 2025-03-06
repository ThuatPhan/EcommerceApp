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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("Favourite-product")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<FavouriteProductDTO>>>> GetFavouriteProducts()
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _userService.GetFavouriteProductsAsync(userId);
                return Ok(ApiResponse<List<FavouriteProductDTO>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving favourite products");
            }
        }

        [HttpPost("Favourite-product")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<FavouriteProductDTO>>> AddFavouriteProducts([FromBody] FavouriteProductRequestDTO requestDTO)
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _userService.AddFavouriteProductAsync(userId, requestDTO.ProductId);
                return Ok(ApiResponse<FavouriteProductDTO>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving favourite products");
            }
        }
    }
}
