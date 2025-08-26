using Clipo.Application.UseCases.ConvertVideoToFrame;

namespace Clipo.Presentation.Presenters.VideoConverter
{
    public class ConvertVideoToFramePresenter : IConvertVideoToFrameOutputPort
    {
        public ConvertVideoToFrameOutput? ViewModel { get; private set; }
        public string? ErrorMessage { get; private set; }

        public void Invalid(string message)
        {
            ViewModel = null;
            ErrorMessage = message;
        }

        public void Ok(ConvertVideoToFrameOutput output)
        {
            ViewModel = output;
            ErrorMessage = null;
        }
    }
}
