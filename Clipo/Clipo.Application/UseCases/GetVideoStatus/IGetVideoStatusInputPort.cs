namespace Clipo.Application.UseCases.GetVideoStatus
{
    public interface IGetVideoStatusInputPort
    {
        Task Handle(GetVideoStatusInput input, CancellationToken cancellationToken);
    }
}
