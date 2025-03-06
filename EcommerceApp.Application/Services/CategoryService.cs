using AutoMapper;
using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Utils;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Domain.Interfaces;

namespace EcommerceApp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IReposistory<Category> _categoryRepo;
        private readonly IMapper _mapper;
        private readonly IS3Service _s3Service;

        public CategoryService(IReposistory<Category> categoryRepo, IMapper mapper, IS3Service s3Service)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _s3Service = s3Service;
        }

        public async Task<Result<CategoryDTO>> AddCategoryAsync(CategoryRequestDTO requestDTO)
        {
            var newCategory = _mapper.Map<Category>(requestDTO);

            if (requestDTO.IconFile == null)
            {
                return Result<CategoryDTO>.Failure("Icon file is required");
            }

            var uploadIconResult = await _s3Service.UploadFileAsync(requestDTO.IconFile);
            string iconPath = uploadIconResult.IsSuccess ? uploadIconResult.Data : Constants.IconAlternativePath;
            newCategory.Icon = iconPath;

            var addedCategory = await _categoryRepo.AddAsync(newCategory);

            return Result<CategoryDTO>.Success(
                _mapper.Map<CategoryDTO>(addedCategory),
                "Category added successfully"
            );
        }

        public async Task<Result<CategoryDTO>> GetByIdAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            if (category is null)
            {
                return Result<CategoryDTO>.Failure($"Category Id {id} not exist");
            }

            return Result<CategoryDTO>.Success(
                _mapper.Map<CategoryDTO>(category),
                "Category retrieved successfully"
            );
        }

        public async Task<Result<List<CategoryDTO>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return Result<List<CategoryDTO>>.Success(
                _mapper.Map<List<CategoryDTO>>(categories),
                "Categories retrived successfully"
            );
        }
    }
}
