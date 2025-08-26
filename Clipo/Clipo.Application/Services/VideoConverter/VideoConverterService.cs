using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using FFMpegCore;
using Microsoft.Extensions.Logging;

namespace Clipo.Application.Services.VideoConverter
{
    public class VideoConverterService
    {
        private readonly IVideoStatusRepository _videoRepository;
        private readonly ILogger<VideoConverterService> _logger;

        public VideoConverterService(IVideoStatusRepository videoRepository, ILogger<VideoConverterService> logger)
        {
            _videoRepository = videoRepository;
            _logger = logger;
        }

        public async Task ProcessAsync(long jobId, CancellationToken ct)
        {
            Domain.AggregatesModel.VideoAggregate.VideoStatus? video = await _videoRepository.GetByIdAsync(jobId, ct);
            if(video is null) return;

            try
            {
                video.Status = Domain.AggregatesModel.VideoAggregate.Enums.ProcessStatus.Processing;
                video.Progress = 0;
                await _videoRepository.UpdateAsync(video, ct);

                IMediaAnalysis analysis = await FFProbe.AnalyseAsync(video.FilePath);
                double duration = analysis.Duration.TotalSeconds;

                int fps = 1;
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

                // zipa frames
                string zipFilePath = Path.Combine("frames", $"{video.Id}.zip");
                if(File.Exists(zipFilePath)) File.Delete(zipFilePath);
                System.IO.Compression.ZipFile.CreateFromDirectory(framesDir, zipFilePath);

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

                _logger.LogError(ex, "Erro ao processar vídeo {JobId}", video.Id);
            }
        }
    }
}
