using Contas.Application.Commands;
using Contas.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza login e retorna um JWT.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Senha))
                return BadRequest(new { message = "Dados inválidos." });
           
            var token = await _mediator.Send(command);

            if (token == null)
                return Unauthorized(new { message = "Conta ou senha inválidos." });

            return Ok(new { accessToken = token });
        }
    }
}
