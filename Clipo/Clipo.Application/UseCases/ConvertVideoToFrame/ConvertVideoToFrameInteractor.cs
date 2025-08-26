using Clipo.Application.Services.VideoConverter;
using Clipo.Domain.AggregatesModel.VideoAggregate;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Clipo.Domain.Extensions;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Clipo.Application.UseCases.ConvertVideoToFrame
{
    public class ConvertVideoToFrameInteractor : IConvertVideoToFrameInputPort
    {
        private readonly IVideoStatusRepository _videoRepository;
        private readonly IBackgroundJobClient _hangfire;
        private readonly IConvertVideoToFrameOutputPort _output;
        private readonly ILogger<ConvertVideoToFrameInteractor> _logger;

        public ConvertVideoToFrameInteractor(
            IVideoStatusRepository videoRepository,
            IBackgroundJobClient hangfire,
            IConvertVideoToFrameOutputPort output,
            ILogger<ConvertVideoToFrameInteractor> logger)
        {
            _videoRepository = videoRepository;
            _hangfire = hangfire;
            _output = output;
            _logger = logger;
        }

        public async Task Handle(ConvertVideoToFrameInput input, CancellationToken cancellationToken)
        {

            try
            {
                if(input.File is null || input.File.Length == 0)
                {
                    _output.Invalid("O arquivo de vídeo é obrigatório.");
                    return;
                }

                string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                Directory.CreateDirectory(uploadsDir);

                string filePath = Path.Combine(uploadsDir, Guid.NewGuid() + Path.GetExtension(input.File.FileName));

                await using(FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await input.File.CopyToAsync(stream, cancellationToken);
                }

                VideoStatus job = new VideoStatus
                {
                    FileName = input.File.FileName,
                    FilePath = filePath,
                    Progress = 0,
                };

                await _videoRepository.AddAsync(job, cancellationToken);

                _hangfire.Enqueue<VideoConverterService>(j => j.ProcessAsync(job.Id, CancellationToken.None));

                _logger.LogInformation("Job {JobId} enfileirado com sucesso.", job.Id);

                _output.Ok(new ConvertVideoToFrameOutput(
                    job.Id,
                    job.Status.GetDescription(),
                    job.CreatedAt
                ));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao enfileirar job de conversão.");
                _output.Invalid("Erro inesperado ao enfileirar job de conversão.");
            }
        }

    }
}
