using Clipo.Application.Services.EmailSender;
using Clipo.Application.Services.S3Storage;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using FFMpegCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg.Downloader;

namespace Clipo.Application.Services.VideoConverter
{
    public class VideoConverterService
    {
        private readonly IVideoStatusRepository _videoRepository;
        private readonly IS3StorageService _s3StorageService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VideoConverterService> _logger;
        private readonly IEmailSenderService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VideoConverterService(
            IVideoStatusRepository videoRepository,
            IS3StorageService s3StorageService,
            IConfiguration configuration,
            IEmailSenderService emailService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<VideoConverterService> logger)
        {
            _videoRepository = videoRepository;
            _s3StorageService = s3StorageService;
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task ProcessAsync(long jobId, CancellationToken ct = default)
        {
            Domain.AggregatesModel.VideoAggregate.VideoStatus? video = await _videoRepository.GetByIdAsync(jobId, ct);
            if(video is null) return;

            try
            {
                video.Status = Domain.AggregatesModel.VideoAggregate.Enums.ProcessStatus.Processing;
                video.Progress = 0;
                await _videoRepository.UpdateAsync(video, ct);

                string binFolder = Path.Combine(AppContext.BaseDirectory, "ffmpeg");
                await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, binFolder);

                GlobalFFOptions.Configure(new FFOptions
                {
                    BinaryFolder = binFolder,
                    TemporaryFilesFolder = Path.Combine(Path.GetTempPath(), "FFmpegTemp")
                });

                IMediaAnalysis analysis = await FFProbe.AnalyseAsync(video.FilePath);

                double duration = analysis.Duration.TotalSeconds;

                double fps = 0.5;
                int totalFrames = (int)Math.Ceiling(duration * fps);

                string framesDir = Path.Combine("frames", video.Id.ToString());
                Directory.CreateDirectory(framesDir);

                for(int second = 0; second < duration; second++)
                {
                    string frameFile = Path.Combine(framesDir, $"frame_{second:D4}.jpg");

                    await FFMpegArguments
                        .FromFileInput(video.FilePath, verifyExists: true, options => options.Seek(TimeSpan.FromSeconds(second)))
                        .OutputToFile(frameFile, overwrite: true, options => options
                            .WithFrameOutputCount(1)
                            .WithVideoCodec("mjpeg")
                            .ForceFormat("image2"))
                        .ProcessAsynchronously();

                    video.Progress = (int)(((double)(second + 1) / totalFrames) * 100);
                    await _videoRepository.UpdateAsync(video, ct);
                }

                string zipFilePath = Path.Combine("frames", $"{video.Id}.zip");
                if(File.Exists(zipFilePath)) File.Delete(zipFilePath);
                System.IO.Compression.ZipFile.CreateFromDirectory(framesDir, zipFilePath);

                bool uploadEnabled = _configuration.GetValue<bool>("AWS:UploadEnabled", false);
                if(uploadEnabled)
                {
                    try
                    {
                        string s3FileName = $"frames/{video.Id}/{Path.GetFileName(zipFilePath)}";
                        string s3Url = await _s3StorageService.UploadFileAsync(zipFilePath, s3FileName, ct);
                        video.S3Url = s3Url;
                        _logger.LogInformation("Arquivo ZIP enviado para S3: {S3Url}", s3Url);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao fazer upload para S3 do job {JobId}", video.Id);
                    }
                }

                video.Status = Domain.AggregatesModel.VideoAggregate.Enums.ProcessStatus.Done;

                video.ZipPath = zipFilePath;
                video.Progress = 100;
                await _videoRepository.UpdateAsync(video, ct);

                _logger.LogInformation("Job {JobId} concluído com sucesso.", video.Id);
            }
            catch(Exception ex)
            {
                video.Status = Domain.AggregatesModel.VideoAggregate.Enums.ProcessStatus.Error;
                video.Progress = 0;
                await _videoRepository.UpdateAsync(video, ct);

                _logger.LogError(ex, "Error while processing video {JobId}", video.Id);

                string? email = _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

                if(!string.IsNullOrEmpty(email))
                {
                    await _emailService.SendEmailAsync(
                        recipient: email,
                        subject: "Video Processing Failed",
                        body: $"Your video job {video.Id} failed with error: {ex.Message}",
                        ct: ct
                    );
                }
                else
                {
                    _logger.LogWarning("No email claim found in token for job {JobId}", video.Id);
                }
            }
        }
    }
}
