namespace Clipo.Application.UseCases.ConvertVideoToFrame
{
    public interface IConvertVideoToFrameOutputPort
    {
        void Invalid(string message);

        void Ok(ConvertVideoToFrameOutput output);

        ConvertVideoToFrameOutput? ViewModel { get; }
        string? ErrorMessage { get; }
    }
}
