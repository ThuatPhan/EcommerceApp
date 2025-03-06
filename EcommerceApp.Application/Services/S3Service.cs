using Amazon.S3;
using Amazon.S3.Model;
using EcommerceApp.Application.DTO.Responses;
using EcommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EcommerceApp.Application.Services
{
    public class S3Service : IS3Service
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly string _cloundFrontDomain;

        public S3Service(IConfiguration configuration)
        {
            _configuration = configuration;
            _accessKey = _configuration["AwsS3:AccessKey"]!;
            _secretKey = _configuration["AwsS3:SecretKey"]!;
            _s3Client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.APSoutheast1);
            _bucketName = _configuration["AwsS3:BucketName"]!;
            _cloundFrontDomain = _configuration["AwsCloudFront:Domain"]!;
        }

        public async Task<Result<string>> UploadFileAsync(IFormFile file)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = file.FileName,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };
            var putObjectResponse = await _s3Client.PutObjectAsync(putObjectRequest);
            if (putObjectResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                string filePath = $"{_cloundFrontDomain}/{file.FileName}";
                return Result<string>.Success(filePath, "File uploaded successfully");
            }
            return Result<string>.Failure("Failed to upload file");
        }

        public async Task DeleteFileAsync(string filePath)
        {
            string fileKey = filePath.Substring(filePath.LastIndexOf("/") + 1);
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
            };
            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
    }
}
