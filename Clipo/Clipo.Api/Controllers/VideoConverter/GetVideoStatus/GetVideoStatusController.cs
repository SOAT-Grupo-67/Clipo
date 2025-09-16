using Clipo.Application.UseCases.GetVideoStatus;
using Microsoft.AspNetCore.Mvc;

namespace Clipo.Api.Controllers.VideoConverter.GetVideoStatus
{
    [ApiController]
    [Route("video")]
    public sealed class GetVideoStatusController : ControllerBase
    {
        private readonly Presentation.Controller.VideoConverter.GetVideoStatusController _useCase;

        public GetVideoStatusController(Presentation.Controller.VideoConverter.GetVideoStatusController useCase)
            => _useCase = useCase;

        /// <summary>
        /// Busca o status de processamento de um vídeo específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna o status atual de processamento de um vídeo pelo seu ID.
        ///
        /// <b>Retornos possíveis:</b>
        /// - <b>200 OK</b>: Retorna o status do vídeo.
        /// - <b>400 Bad Request</b>: ID do vídeo inválido.
        /// - <b>404 Not Found</b>: Vídeo não encontrado.
        /// - <b>500 Internal Server Error</b>: Falha inesperada no servidor.
        /// </remarks>
        /// <param name="id">ID do vídeo para buscar o status.</param>
        /// <param name="ct">Token de cancelamento para a operação assíncrona.</param>
        /// <returns>Status do vídeo.</returns>
        [HttpGet("{id}/status")]
        [ProducesResponseType(typeof(GetVideoStatusOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVideoStatusAsync(long id, CancellationToken ct)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                GetVideoStatusInput input = new(id);
                GetVideoStatusOutput? result = await _useCase.ExecuteAsync(input, ct);

                if(result == null)
                {
                    return NotFound(new { Message = "Vídeo não encontrado." });
                }

                return Ok(result);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch(Exception)
            {
                return StatusCode(500, new { Message = "Erro interno do servidor." });
            }
        }
    }
}
