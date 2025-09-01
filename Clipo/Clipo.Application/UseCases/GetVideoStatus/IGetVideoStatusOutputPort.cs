namespace Clipo.Application.UseCases.GetVideoStatus
{
    public interface IGetVideoStatusOutputPort
    {
        void Success(GetVideoStatusOutput video);
        void Invalid(string message);
        void NotFound();
    }
}
