using Clipo.Application.UseCases.GetVideosByUser;

namespace Clipo.Presentation.Presenters.VideoConverter
{
    public class GetVideosByUserPresenter : IGetVideosByUserOutputPort
    {
        public List<GetVideosByUserOutput>? ViewModel { get; private set; }
        public string? ErrorMessage { get; private set; }
        public bool IsNotFound { get; private set; }

        public void Success(List<GetVideosByUserOutput> videos)
        {
            ViewModel = videos;
            ErrorMessage = null;
            IsNotFound = false;
        }

        public void Invalid(string message)
        {
            ViewModel = null;
            ErrorMessage = message;
            IsNotFound = false;
        }

        public void NotFound()
        {
            ViewModel = null;
            ErrorMessage = null;
            IsNotFound = true;
        }
    }
}
