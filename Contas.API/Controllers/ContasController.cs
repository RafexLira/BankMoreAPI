using Contas.Application.Commands;
using Contas.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Contas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ContasController> _logger;

        public ContasController(IMediator mediator, ILogger<ContasController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarConta([FromBody] RegisterContaCorrenteCommand command)
        {
            _logger.LogInformation("Iniciando registro de conta. Numero={Numero}, Nome={Nome}, CPF={Cpf}",command.Numero, command.Nome, command.Cpf);

            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(ObterPorNumero), new { numero = command.Numero }, new { id = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar conta. Numero={Numero}, Nome={Nome}, CPF={Cpf}", command.Numero, command.Nome, command.Cpf);
                throw;
            }

          
        }

        [HttpGet("{numero:int}")]
        [Authorize]
        public async Task<IActionResult> ObterPorNumero(int numero)
        {

            _logger.LogInformation("Consultando conta. Numero={Numero}", numero);

            try
            {
                var query = new GetContaCorrenteByNumeroQuery(numero);
                var conta = await _mediator.Send(query);

                if (conta == null)
                    return NotFound();

                return Ok(conta);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar conta. Numero={Numero}", numero);
                throw;
            }
         
        }

        [HttpPost("inativar")]
        [Authorize]
        public async Task<IActionResult> InativarConta([FromBody] InativarContaCommand request)
        {

            _logger.LogInformation("Solicitação para inativar conta. Numero={Numero}", request.Numero);

            try
            {
                var command = new InativarContaCommand(request.Numero, request.Senha);

                var result = await _mediator.Send(command);

                return NoContent();

            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Erro ao inativar conta. Numero={Numero}", request.Numero);
                throw;
            }

        }
        [HttpPost("movimentar")]
        [Authorize]
        public async Task<IActionResult> RegistrarMovimento([FromBody] RegistrarMovimentoCommand command)
        {
            _logger.LogInformation(
                "Registrando movimento. Conta={Conta}, Tipo={Tipo}, Valor={Valor}", command.IdContaCorrente, command.Tipo, command.Valor );

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Erro ao registrar movimento. Conta={Conta}, Tipo={Tipo}, Valor={Valor}", command.IdContaCorrente,command.Tipo,command.Valor);

                return StatusCode(500, "Erro ao registrar movimento.");
            }
        }

        //[HttpGet("teste-token")]
        //[Authorize]
        //public IActionResult TesteToken()
        //{
        //    return Ok(new { message = "Token recebido e válido!" });
        //}

    }
}
