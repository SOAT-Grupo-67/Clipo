using Clipo.Application.UseCases.GetVideosByUser;
using Clipo.Presentation.Presenters.VideoConverter;

namespace Clipo.Presentation.Controller.VideoConverter
{
    public class GetVideosByUserController
    {
        private readonly IGetVideosByUserInputPort _input;
        private readonly IGetVideosByUserOutputPort _output;

        public GetVideosByUserController(IGetVideosByUserInputPort input,
                                       IGetVideosByUserOutputPort output)
        {
            _input = input;
            _output = output;
        }

        public async Task<List<GetVideosByUserOutput>?> ExecuteAsync(
            GetVideosByUserInput dto, CancellationToken ct = default)
        {
            await _input.Handle(dto, ct);

            GetVideosByUserPresenter? presenter = _output as GetVideosByUserPresenter;
            if (presenter?.ErrorMessage != null)
            {
                throw new InvalidOperationException(presenter.ErrorMessage);
            }

            return presenter?.ViewModel;
        }
    }
}
