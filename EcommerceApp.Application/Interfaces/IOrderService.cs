using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;

namespace EcommerceApp.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderDTO>> CreateOrderAsync(string userId, DateTime createdDate, List<OrderItemRequestDTO> orderItems);
        Task<Result<OrderDTO>> GetOrderByIdAsync(int id, string userId);
        Task<Result<List<OrderDTO>>> GetOrdersAsync(string userId);
    }
}
