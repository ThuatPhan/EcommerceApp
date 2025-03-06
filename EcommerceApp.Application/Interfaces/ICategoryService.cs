using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;

namespace EcommerceApp.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<CategoryDTO>> AddCategoryAsync(CategoryRequestDTO requestDTO);
        Task<Result<CategoryDTO>> GetByIdAsync(int id);
        Task<Result<List<CategoryDTO>>> GetAllCategoriesAsync();
    }
}
