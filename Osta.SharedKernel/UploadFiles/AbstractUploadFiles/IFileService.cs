using Microsoft.AspNetCore.Http;

namespace Osta.SharedKernel
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderPath,
            CancellationToken ct = default);
        Task<string> UploadFileAsync(string Location, IFormFile file);
        Task<string> UploadVideo(string location, IFormFile file);
        Task<string> DeleteImage(string imageUrl, string folderPath);
    }
}
