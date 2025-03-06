using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IReposistory<FavouriteProduct> _favouriteProductRepository;
        private readonly IReposistory<Product> _productRepository;
        private readonly IMapper _mapper;

        public UserService(IReposistory<FavouriteProduct> favouriteProducts, IReposistory<Product> productRepository, IMapper mapper)
        {
            _favouriteProductRepository = favouriteProducts;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Result<FavouriteProductDTO>> AddFavouriteProductAsync(string userId, int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
            {
                return Result<FavouriteProductDTO>.Failure($"Product Id {productId} not exist");
            }

            var addedFavouriteProduct = await _favouriteProductRepository.AddAsync(
                new FavouriteProduct { UserId = userId, ProductId = productId }
            );

            var favouriteProductDTO = await _favouriteProductRepository
                .GetBaseQuery(f => f.Id == addedFavouriteProduct.Id)
                .ProjectTo<FavouriteProductDTO>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<FavouriteProductDTO>.Success(favouriteProductDTO, "Favourite product added successfully");
        }

        public async Task<Result<List<FavouriteProductDTO>>> GetFavouriteProductsAsync(string userId)
        {
            var favouriteProducts = await _favouriteProductRepository
                .GetBaseQuery(f => f.UserId == userId)
                .ProjectTo<FavouriteProductDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<List<FavouriteProductDTO>>.Success(favouriteProducts, "Favourite products retrieved successfully");
        }
    }
}
