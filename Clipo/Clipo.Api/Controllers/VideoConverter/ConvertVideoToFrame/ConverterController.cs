using Clipo.Application.UseCases.ConvertVideoToFrame;
using Clipo.Presentation.Controller.VideoConverter;
using Microsoft.AspNetCore.Mvc;

namespace Clipo.Api.Controllers.VideoConverter.ConvertVideoToFrame
{
    [ApiController]
    [Route("video")]
    public sealed class ConverterController : ControllerBase
    {
        private readonly ConvertVideoToFrameController _useCase;

        public ConverterController(ConvertVideoToFrameController useCase)
            => _useCase = useCase;

        /// <summary>
        /// Cria uma nova conversão de vídeo para frames.
        /// </summary>
        /// <remarks>
        /// Este endpoint recebe os dados do vídeo (ou referência no storage) e cria um job de conversão.
        ///
        /// <b>Retornos possíveis:</b>
        /// - <b>201 Created</b>: Retorna o job criado (<c>ConvertVideoToFrameOutput</c>).
        /// - <b>400 Bad Request</b>: Dados inválidos ou erro de negócio.
        /// - <b>500 Internal Server Error</b>: Falha inesperada no servidor.
        /// </remarks>
        /// <param name="body">Dados necessários para criação da conversão (<c>ConvertVideoToFrameInput</c>).</param>
        /// <param name="ct">Token de cancelamento para a operação assíncrona.</param>
        /// <returns>Um resultado contendo o job criado ou informações de erro.</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(ConvertVideoToFrameOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(IFormFile file, Guid userID, CancellationToken ct)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                ConvertVideoToFrameInput body = new ConvertVideoToFrameInput(file, userID);
                ConvertVideoToFrameOutput? vm = await _useCase.ExecuteAsync(body, ct);
                return Created(string.Empty, null);
            }
            catch(Exception be)
            {
                return BadRequest(new { be.Message });
            }
        }
    }
}
