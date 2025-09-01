namespace Clipo.Application.UseCases.GetVideosByUser
{
    public interface IGetVideosByUserInputPort
    {
        Task Handle(GetVideosByUserInput input, CancellationToken cancellationToken);
    }
}
