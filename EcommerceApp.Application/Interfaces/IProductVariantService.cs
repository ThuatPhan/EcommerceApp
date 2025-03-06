using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;

namespace EcommerceApp.Application.Interfaces
{
    public interface IProductVariantService
    {
        Task<Result<ProductVariantDTO>> AddProductVariantAsync(ProductVariantRequestDTO requestDTO);
        Task<Result<ProductVariantDTO>> GetProductVariantByIdAsync(int id);
    }
}
