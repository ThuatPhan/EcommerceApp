namespace EcommerceApp.Domain.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public required string SKU { get; set; }
        public required string Name { get; set; }
        public required string OptionName { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Stock { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
