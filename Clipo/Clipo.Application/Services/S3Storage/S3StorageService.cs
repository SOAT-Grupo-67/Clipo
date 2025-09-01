using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Clipo.Application.Services.S3Storage
{
    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly ILogger<S3StorageService> _logger;

        public S3StorageService(IAmazonS3 s3Client, IConfiguration configuration, ILogger<S3StorageService> logger)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:S3:BucketName"] ?? throw new ArgumentNullException("AWS:S3:BucketName configuration is required");
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(string filePath, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");
                }

                var transferUtility = new TransferUtility(_s3Client);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    FilePath = filePath,
                    Key = fileName,
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private
                };

                await transferUtility.UploadAsync(uploadRequest, cancellationToken);

                var s3Url = $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
                _logger.LogInformation("Arquivo {FileName} enviado com sucesso para S3: {S3Url}", fileName, s3Url);

                return s3Url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload do arquivo {FileName} para S3", fileName);
                throw;
            }
        }



        public async Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
                _logger.LogInformation("Arquivo {FileName} deletado com sucesso do S3", fileName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar arquivo {FileName} do S3", fileName);
                return false;
            }
        }
    }
}
