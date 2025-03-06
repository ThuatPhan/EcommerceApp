using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;

namespace EcommerceApp.Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<CartItemDTO>> AddCartItemAsync(string userId, CartItemRequestDTO requestDTO);
        Task<Result<List<CartItemDTO>>> GetCartOfUserAsync(string userId);
        Task<Result<int>> GetCartItemCountAsync(string userId);
        Task<Result<CartItemDTO>> UpdateCartItemQuantityAsync(string userId, CartItemRequestDTO requestDTO);
        Task<Result<bool>> DeleteCartItemAsync(string userId, int productId, int? variantId);
    }
}
