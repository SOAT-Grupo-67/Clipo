using Clipo.Application.UseCases.GetVideosByUser;
using Clipo.Domain.AggregatesModel.VideoAggregate;
using Clipo.Domain.AggregatesModel.VideoAggregate.Enums;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Clipo.Tests.Services
{
    public class GetVideosByUserServiceTests
    {
        private readonly Mock<IVideoStatusRepository> _mockRepository;
        private readonly Mock<IGetVideosByUserOutputPort> _mockOutputPort;
        private readonly Mock<ILogger<GetVideosByUserInteractor>> _mockLogger;
        private readonly GetVideosByUserInteractor _service;

        public GetVideosByUserServiceTests()
        {
            _mockRepository = new Mock<IVideoStatusRepository>();
            _mockOutputPort = new Mock<IGetVideosByUserOutputPort>();
            _mockLogger = new Mock<ILogger<GetVideosByUserInteractor>>();
            
            _service = new GetVideosByUserInteractor(
                _mockRepository.Object,
                _mockOutputPort.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_WithValidUserId_ShouldReturnVideos()
        {
            var userId = Guid.NewGuid();
            var videos = new List<VideoStatus>
            {
                new VideoStatus
                {
                    Id = 1,
                    UserId = userId,
                    FileName = "video1.mp4",
                    FilePath = "/path/to/video1.mp4",
                    Status = ProcessStatus.Done,
                    Progress = 100,
                    UpdatedAt = DateTime.Now
                }
            };

            _mockRepository.Setup(r => r.GetAllByUser(userId)).Returns(videos);

            await _service.Handle(new GetVideosByUserInput(userId), CancellationToken.None);

            _mockOutputPort.Verify(o => o.Success(It.Is<List<GetVideosByUserOutput>>(
                list => list.Count == 1 && list[0].Id == 1)), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNoVideosFound_ShouldCallNotFound()
        {
            var userId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetAllByUser(userId)).Returns(new List<VideoStatus>());

            await _service.Handle(new GetVideosByUserInput(userId), CancellationToken.None);

            _mockOutputPort.Verify(o => o.NotFound(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithRepositoryException_ShouldCallInvalid()
        {
            var userId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetAllByUser(userId)).Throws(new Exception("Database error"));

            await _service.Handle(new GetVideosByUserInput(userId), CancellationToken.None);

            _mockOutputPort.Verify(o => o.Invalid(It.IsAny<string>()), Times.Once);
        }
    }
}