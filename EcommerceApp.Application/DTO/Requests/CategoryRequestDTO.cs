using Microsoft.AspNetCore.Http;

namespace EcommerceApp.Application.DTO.Requests
{
    public class CategoryRequestDTO
    {
        public required string Name { get; set; }
        public IFormFile? IconFile { get; set; }
    }
}
