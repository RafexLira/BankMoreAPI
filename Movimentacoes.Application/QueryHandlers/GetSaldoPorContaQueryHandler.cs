using MediatR;
using Movimentacoes.Application.Dtos;
using Movimentacoes.Application.Queries;
using Movimentacoes.Domain.Entities.Repositories;

namespace Movimentacoes.Application.QueryHandlers
{
    public class GetSaldoPorContaQueryHandler
        : IRequestHandler<GetSaldoPorContaQuery, SaldoDto>
    {
        private readonly IMovimentacaoRepository _repo;

        public GetSaldoPorContaQueryHandler(IMovimentacaoRepository repo)
        {
            _repo = repo;
        }

        public async Task<SaldoDto> Handle(
            GetSaldoPorContaQuery request,
            CancellationToken cancellationToken)
        {
            var saldo = await _repo.ObterSaldoAsync(request.NumeroConta);

            return new SaldoDto
            {
                NumeroConta = request.NumeroConta,
                Saldo = saldo
            };
        }
    }
}
