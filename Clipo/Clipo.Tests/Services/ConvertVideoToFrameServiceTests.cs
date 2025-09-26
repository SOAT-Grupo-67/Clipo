using Clipo.Application.UseCases.ConvertVideoToFrame;
using Clipo.Domain.AggregatesModel.VideoAggregate;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace Clipo.Tests.Services
{
    public class ConvertVideoToFrameServiceTests
    {
        private readonly Mock<IVideoStatusRepository> _mockRepository;
        private readonly Mock<IBackgroundJobClient> _mockHangfire;
        private readonly Mock<IConvertVideoToFrameOutputPort> _mockOutputPort;
        private readonly Mock<ILogger<ConvertVideoToFrameInteractor>> _mockLogger;
        private readonly ConvertVideoToFrameInteractor _service;

        public ConvertVideoToFrameServiceTests()
        {
            _mockRepository = new Mock<IVideoStatusRepository>();
            _mockHangfire = new Mock<IBackgroundJobClient>();
            _mockOutputPort = new Mock<IConvertVideoToFrameOutputPort>();
            _mockLogger = new Mock<ILogger<ConvertVideoToFrameInteractor>>();
            
            _service = new ConvertVideoToFrameInteractor(
                _mockRepository.Object,
                _mockHangfire.Object,
                _mockOutputPort.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_WithRepositoryException_ShouldCallInvalid()
        {
            var userId = Guid.NewGuid();
            var mockFile = CreateMockFile("test.mp4", "video/mp4", 1024);
            var input = new ConvertVideoToFrameInput(mockFile, userId);
            
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<VideoStatus>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            await _service.Handle(input, CancellationToken.None);

            _mockOutputPort.Verify(o => o.Invalid(It.IsAny<string>()), Times.Once);
        }

        private IFormFile CreateMockFile(string fileName, string contentType, long length)
        {
            var mockFile = new Mock<IFormFile>();
            var content = Encoding.UTF8.GetBytes("Mock file content");
            var stream = new MemoryStream(content);
            
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.Length).Returns(length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            return mockFile.Object;
        }
    }
}