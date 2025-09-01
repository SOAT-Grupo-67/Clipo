using System.ComponentModel.DataAnnotations;

namespace Clipo.Application.UseCases.GetVideosByUser
{
    public sealed record GetVideosByUserInput(
        [property: Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        Guid UserId);
}
