using AiCustomerSupport.Application.Contracts.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AiCustomerSupport.Infrastructure.Services
{
    public class FileStorageService :IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "attachments");

            Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{file.FileName}-{Guid.NewGuid()}{extension}";

            var physicalPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(stream);

            string filePath = Path.Combine("attachments", fileName)
                .Replace("\\", "/");

            return filePath;
        }
    }
}
