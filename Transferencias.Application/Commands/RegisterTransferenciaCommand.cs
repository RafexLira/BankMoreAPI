using MediatR;

namespace Transferencias.Application.Commands
{
    public class RegisterTransferenciaCommand : IRequest<string>
    {
        public int NumeroContaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public string ChaveIdempotencia { get; set; } = string.Empty;
    }
}
