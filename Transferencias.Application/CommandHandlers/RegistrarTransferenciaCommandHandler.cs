using MediatR;
using Microsoft.Extensions.Logging;
using Transferencias.Application.Commands;
using Transferencias.Application.Dtos;
using Transferencias.Application.Interfaces;
using Transferencias.Domain.Entities;
using Transferencias.Domain.Exceptions;
using Transferencias.Domain.Entities.Repositories;

namespace Transferencias.Application.CommandHandlers
{
    public class RegistrarTransferenciaCommandHandler
        : IRequestHandler<RegistrarTransferenciaCommand, TransferenciaDto>
    {
        private readonly ITransferenciaRepository _repo;
       
        private readonly ILogger<RegistrarTransferenciaCommandHandler> _logger;

        public RegistrarTransferenciaCommandHandler(ITransferenciaRepository repo,ILogger<RegistrarTransferenciaCommandHandler> logger)
        {
            _repo = repo;          
            _logger = logger;
        }

        public async Task<TransferenciaDto> Handle(RegistrarTransferenciaCommand request,CancellationToken cancellationToken)
        {
            // IDEMPOTÊNCIA
            var existente = await _repo.ObterPorChaveIdempotenciaAsync(request.ChaveIdempotencia);
            if (existente != null)
                return Mapear(existente);

            // Domínio
            Transferencia transferencia;
            try
            {
                transferencia = Transferencia.Criar(
                    request.ChaveIdempotencia,
                    request.NumeroContaOrigem,
                    request.NumeroContaDestino,
                    request.Valor
                );
            }
            catch (DomainException ex)
            {
                return new TransferenciaDto
                {
                    CodigoErro = ex.Codigo,
                    MensagemErro = ex.Message,
                    Status = "Falha"
                };
            }

            await _repo.AdicionarAsync(transferencia);

            string operacaoId = transferencia.Id.ToString("N");

            try
            {
                // Débito
                await _mov.DebitarAsync(
                    request.NumeroContaOrigem,
                    request.Valor,
                    operacaoId);

                // Crédito
                await _mov.CreditarAsync(
                    request.NumeroContaDestino,
                    request.Valor,
                    operacaoId);

                transferencia.Concluir();
                await _repo.AtualizarAsync(transferencia);

                return Mapear(transferencia);
            }
            catch (Exception ex)
            {
                // Estorno
                try
                {
                    await _mov.CreditarAsync(
                        request.NumeroContaOrigem,
                        request.Valor,
                        operacaoId + "-estorno");
                }
                catch { }

                transferencia.Falhar("TRANSFER_FAILED", ex.Message);
                await _repo.AtualizarAsync(transferencia);

                return Mapear(transferencia);
            }
        }

        private TransferenciaDto Mapear(Transferencia t)
        {
            return new TransferenciaDto
            {
                Id = t.Id,
                NumeroContaOrigem = t.NumeroContaOrigem,
                NumeroContaDestino = t.NumeroContaDestino,
                ChaveIdempotencia = t.ChaveIdempotencia,
                Valor = t.Valor,
                Status = t.Status.ToString(),
                CodigoErro = t.CodigoErro,
                MensagemErro = t.MensagemErro,
                DataCriacao = t.DataCriacao,
                DataConclusao = t.DataConclusao
            };
        }
    }
}
