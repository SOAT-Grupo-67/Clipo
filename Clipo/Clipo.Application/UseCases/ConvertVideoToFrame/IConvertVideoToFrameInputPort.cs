namespace Clipo.Application.UseCases.ConvertVideoToFrame
{
    public interface IConvertVideoToFrameInputPort
    {
        Task Handle(ConvertVideoToFrameInput input, CancellationToken cancellationToken);
    }
}
