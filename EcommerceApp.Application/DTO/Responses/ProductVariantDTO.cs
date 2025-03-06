namespace EcommerceApp.Application.DTO.Responses
{
    public class ProductVariantDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string SKU { get; set; }
        public required string OptionName { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Stock { get; set; }
    }
}
