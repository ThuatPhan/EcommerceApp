using EcommerceApp.Application.DTO.Responses;
using Microsoft.AspNetCore.Http;

namespace EcommerceApp.Application.Interfaces
{
    public interface IS3Service
    {
        Task<Result<string>> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string filePath);
    }
}
