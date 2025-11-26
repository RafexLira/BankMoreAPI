using MediatR;
using Microsoft.Extensions.Logging;
using Movimentacoes.Application.Commands;
using Movimentacoes.Application.Dtos;
using Movimentacoes.Domain.Entities;
using Movimentacoes.Domain.Entities.Repositories;
using Movimentacoes.Domain.Enums;
using Movimentacoes.Domain.Exceptions;

namespace Movimentacoes.Application.CommandHandlers
{
    public class RegistrarMovimentoCommandHandler
        : IRequestHandler<RegistrarMovimentoCommand, MovimentacaoDto>
    {
        private readonly IMovimentacaoRepository _repo;
        private readonly ILogger<RegistrarMovimentoCommandHandler> _logger;

        public RegistrarMovimentoCommandHandler(
            IMovimentacaoRepository repo,
            ILogger<RegistrarMovimentoCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<MovimentacaoDto> Handle(
            RegistrarMovimentoCommand request,
            CancellationToken cancellationToken)
        {
            // Idempotência básica: não registrar se já existir essa identificação
            var jaExiste = await _repo.ExistePorIdentificacaoAsync(request.IdentificacaoRequisicao);
            if (jaExiste)
            {
                _logger.LogInformation(
                    "Movimento já existente para IdentificacaoRequisicao={IdReq}",
                    request.IdentificacaoRequisicao);

                // Busca saldo atual e retorna DTO "vazio" de criação
                var saldoAtual = await _repo.ObterSaldoAsync(request.NumeroConta);

                return new MovimentacaoDto
                {
                    Id = Guid.Empty,
                    NumeroConta = request.NumeroConta,
                    Valor = 0,
                    Tipo = request.Tipo,
                    DataMovimento = DateTime.UtcNow,
                    IdentificacaoRequisicao = request.IdentificacaoRequisicao,
                    SaldoAtual = saldoAtual
                };
            }

            Movimentacao mov;

            try
            {
                var tipo = request.Tipo?.ToUpperInvariant();

                if (tipo == "C")
                {
                    mov = Movimentacao.CriarCredito(
                        request.NumeroConta,
                        request.Valor,
                        request.IdentificacaoRequisicao);
                }
                else if (tipo == "D")
                {
                    mov = Movimentacao.CriarDebito(
                        request.NumeroConta,
                        request.Valor,
                        request.IdentificacaoRequisicao);
                }
                else
                {
                    throw new DomainException("Tipo de movimento inválido. Use 'C' ou 'D'.", "INVALID_TYPE");
                }
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("Erro de domínio ao registrar movimento: {Msg}", ex.Message);

                // Em Movimentações, erro de domínio normalmente vira 400 na API
                throw;
            }

            await _repo.AdicionarAsync(mov);

            var saldo = await _repo.ObterSaldoAsync(request.NumeroConta);

            return new MovimentacaoDto
            {
                Id = mov.Id,
                NumeroConta = mov.NumeroConta,
                Valor = mov.Valor,
                Tipo = mov.Tipo.ToString(),
                DataMovimento = mov.DataMovimento,
                IdentificacaoRequisicao = mov.IdentificacaoRequisicao,
                SaldoAtual = saldo
            };
        }
    }
}
