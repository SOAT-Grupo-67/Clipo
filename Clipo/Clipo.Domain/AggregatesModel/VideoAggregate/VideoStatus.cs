using Clipo.Domain.AggregatesModel.Base;
using Clipo.Domain.AggregatesModel.VideoAggregate.Enums;

namespace Clipo.Domain.AggregatesModel.VideoAggregate
{
    public class VideoStatus : BaseModel
    {
        public Guid UserId { get; set; }
        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
        public string? ZipPath { get; set; }
        public string? S3Url { get; set; }
        public ProcessStatus Status { get; set; } = ProcessStatus.Queued;
        public int Progress { get; set; } = 0;
    }
}
