using EcommerceApp.Application.DTO.Responses;

namespace EcommerceApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<FavouriteProductDTO>> AddFavouriteProductAsync(string userId, int productId);
        Task<Result<List<FavouriteProductDTO>>> GetFavouriteProductsAsync(string userId);
    }
}
