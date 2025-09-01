using System.ComponentModel.DataAnnotations;

namespace Clipo.Application.UseCases.GetVideoStatus
{
    public sealed record GetVideoStatusInput(
        [property: Required(ErrorMessage = "O ID do vídeo é obrigatório.")]
        long VideoId);
}
