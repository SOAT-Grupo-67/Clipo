using Clipo.Application.UseCases.GetVideoStatus;
using Clipo.Presentation.Presenters.VideoConverter;

namespace Clipo.Presentation.Controller.VideoConverter
{
    public class GetVideoStatusController
    {
        private readonly IGetVideoStatusInputPort _input;
        private readonly IGetVideoStatusOutputPort _output;

        public GetVideoStatusController(IGetVideoStatusInputPort input,
                                       IGetVideoStatusOutputPort output)
        {
            _input = input;
            _output = output;
        }

        public async Task<GetVideoStatusOutput?> ExecuteAsync(
            GetVideoStatusInput dto, CancellationToken ct = default)
        {
            await _input.Handle(dto, ct);

            GetVideoStatusPresenter? presenter = _output as GetVideoStatusPresenter;
            if (presenter?.ErrorMessage != null)
            {
                throw new InvalidOperationException(presenter.ErrorMessage);
            }

            return presenter?.ViewModel;
        }
    }
}
