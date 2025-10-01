namespace Clipo.Application.UseCases.GetVideosByUser
{
    public class GetVideosByUserOutput
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
        public string Status { get; set; } = default!;
        public int Progress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ZipPath { get; set; }
        public string? S3Url { get; set; }
    }
}
