namespace Clipo.Application.UseCases.GetVideosByUser
{
    public interface IGetVideosByUserOutputPort
    {
        void Success(List<GetVideosByUserOutput> videos);
        void Invalid(string message);
        void NotFound();
    }
}
