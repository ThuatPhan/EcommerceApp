using Microsoft.AspNetCore.Http;

namespace EcommerceApp.Application.DTO.Requests
{
    public class ProductRequestDTO
    {
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public required int Stock { get; set; }
        public required int CategoryId { get; set; }
    }
}
