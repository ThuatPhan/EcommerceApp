using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;

namespace EcommerceApp.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDTO>> AddProductAsync(ProductRequestDTO requestDTO);
        Task<Result<ProductDTO>> GetProductByIdAsync(int id);
        Task<Result<PagedResult<ProductDTO>>> GetProductsAsync(int pageNumber, int pageSize);
        Task<Result<List<ProductDTO>>> GetProductsOfCategoryAsync(int categoryId);
        Task<Result<List<ProductDTO>>> SearchProductAsync(string keyword);
    }
}
