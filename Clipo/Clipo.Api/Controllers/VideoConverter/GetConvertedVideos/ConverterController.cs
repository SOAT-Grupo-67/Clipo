using Microsoft.AspNetCore.Mvc;

namespace Clipo.Api.Controllers.VideoConverter.GetConvertedVideos
{
    [ApiController]
    [Route("video")]
    public sealed class ConverterController : ControllerBase
    {
        //private readonly IGetConvertedVideosUseCase _useCase;

        //public ConverterController(IGetConvertedVideosUseCase useCase)
        //    => _useCase = useCase;

        ///// <summary>
        ///// Lista vídeos já convertidos do usuário.
        ///// </summary>
        ///// <remarks>
        ///// Retorna uma lista paginada de vídeos convertidos para o usuário autenticado (ou filtrado).
        /////
        ///// <b>Retornos possíveis:</b>
        ///// - <b>200 OK</b>: Retorna a lista de vídeos convertidos (<c>PagedResult&lt;ConvertedVideoOutput&gt;</c>).
        ///// - <b>400 Bad Request</b>: Parâmetros inválidos.
        ///// - <b>500 Internal Server Error</b>: Falha inesperada no servidor.
        ///// </remarks>
        ///// <param name="query">Parâmetros de paginação/filtragem (<c>GetConvertedVideosInput</c>).</param>
        ///// <param name="ct">Token de cancelamento.</param>
        ///// <returns>Lista paginada de vídeos convertidos.</returns>
        //[Authorize]
        //[HttpGet("list")]
        //[ProducesResponseType(typeof(PagedResult<ConvertVideoToFrameOutput>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> ListAsync([FromQuery] GetConvertedVideosInput query, CancellationToken ct)
        //{
        //    if(!ModelState.IsValid) return BadRequest(ModelState);

        //    try
        //    {
        //        var vm = await _useCase.ExecuteAsync(query, ct);
        //        return Ok(vm);
        //    }
        //    catch(Exception be)
        //    {
        //        return BadRequest(new { be.Message });
        //    }
        //}
    }
}
