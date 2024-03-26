using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BuildyBackend.Infrastructure.Services
{
    /// <summary>
    /// Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/20094958#notes
    /// </summary>
    public class FileStorageLocal : IFileStorage
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileStorageLocal(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string path, string container)
        {
            if (path != null)
            {
                var fileName = Path.GetFileName(path);
                string filePath = Path.Combine(_environment.WebRootPath, container, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType)
        {
            await DeleteFile(path, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_environment.WebRootPath, container);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string path = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(path, content);

            var currentUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            return Path.Combine(currentUrl, container, fileName).Replace("\\", "/");
        }

        public async Task DeleteFolder(string container)
        {
            string folderPath = Path.Combine(_environment.WebRootPath, container);
            if (Directory.Exists(folderPath))
            {
                await Task.Run(() => Directory.Delete(folderPath, true));
            }
        }

    }
}