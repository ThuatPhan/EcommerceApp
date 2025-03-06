namespace EcommerceApp.Application.DTO.Responses
{
    public class OrderItemDTO
    {
        public SimpleProductDTO Product { get; set; }
        public ProductVariantDTO? SelectedVariant { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
