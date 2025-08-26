using Clipo.Application.UseCases.ConvertVideoToFrame;
using Clipo.Presentation.Presenters.VideoConverter;

namespace Clipo.Presentation.Controller.VideoConverter
{
    public class ConvertVideoToFrameController
    {
        private readonly IConvertVideoToFrameInputPort _input;
        private readonly IConvertVideoToFrameOutputPort _output;

        public ConvertVideoToFrameController(IConvertVideoToFrameInputPort input,
                                       IConvertVideoToFrameOutputPort output)
        {
            _input = input;
            _output = output;
        }

        public async Task<ConvertVideoToFrameOutput?> ExecuteAsync(
            ConvertVideoToFrameInput dto, CancellationToken ct = default)
        {
            await _input.Handle(dto, ct);

            ConvertVideoToFramePresenter? presenter = _output as ConvertVideoToFramePresenter;
            if(presenter?.ErrorMessage != null)
            {
                throw new InvalidOperationException(presenter.ErrorMessage);
            }

            return presenter?.ViewModel;
        }
    }
}
