using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace Osta.SharedKernel
{
    public class FileService : IFileService
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", "MP4" };
        private const long MaxFileSizeBytes = 30 * 1024 * 1024;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<string> UploadImageAsync(IFormFile file, string folderPath, CancellationToken ct = default)
        {
            var path = _webHostEnvironment.WebRootPath + "/" + folderPath + "/";
            var Extension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + Extension;

            if (file.Length > 0)
            {
                if (file.Length > MaxFileSizeBytes)
                    return "Maximum Size Can Be 7mb";


                if (!AllowedExtensions.Contains(Extension))
                    return $"Invalid file type With Extension {Extension}";

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                using (FileStream fileStream = File.Create(path + fileName))
                {
                    await file.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                    return $"/{folderPath}/{fileName}";
                }

            }
            return "File is null or empty.";
        }

        public Task<string> DeleteImage(string imageUrl, string folderPath)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL is null or empty.", nameof(imageUrl));

            var relativePath = imageUrl.TrimStart('/')
                                       .Replace('/', Path.DirectorySeparatorChar);

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult("Image deleted successfully.");
            }

            return Task.FromResult("Image not found.");
        }

        public async Task<string> UploadFileAsync(string Location, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");



            var extension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedExtensions = { ".pdf", ".zip", ".docx", ".pptx", ".jpg", ".png", ".xlsx" };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid File extension");

            long maxSize = 40 * 1024 * 1024;
            if (file.Length > maxSize)
                throw new InvalidOperationException("File size exceeded");

            var path = Path.Combine(_webHostEnvironment.WebRootPath, Location);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{Location}/{fileName}";
        }

        public async Task<string> UploadVideo(string location, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            if (!file.ContentType.StartsWith("video/"))
                throw new InvalidOperationException("Invalid video content");

            var extension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedExtensions = { ".mp4", ".avi", ".mov", ".mkv" };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid video extension");

            long maxSize = 400 * 1024 * 1024;
            if (file.Length > maxSize)
                throw new InvalidOperationException("File size exceeded");

            var path = Path.Combine(_webHostEnvironment.WebRootPath, location);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{location}/{fileName}";
        }
    }
}