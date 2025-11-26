using MediatR;
using Movimentacoes.Application.Dtos;

namespace Movimentacoes.Application.Commands
{
    public class RegistrarMovimentoCommand : IRequest<MovimentacaoDto>
    {
        public int NumeroConta { get; set; }
        public decimal Valor { get; set; }

        // "C" = crédito, "D" = débito
        public string Tipo { get; set; } = null!;

        public string IdentificacaoRequisicao { get; set; } = null!;
    }
}
