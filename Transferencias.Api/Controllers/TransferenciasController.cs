using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transferencias.Application.Commands;
using Transferencias.Application.Queries;

namespace Transferencias.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferenciasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarTransferenciaCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Status == "Falha")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var result = await _mediator.Send(new GetTransferenciaByIdQuery { Id = id });

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
