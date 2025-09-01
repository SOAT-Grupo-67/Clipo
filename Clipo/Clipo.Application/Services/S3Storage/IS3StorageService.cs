
namespace Clipo.Application.Services.S3Storage {
    public interface IS3StorageService {
        Task<string> UploadFileAsync(string filePath, string fileName, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
