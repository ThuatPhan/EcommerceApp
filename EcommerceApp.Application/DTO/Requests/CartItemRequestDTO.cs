using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Application.DTO.Requests
{
    public class CartItemRequestDTO
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        [Range(1, 100, ErrorMessage = "Quantity must be in range 1 to 100")]
        public int Quantity { get; set; }
    }
}
