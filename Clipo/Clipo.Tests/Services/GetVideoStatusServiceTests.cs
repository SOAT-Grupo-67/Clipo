using Clipo.Application.UseCases.GetVideoStatus;
using Clipo.Domain.AggregatesModel.VideoAggregate;
using Clipo.Domain.AggregatesModel.VideoAggregate.Enums;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Clipo.Tests.Services
{
    public class GetVideoStatusServiceTests
    {
        private readonly Mock<IVideoStatusRepository> _mockRepository;
        private readonly Mock<IGetVideoStatusOutputPort> _mockOutputPort;
        private readonly Mock<ILogger<GetVideoStatusInteractor>> _mockLogger;
        private readonly GetVideoStatusInteractor _service;

        public GetVideoStatusServiceTests()
        {
            _mockRepository = new Mock<IVideoStatusRepository>();
            _mockOutputPort = new Mock<IGetVideoStatusOutputPort>();
            _mockLogger = new Mock<ILogger<GetVideoStatusInteractor>>();
            
            _service = new GetVideoStatusInteractor(
                _mockRepository.Object,
                _mockOutputPort.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnVideoStatus()
        {
            long videoId = 1;
            var video = new VideoStatus
            {
                Id = videoId,
                FilePath = "/path/to/video.mp4",
                Status = ProcessStatus.Done,
                Progress = 100,
                S3Url = "https://s3.example.com/video.zip"
            };

            _mockRepository.Setup(r => r.GetStatusById(videoId)).ReturnsAsync(video);

            await _service.Handle(new GetVideoStatusInput(videoId), CancellationToken.None);

            _mockOutputPort.Verify(o => o.Success(It.Is<GetVideoStatusOutput>(
                v => v.Id == videoId && v.Status == ProcessStatus.Done.ToString())), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldCallInvalid()
        {
            long invalidId = -1;

            await _service.Handle(new GetVideoStatusInput(invalidId), CancellationToken.None);

            _mockOutputPort.Verify(o => o.Invalid(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentId_ShouldCallNotFound()
        {
            long nonExistentId = 999;
            _mockRepository.Setup(r => r.GetStatusById(nonExistentId)).ReturnsAsync((VideoStatus)null);

            await _service.Handle(new GetVideoStatusInput(nonExistentId), CancellationToken.None);

            _mockOutputPort.Verify(o => o.NotFound(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithRepositoryException_ShouldCallInvalid()
        {
            long videoId = 1;
            _mockRepository.Setup(r => r.GetStatusById(videoId)).ThrowsAsync(new Exception("Database error"));

            await _service.Handle(new GetVideoStatusInput(videoId), CancellationToken.None);

            _mockOutputPort.Verify(o => o.Invalid(It.IsAny<string>()), Times.Once);
        }
    }
}