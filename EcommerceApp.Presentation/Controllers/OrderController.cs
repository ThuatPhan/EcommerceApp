using AutoMapper;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, IMapper mapper, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("Create-order")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<OrderDTO>>> CreateOrder([FromBody] OrderRequestDTO requestDTO)
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var createdDate = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                );
                var result = await _orderService.CreateOrderAsync(userId, createdDate, requestDTO.Items);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<OrderDTO>.Failure(result.Message));
                }

                return CreatedAtAction(nameof(GetOrder), new { result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating order");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<OrderDTO>>> GetOrder([FromRoute] int id)
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _orderService.GetOrderByIdAsync(id, userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<OrderDTO>.Failure(result.Message));
                }

                return Ok(ApiResponse<OrderDTO>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving order");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<OrderDTO>>>> GetOrders()
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _orderService.GetOrdersAsync(userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<OrderDTO>>.Failure(result.Message));
                }

                return Ok(ApiResponse<List<OrderDTO>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving orders");
            }
        }
    }
}
