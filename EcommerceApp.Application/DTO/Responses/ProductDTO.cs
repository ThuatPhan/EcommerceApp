namespace EcommerceApp.Application.DTO.Responses
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Image { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public required CategoryDTO Category { get; set; }
        public List<ProductVariantDTO>? Variants { get; set; }
        public int? VariantId { get; set; }
    }
}
