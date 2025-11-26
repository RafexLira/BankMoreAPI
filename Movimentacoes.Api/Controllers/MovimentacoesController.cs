using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movimentacoes.Application.Commands;

namespace Movimentacoes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MovimentacoesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentacoesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("credito")]
        public async Task<IActionResult> RegistrarCredito([FromBody] CreditoRequest req)
        {
            var cmd = new RegistrarMovimentoCommand
            {
                NumeroConta = req.NumeroConta,
                Valor = req.Valor,
                Tipo = "C",
                IdentificacaoRequisicao = req.IdentificacaoRequisicao
            };

            var result = await _mediator.Send(cmd);

            return Ok(result);
        }

        [HttpPost("debito")]
        public async Task<IActionResult> RegistrarDebito([FromBody] DebitoRequest req)
        {
            var cmd = new RegistrarMovimentoCommand
            {
                NumeroConta = req.NumeroConta,
                Valor = req.Valor,
                Tipo = "D",
                IdentificacaoRequisicao = req.IdentificacaoRequisicao
            };

            var result = await _mediator.Send(cmd);

            return Ok(result);
        }

        public class CreditoRequest
        {
            public int NumeroConta { get; set; }
            public decimal Valor { get; set; }
            public string IdentificacaoRequisicao { get; set; } = null!;
        }

        public class DebitoRequest
        {
            public int NumeroConta { get; set; }
            public decimal Valor { get; set; }
            public string IdentificacaoRequisicao { get; set; } = null!;
        }
    }
}
