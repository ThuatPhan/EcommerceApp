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
    public class OrderService : IOrderService
    {
        private readonly IReposistory<Product> _productRepo;
        private readonly IReposistory<ProductVariant> _variantRepo;
        private readonly IReposistory<Order> _orderRepo;
        private readonly IReposistory<OrderItem> _orderItemRepo;
        private readonly IMapper _mapper;

        public OrderService(
            IReposistory<Product> productRepo,
            IReposistory<ProductVariant> variantRepo,
            IReposistory<Order> orderRepo,
            IReposistory<OrderItem> orderItemRepo,
            IMapper mapper
        )
        {
            _productRepo = productRepo;
            _variantRepo = variantRepo;
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _mapper = mapper;
        }

        public async Task<Result<OrderDTO>> CreateOrderAsync(string userId, DateTime createdDate, List<OrderItemRequestDTO> orderItems)
        {
            var productIds = orderItems.Select(i => i.ProductId).Distinct().ToList();
            var variantIds = orderItems.Where(i => i.VariantId.HasValue).Select(i => i.VariantId.Value).Distinct().ToList();

            var existingProductIds = await _productRepo
                .GetBaseQuery(p => productIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            if (existingProductIds.Count != productIds.Count)
            {
                var invalidProductIds = productIds.Except(existingProductIds).ToList();
                return Result<OrderDTO>.Failure($"Invalid Product IDs: {string.Join(", ", invalidProductIds)}");
            }

            var existingVariantIds = await _variantRepo
                .GetBaseQuery(v => variantIds.Contains(v.Id))
                .Select(v => v.Id)
                .ToListAsync();

            if (existingVariantIds.Count != variantIds.Count)
            {
                var invalidVariantIds = variantIds.Except(existingVariantIds).ToList();
                return Result<OrderDTO>.Failure($"Invalid Variant IDs: {string.Join(", ", invalidVariantIds)}");
            }

            var newOrder = await _orderRepo.AddAsync(new Order
            {
                UserId = userId,
                CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(
                    createdDate,
                    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                ),
                Items = orderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    VariantId = i.VariantId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                }).ToList(),
                TotalAmount = orderItems.Sum(i => i.Quantity * i.Price)
            });

            var orderDTO = await _orderRepo
                .GetBaseQuery(predicate: o => o.Id == newOrder.Id)
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<OrderDTO>.Success(orderDTO, "Order created successfully");
        }

        public async Task<Result<OrderDTO>> GetOrderByIdAsync(int id, string userId)
        {
            var order = await _orderRepo
                .GetBaseQuery(predicate: o => o.Id == id && o.UserId == userId)
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (order is null)
            {
                return Result<OrderDTO>.Failure($"Order Id {id} not exist");
            }

            return Result<OrderDTO>.Success(order, "Order retrieved successfully");
        }

        public async Task<Result<List<OrderDTO>>> GetOrdersAsync(string userId)
        {
            var orders = await _orderRepo.GetBaseQuery(predicate: o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedDate)
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<List<OrderDTO>>.Success(orders, "Orders retrieved successfully");
        }

    }
}
