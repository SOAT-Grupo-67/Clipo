using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Clipo.Application.UseCases.ConvertVideoToFrame
{
    public sealed record ConvertVideoToFrameInput(
        [property: Required(ErrorMessage = "O arquivo de vídeo é obrigatório.")]
        IFormFile File);
}
