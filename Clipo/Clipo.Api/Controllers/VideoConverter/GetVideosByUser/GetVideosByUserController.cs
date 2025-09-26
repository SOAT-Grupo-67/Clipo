using Clipo.Application.UseCases.GetVideosByUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clipo.Api.Controllers.VideoConverter.GetVideosByUser
{
	[Authorize]
	[ApiController]
    [Route("video")]
    public sealed class GetVideosByUserController : ControllerBase
    {
        private readonly Presentation.Controller.VideoConverter.GetVideosByUserController _useCase;

        public GetVideosByUserController(Presentation.Controller.VideoConverter.GetVideosByUserController useCase)
            => _useCase = useCase;

        /// <summary>
        /// Lista todos os vídeos de um usuário específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna todos os vídeos associados ao usuário especificado no token.
        ///
        /// <b>Retornos possíveis:</b>
        /// - <b>200 OK</b>: Retorna a lista de vídeos do usuário.
        /// - <b>400 Bad Request</b>: ID do usuário inválido.
        /// - <b>404 Not Found</b>: Nenhum vídeo encontrado para o usuário.
        /// - <b>500 Internal Server Error</b>: Falha inesperada no servidor.
        /// </remarks>
        /// <param name="ct">Token de cancelamento para a operação assíncrona.</param>
        /// <returns>Lista de vídeos do usuário.</returns>
        [HttpGet("user")]
        [ProducesResponseType(typeof(List<GetVideosByUserOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVideosByUserAsync(CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
				var userId = User.FindFirst("userId")?.Value;
                if (userId == null) return Unauthorized();
				GetVideosByUserInput input = new(Guid.Parse(userId));
                var result = await _useCase.ExecuteAsync(input, ct);

                if (result == null || !result.Any())
                {
                    return NotFound(new { Message = "Nenhum vídeo encontrado para este usuário." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno do servidor." });
            }
        }
    }
}
