using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Utils;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Application.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IReposistory<ProductVariant> _productVariantRepository;
        private readonly IReposistory<Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IS3Service _s3Service;

        public ProductVariantService(
            IReposistory<ProductVariant> productVariantRepository,
            IMapper mapper,
            IS3Service s3Service,
            IReposistory<Category> categoryRepository
        )
        {
            _productVariantRepository = productVariantRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _s3Service = s3Service;
        }

        public async Task<Result<ProductVariantDTO>> AddProductVariantAsync(ProductVariantRequestDTO requestDTO)
        {
            var newProductVariant = _mapper.Map<ProductVariant>(requestDTO);

            if (requestDTO.ImageFile != null)
            {
                var uploadImageResult = await _s3Service.UploadFileAsync(requestDTO.ImageFile);
                string imagePath = uploadImageResult.IsSuccess ? uploadImageResult.Data : Constants.ProductImageAlternativePath;
                newProductVariant.Image = imagePath;
            }

            var addedProductVariant = await _productVariantRepository.AddAsync(newProductVariant);
            var productVariantDTO = await _productVariantRepository
                .GetBaseQuery(pv => pv.Id == addedProductVariant.Id)
                .ProjectTo<ProductVariantDTO>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<ProductVariantDTO>.Success(productVariantDTO, "Product variant added successfully");
        }

        public async Task<Result<ProductVariantDTO>> GetProductVariantByIdAsync(int id)
        {
            var productVariantDTO = await _productVariantRepository
                .GetBaseQuery(pv => pv.Id == id)
                .ProjectTo<ProductVariantDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (productVariantDTO is null)
            {
                return Result<ProductVariantDTO>.Failure($"Product variant Id {id} not exist");
            }

            return Result<ProductVariantDTO>.Success(productVariantDTO, "Product variant retrived successfully");
        }
    }
}
