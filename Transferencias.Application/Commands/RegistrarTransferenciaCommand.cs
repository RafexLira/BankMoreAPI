using MediatR;
using Transferencias.Application.Dtos;

namespace Transferencias.Application.Commands
{
    public class RegistrarTransferenciaCommand : IRequest<TransferenciaDto>
    {
        public string ChaveIdempotencia { get; set; } = null!;
        public int NumeroContaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}
