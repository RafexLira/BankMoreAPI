using Contas.Application.Commands;
using Contas.Application.Security;
using Contas.Domain.Entities.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contas.Application.CommandHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string?>
    {
        private readonly IContaCorrenteReadRepository _readRepo;
        private readonly TokenService _tokenService;

        public LoginCommandHandler(
            IContaCorrenteReadRepository readRepo,
            TokenService tokenService)
        {
            _readRepo = readRepo;
            _tokenService = tokenService;
        }

        public async Task<string?> Handle(LoginCommand request, CancellationToken ct)
        {
      
            var conta = await _readRepo.GetByNumeroAsync(request.Numero, ct);
            if (conta == null)
                return null;

      
            if (!string.Equals(conta.CPF, request.Cpf, StringComparison.Ordinal))
                return null;

            if (!PasswordHasher.Verify(request.Senha, conta.SenhaHash, conta.Salt))
                return null;

       
            var token = _tokenService.GenerateToken(Guid.Parse(conta.Id));

            return token;
        }
    }
}
