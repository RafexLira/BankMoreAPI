using MediatR;
using Contas.Application.Commands;
using Contas.Domain.Entities.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Contas.Application.CommandHandlers
{
    public class InativarContaCommandHandler : IRequestHandler<InativarContaCommand, bool>
    {
        private readonly IContaCorrenteWriteRepository _writeRepo;
        private readonly IContaCorrenteReadRepository _readRepo;

        public InativarContaCommandHandler(
            IContaCorrenteWriteRepository writeRepo,
            IContaCorrenteReadRepository readRepo)
        {
            _writeRepo = writeRepo;
            _readRepo = readRepo;
        }

        public async Task<bool> Handle(InativarContaCommand request, CancellationToken ct)
        {
            var conta = await _readRepo.GetByNumeroAsync(request.Numero, ct);

            if (conta == null)
                throw new Exception("INVALID_ACCOUNT");

            // --- VALIDAÇÃO MANUAL DO HASH USANDO O SALT SALVO ---
            var combined = request.Senha + conta.Salt;
            var combinedBytes = Encoding.UTF8.GetBytes(combined);

            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(combinedBytes);
            var calculatedHash = Convert.ToBase64String(hashBytes);

            if (calculatedHash != conta.SenhaHash)
                throw new Exception("USER_UNAUTHORIZED");

            // --- DESATIVAR CONTA ---
            conta.Desativar();

            await _writeRepo.UpdateAsync(conta, ct);

            return true;
        }
    }
}
