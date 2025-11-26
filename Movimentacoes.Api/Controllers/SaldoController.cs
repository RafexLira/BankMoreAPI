using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movimentacoes.Application.Queries;

namespace Movimentacoes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SaldoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SaldoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{numeroConta}")]
        public async Task<IActionResult> ObterSaldo(int numeroConta)
        {
            var result = await _mediator.Send(new GetSaldoPorContaQuery
            {
                NumeroConta = numeroConta
            });

            return Ok(result);
        }
    }
}
