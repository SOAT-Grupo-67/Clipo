namespace Clipo.Application.UseCases.GetVideoStatus
{
    public class GetVideoStatusOutput
    {
        public long Id { get; set; }
        public string FilePath { get; set; } = default!;
        public string Status { get; set; } = default!;
        public int Progress { get; set; }
        public string? S3Url { get; set; }
    }
}
