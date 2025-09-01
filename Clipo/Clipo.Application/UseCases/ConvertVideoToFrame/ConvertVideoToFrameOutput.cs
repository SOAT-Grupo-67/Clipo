namespace Clipo.Application.UseCases.ConvertVideoToFrame
{
    public record ConvertVideoToFrameOutput(long JobId, string Status, DateTime CreatedAt, string? S3Url = null);

}
