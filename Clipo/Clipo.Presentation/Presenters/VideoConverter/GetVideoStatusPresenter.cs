using Clipo.Application.UseCases.GetVideoStatus;

namespace Clipo.Presentation.Presenters.VideoConverter
{
    public class GetVideoStatusPresenter : IGetVideoStatusOutputPort
    {
        public GetVideoStatusOutput? ViewModel { get; private set; }
        public string? ErrorMessage { get; private set; }
        public bool IsNotFound { get; private set; }

        public void Success(GetVideoStatusOutput video)
        {
            ViewModel = video;
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
