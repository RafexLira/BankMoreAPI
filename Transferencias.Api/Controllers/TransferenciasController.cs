using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transferencias.Application.Commands;

namespace Transferencias.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "MicroserviceScheme")]

    public class TransferenciasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransferenciasController> _logger;

        public TransferenciasController(IMediator mediator, ILogger<TransferenciasController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegisterTransferenciaCommand command)
        {
            _logger.LogInformation("Recebendo solicitação de transferência. Origem={Origem}, Destino={Destino}, Valor={Valor}",
                command.NumeroContaOrigem, command.NumeroContaDestino, command.Valor);

            try
            {
                var id = await _mediator.Send(command);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar transferência.");
                return StatusCode(500, "Erro ao registrar transferência.");
            }
        }
    }
}
