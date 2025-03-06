namespace EcommerceApp.Application.DTO.Requests
{
    public class OrderItemRequestDTO
    {
        public required int ProductId { get; set; }
        public int? VariantId { get; set; }
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }
    }
}
