using Microsoft.AspNetCore.Http;

namespace EcommerceApp.Application.DTO.Requests
{
    public class ProductVariantRequestDTO
    {
        public required string SKU { get; set; }
        public required string OptionName { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
        public required int Stock { get; set; }
        public required int ProductId { get; set; }
    }
}
