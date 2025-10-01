using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Microsoft.Extensions.Logging;

namespace Clipo.Application.UseCases.GetVideosByUser
{
    public class GetVideosByUserInteractor : IGetVideosByUserInputPort
    {
        private readonly IVideoStatusRepository _videoRepository;
        private readonly IGetVideosByUserOutputPort _output;
        private readonly ILogger<GetVideosByUserInteractor> _logger;

        public GetVideosByUserInteractor(
            IVideoStatusRepository videoRepository,
            IGetVideosByUserOutputPort output,
            ILogger<GetVideosByUserInteractor> logger)
        {
            _videoRepository = videoRepository;
            _output = output;
            _logger = logger;
        }

        public async Task Handle(GetVideosByUserInput input, CancellationToken cancellationToken)
        {
            try
            {
                if (input.UserId == 0)
                {
                    _output.Invalid("O ID do usuário é inválido.");
                    return;
                }

                List<Domain.AggregatesModel.VideoAggregate.VideoStatus> videos = _videoRepository.GetAllByUser(input.UserId);

                if (videos == null || !videos.Any())
                {
                    _output.NotFound();
                    return;
                }

                List<GetVideosByUserOutput> outputVideos = videos.Select(v => new GetVideosByUserOutput
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    FileName = v.FileName,
                    FilePath = v.FilePath,
                    Status = v.Status.ToString(),
                    Progress = v.Progress,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    ZipPath = v.ZipPath,
                    S3Url = v.S3Url
                }).ToList();

                _output.Success(outputVideos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar vídeos do usuário {UserId}", input.UserId);
                _output.Invalid("Erro interno do servidor ao buscar vídeos.");
            }
        }
    }
}
