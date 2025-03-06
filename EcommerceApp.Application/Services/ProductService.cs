using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Utils;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IReposistory<Product> _productRepo;
        private readonly IReposistory<ProductVariant> _productVariantRepo;
        private readonly IReposistory<Category> _categoryRepo;
        private readonly IMapper _mapper;
        private readonly IS3Service _s3Service;

        public ProductService(
            IMapper mapper,
            IS3Service s3Service,
            IReposistory<Product> productRepo,
            IReposistory<Category> categoryRepo,
            IReposistory<ProductVariant> productVariantRepo
        )
        {
            _mapper = mapper;
            _s3Service = s3Service;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _productVariantRepo = productVariantRepo;
        }

        public async Task<Result<ProductDTO>> AddProductAsync(ProductRequestDTO requestDTO)
        {
            var newProduct = _mapper.Map<Product>(requestDTO);
            var categoryOfProduct = await _categoryRepo.GetByIdAsync(newProduct.CategoryId);

            if (categoryOfProduct is null)
            {
                return Result<ProductDTO>.Failure($"Category Id {newProduct.CategoryId} not exist");
            }
            if (requestDTO.ImageFile is null)
            {
                return Result<ProductDTO>.Failure("Image file is required");
            }

            var uploadImageResult = await _s3Service.UploadFileAsync(requestDTO.ImageFile);
            string imagePath = uploadImageResult.IsSuccess
                ? uploadImageResult.Data
                : Constants.ProductImageAlternativePath;
            newProduct.Image = imagePath;

            var addedProduct = await _productRepo.AddAsync(newProduct);

            var productDTO = await _productRepo
                .GetBaseQuery(p => p.Id == addedProduct.Id)
                .ProjectTo<ProductDTO>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<ProductDTO>.Success(productDTO, "Product added successfully");
        }

        public async Task<Result<ProductDTO>> GetProductByIdAsync(int id)
        {
            var productDTO = await _productRepo
                .GetBaseQuery(p => p.Id == id)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    VariantId = null,
                    Name = p.Name,
                    Description = p.Description,
                    Image = p.Image,
                    Price = p.Price,
                    Stock = p.Stock,
                    Category = _mapper.Map<CategoryDTO>(p.Category),
                    Variants = p.Variants.Select(v => _mapper.Map<ProductVariantDTO>(v)).ToList()

                })
                .FirstOrDefaultAsync();

            if (productDTO is null)
            {
                return Result<ProductDTO>.Failure($"Product Id {id} not exist");
            }
            return Result<ProductDTO>.Success(productDTO, "Product retrived successfully");
        }

        public async Task<Result<PagedResult<ProductDTO>>> GetProductsAsync(int pageNumber, int pageSize)
        {
            var query = GetCombinedQuery(productPredicate: p => !p.Variants.Any(), variantPredicate: _ => true)
                .OrderBy(p => p.Id)
                .ThenBy(p => p.VariantId);

            var combinedProducts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalCount = await query.CountAsync();
            var itemsFetched = (pageNumber - 1) * pageSize;
            var remainingItems = Math.Max(0, totalCount - itemsFetched - pageSize);
            await Task.Delay(1000);
            return Result<PagedResult<ProductDTO>>.Success(
                new PagedResult<ProductDTO>
                {
                    RemainingItems = remainingItems,
                    Items = combinedProducts
                },
                "Products retrieved successfully"
            );
        }

        public async Task<Result<List<ProductDTO>>> GetProductsOfCategoryAsync(int categoryId)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryId);
            if (category is null)
            {
                return Result<List<ProductDTO>>.Failure($"Category Id {categoryId} not exist");
            }

            var combinedProducts = await GetCombinedQuery(
                productPredicate: p => !p.Variants.Any() && p.CategoryId == categoryId,
                variantPredicate: v => v.Product.CategoryId == categoryId
            ).OrderBy(p => p.Id)
            .ThenBy(p => p.VariantId)
            .ToListAsync();

            return Result<List<ProductDTO>>.Success(combinedProducts, "Products retrieved successfully");
        }

        public async Task<Result<List<ProductDTO>>> SearchProductAsync(string keyword)
        {
            var normalizedKeyword = keyword.Trim().ToLower();
            var keywordParts = normalizedKeyword.Split(' ');

            var combinedProducts = await GetCombinedQuery(
                productPredicate: p => !p.Variants.Any() && keywordParts.All(part => p.Name.ToLower().Contains(part)),
                variantPredicate: v => keywordParts.All(part => v.Name.ToLower().Contains(part))
            )
            .ToListAsync();

            return Result<List<ProductDTO>>.Success(combinedProducts, "Products retrieved successfully");
        }

        private IQueryable<ProductDTO> GetCombinedQuery(
            Expression<Func<Product, bool>> productPredicate,
            Expression<Func<ProductVariant, bool>> variantPredicate
        )
        {
            return _productRepo
                .GetBaseQuery(productPredicate)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    VariantId = null,
                    Name = p.Name,
                    Description = p.Description,
                    Image = p.Image,
                    Price = p.Price,
                    Stock = p.Stock,
                    Category = new CategoryDTO
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        Icon = p.Category.Icon,
                    }
                })
                .Union(
                    _productVariantRepo.GetBaseQuery(variantPredicate)
                    .Select(v => new ProductDTO
                    {
                        Id = v.ProductId,
                        VariantId = v.Id,
                        Name = v.Name,
                        Description = v.Product.Description,
                        Image = v.Image ?? v.Product.Image,
                        Price = v.Price,
                        Stock = v.Stock,
                        Category = new CategoryDTO
                        {
                            Id = v.Product.Category.Id,
                            Name = v.Product.Category.Name,
                            Icon = v.Product.Category.Icon
                        }
                    })
                );
        }
    }

}
