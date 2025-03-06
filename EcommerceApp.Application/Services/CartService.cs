using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IReposistory<Cart> _cartRepository;
        private readonly IReposistory<CartItem> _cartItemRepository;
        private readonly IMapper _mapper;

        public CartService(IReposistory<Cart> cartRepository, IReposistory<CartItem> cartItemRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
        }

        public async Task<Result<CartItemDTO>> AddCartItemAsync(string userId, CartItemRequestDTO requestDTO)
        {
            var cart = await _cartRepository
                .GetBaseQuery(c => c.UserId == userId)
                .FirstOrDefaultAsync() ?? await _cartRepository.AddAsync(new Cart
                {
                    UserId = userId
                });


            var cartItemExist = await _cartItemRepository
                .GetBaseQuery(
                    ci => ci.CartId == cart.Id
                    && ci.ProductId == requestDTO.ProductId
                    && ci.VariantId == requestDTO.VariantId
                )
                .FirstOrDefaultAsync();

            if (cartItemExist != null)
            {
                cartItemExist.Quantity += requestDTO.Quantity;
                await _cartItemRepository.UpdateAsync(cartItemExist);
            }
            else
            {
                var cartItemToAdd = _mapper.Map<CartItem>(requestDTO);
                cartItemToAdd.CartId = cart.Id;
                await _cartItemRepository.AddAsync(cartItemToAdd);
            }

            var cartItemDTO = await _cartItemRepository
                .GetBaseQuery(predicate: ci => ci.ProductId == requestDTO.ProductId && ci.VariantId == requestDTO.VariantId)
                .ProjectTo<CartItemDTO>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<CartItemDTO>.Success(cartItemDTO, "Cart item added successfully");
        }

        public async Task<Result<List<CartItemDTO>>> GetCartOfUserAsync(string userId)
        {
            var cartItems = await _cartRepository
                .GetBaseQuery(c => c.UserId == userId)
                .SelectMany(c => c.Items)
                .ProjectTo<CartItemDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<List<CartItemDTO>>.Success(cartItems, "Cart items retrieved successfully");
        }

        public async Task<Result<int>> GetCartItemCountAsync(string userId)
        {
            var cart = await _cartRepository
                .GetBaseQuery(c => c.UserId == userId)
                .FirstOrDefaultAsync();

            if(cart is null)
            {
                return Result<int>.Success(0, "Cart item count retrieved successfully");
            }

            var itemCount = await _cartItemRepository
                .GetBaseQuery(ci => ci.CartId == cart.Id)
                .CountAsync();

            return Result<int>.Success(itemCount, "Cart item count retrieved successfully");
        }

        public async Task<Result<CartItemDTO>> UpdateCartItemQuantityAsync(string userId, CartItemRequestDTO requestDTO)
        {

            if (requestDTO.Quantity == 0)
            {
                return Result<CartItemDTO>.Failure("Quantity must be greater than 0");
            }

            var cart = await _cartRepository
                .GetBaseQuery(c => c.UserId == userId)
                .FirstOrDefaultAsync();

            if (cart is null)
            {
                return Result<CartItemDTO>.Failure($"Cart of user Id {userId} not exist");
            }

            var cartItem = await _cartItemRepository
                .GetBaseQuery(
                    ci => ci.CartId == cart.Id
                    && ci.ProductId == requestDTO.ProductId
                    && ci.VariantId == requestDTO.VariantId
                )
                .FirstOrDefaultAsync();

            if (cartItem is null)
            {
                return Result<CartItemDTO>.Failure("Product or variant not exists in cart of user");
            }

            cartItem.Quantity = requestDTO.Quantity;

            await _cartItemRepository.UpdateAsync(cartItem);

            var cartItemDTO = await _cartItemRepository
                .GetBaseQuery(predicate: ci => ci.ProductId == requestDTO.ProductId && ci.VariantId == requestDTO.VariantId)
                .ProjectTo<CartItemDTO>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<CartItemDTO>.Success(_mapper.Map<CartItemDTO>(cartItemDTO), "Cart item updated successfully");
        }

        public async Task<Result<bool>> DeleteCartItemAsync(string userId, int productId, int? variantId)
        {
            var cartWithItem = await _cartRepository.GetBaseQuery(c => c.UserId == userId)
                .Select(c => new
                {
                    CartId = c.Id,
                    CartItem = c.Items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId)
                })
                .FirstOrDefaultAsync();

            if (cartWithItem == null)
            {
                return Result<bool>.Failure($"Cart of userId {userId} does not exist");
            }

            if (cartWithItem.CartItem == null)
            {
                return Result<bool>.Failure($"Cart item does not exist in the cart of userId {userId}");
            }

            await _cartItemRepository.RemoveAsync(cartWithItem.CartItem);

            return Result<bool>.Success(true, "Cart item deleted successfully");
        }

    }
}
