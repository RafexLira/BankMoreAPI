using MediatR;
using Transferencias.Application.Commands;
using Transferencias.Application.ReadModels;
using Transferencias.Domain.Entities;
using Transferencias.Domain.Entities.Repositories;

namespace Transferencias.Application.CommandHandlers
{
    public class RegisterTransferenciaCommandHandler
        : IRequestHandler<RegisterTransferenciaCommand, string>
    {
        private readonly ITransferenciaRepository _repoTransferencias;
        private readonly IIdempotenciaRepository _repoIdempotencia;

        public RegisterTransferenciaCommandHandler(
            ITransferenciaRepository repoTransferencias,
            IIdempotenciaRepository repoIdempotencia)
        {
            _repoTransferencias = repoTransferencias;
            _repoIdempotencia = repoIdempotencia;
        }

        public async Task<string> Handle(RegisterTransferenciaCommand request, CancellationToken ct)
        {
            // Idempotência
            var idem = await _repoIdempotencia.GetByChaveAsync(request.ChaveIdempotencia, ct);
            if (idem != null)
                return idem.Resultado;

            // Criar entidade
            var transferencia = new Transferencia(
                request.NumeroContaOrigem.ToString(),
                request.NumeroContaDestino.ToString(),
                request.Valor
            );

            // Persistir transferência
            await _repoTransferencias.AddAsync(transferencia, ct);

            // Salvar idempotência
            var idemNovo = new Idempotencia(
                request.ChaveIdempotencia,
                System.Text.Json.JsonSerializer.Serialize(request),
                transferencia.Id
            );

            await _repoIdempotencia.AddAsync(idemNovo, ct);

            return transferencia.Id;
        }
    }
}
