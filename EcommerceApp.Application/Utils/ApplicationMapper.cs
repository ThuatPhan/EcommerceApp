using AutoMapper;
using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Utils
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<CategoryRequestDTO, Category>();
            CreateMap<Category, CategoryDTO>();

            CreateMap<ProductRequestDTO, Product>();
            CreateMap<Product, ProductDTO>();
            CreateMap<Product, SimpleProductDTO>();

            CreateMap<ProductVariantRequestDTO, ProductVariant>();
            CreateMap<ProductVariant, ProductVariantDTO>();

            CreateMap<CartItemRequestDTO, CartItem>();
            CreateMap<CartItem, CartItemDTO>()
                .ForMember(dest => dest.SelectedVariant, opt => opt.MapFrom(src => src.Variant != null ? src.Variant : null))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Product.Price * src.Quantity));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.SelectedVariant, opt => opt.MapFrom(src => src.Variant))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(
                    src => src.Quantity * (src.Variant != null ? src.Variant.Price : src.Product.Price)
                ));
            CreateMap<Order, OrderDTO>();
            CreateMap<FavouriteProduct, FavouriteProductDTO>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
        }
    }
}
