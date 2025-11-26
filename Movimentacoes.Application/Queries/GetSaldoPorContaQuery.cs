using MediatR;
using Movimentacoes.Application.Dtos;

namespace Movimentacoes.Application.Queries
{
    public class GetSaldoPorContaQuery : IRequest<SaldoDto>
    {
        public int NumeroConta { get; set; }
    }
}
