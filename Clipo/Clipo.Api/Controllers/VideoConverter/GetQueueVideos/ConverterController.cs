using Microsoft.AspNetCore.Mvc;

namespace Clipo.Api.Controllers.VideoConverter.GetQueueVideos
{
    [ApiController]
    [Route("videoconverter/queue")]
    public sealed class ConverterController : ControllerBase
    {
        //private readonly IGetQueueVideosUseCase _useCase;

        //public ConverterController(IGetQueueVideosUseCase useCase)
        //    => _useCase = useCase;

        ///// <summary>
        ///// Lista vídeos na fila de processamento.
        ///// </summary>
        ///// <remarks>
        ///// Retorna itens atualmente na fila (pendentes/processing) e/ou métricas de fila.
        /////
        ///// <b>Retornos possíveis:</b>
        ///// - <b>200 OK</b>: Retorna a lista ou visão da fila (<c>PagedResult&lt;QueuedVideoOutput&gt;</c>).
        ///// - <b>400 Bad Request</b>: Parâmetros inválidos.
        ///// - <b>500 Internal Server Error</b>: Falha inesperada no servidor.
        ///// </remarks>
        ///// <param name="query">Parâmetros de filtro/paginação (<c>GetQueueVideosInput</c>).</param>
        ///// <param name="ct">Token de cancelamento.</param>
        ///// <returns>Visão paginada dos itens na fila.</returns>
        //[Authorize]
        //[HttpGet("list")]
        //[ProducesResponseType(typeof(PagedResult<QueuedVideoOutput>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> ListAsync([FromQuery] GetQueueVideosInput query, CancellationToken ct)
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
