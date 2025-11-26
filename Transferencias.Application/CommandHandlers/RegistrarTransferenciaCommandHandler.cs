using MediatR;
using Microsoft.Extensions.Logging;
using Transferencias.Application.Commands;
using Transferencias.Application.Dtos;
using Transferencias.Domain.Entities;
using Transferencias.Domain.Entities.Repositories;
using Transferencias.Domain.Exceptions;

namespace Transferencias.Application.CommandHandlers
{
    public class RegistrarTransferenciaCommandHandler
        : IRequestHandler<RegistrarTransferenciaCommand, TransferenciaDto>
    {
        private readonly ITransferenciaRepository _repo;
        private readonly ILogger<RegistrarTransferenciaCommandHandler> _logger;

        public RegistrarTransferenciaCommandHandler(
            ITransferenciaRepository repo,
            ILogger<RegistrarTransferenciaCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<TransferenciaDto> Handle(
            RegistrarTransferenciaCommand request,
            CancellationToken cancellationToken)
        {
            // ---------------------------
            // 1) IDEMPOTÊNCIA
            // ---------------------------
            var existente = await _repo.ObterPorChaveIdempotenciaAsync(request.ChaveIdempotencia);

            if (existente != null)
            {
                _logger.LogInformation(
                    "Transferência já existente para a chave {Chave}",
                    request.ChaveIdempotencia
                );

                return Mapear(existente);
            }

            // ---------------------------
            // 2) REGRA DE DOMÍNIO
            // ---------------------------
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
                _logger.LogWarning(
                    "Erro de domínio ao criar transferência: {Mensagem}",
                    ex.Message
                );

                return new TransferenciaDto
                {
                    CodigoErro = ex.Codigo,
                    MensagemErro = ex.Message,
                    Status = "Falha"
                };
            }

            // ---------------------------
            // 3) PERSISTIR (PENDENTE)
            // ---------------------------
            await _repo.AdicionarAsync(transferencia);

            // ---------------------------
            // 4) OPERAÇÃO REAL (AINDA NÃO CHAMAMOS Movimentacoes)
            // por enquanto a transferência é concluída imediatamente
            // ---------------------------
            transferencia.Concluir();
            await _repo.AtualizarAsync(transferencia);

            // ---------------------------
            // 5) RETORNO DTO
            // ---------------------------
            return Mapear(transferencia);
        }

        private TransferenciaDto Mapear(Transferencia t)
        {
            return new TransferenciaDto
            {
                Id = t.Id,
                ChaveIdempotencia = t.ChaveIdempotencia,
                NumeroContaOrigem = t.NumeroContaOrigem,
                NumeroContaDestino = t.NumeroContaDestino,
                Valor = t.Valor,
                CodigoErro = t.CodigoErro,
                MensagemErro = t.MensagemErro,
                Status = t.Status.ToString(),
                DataCriacao = t.DataCriacao,
                DataConclusao = t.DataConclusao
            };
        }
    }
}
