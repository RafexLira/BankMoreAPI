using MediatR;

namespace Contas.Application.Commands
{
    public class RegistrarMovimentoCommand : IRequest<Unit>
    {
        public string IdContaCorrente { get; set; } = default!;
        public string Tipo { get; set; } = default!;
        public decimal Valor { get; set; }
    }
}
