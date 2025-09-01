using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Microsoft.Extensions.Logging;

namespace Clipo.Application.UseCases.GetVideoStatus
{
    public class GetVideoStatusInteractor : IGetVideoStatusInputPort
    {
        private readonly IVideoStatusRepository _videoRepository;
        private readonly IGetVideoStatusOutputPort _output;
        private readonly ILogger<GetVideoStatusInteractor> _logger;

        public GetVideoStatusInteractor(
            IVideoStatusRepository videoRepository,
            IGetVideoStatusOutputPort output,
            ILogger<GetVideoStatusInteractor> logger)
        {
            _videoRepository = videoRepository;
            _output = output;
            _logger = logger;
        }

        public async Task Handle(GetVideoStatusInput input, CancellationToken cancellationToken)
        {
            try
            {
                if (input.VideoId <= 0)
                {
                    _output.Invalid("O ID do vídeo é inválido.");
                    return;
                }

                var video = await _videoRepository.GetStatusById(input.VideoId);

                if (video == null)
                {
                    _output.NotFound();
                    return;
                }

                var outputVideo = new GetVideoStatusOutput
                {
                    Id = video.Id,
                    FilePath = video.FilePath,
                    Status = video.Status.ToString(),
                    Progress = video.Progress,
                    S3Url = video.S3Url
                };

                _output.Success(outputVideo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar status do vídeo {VideoId}", input.VideoId);
                _output.Invalid("Erro interno do servidor ao buscar status do vídeo.");
            }
        }
    }
}
