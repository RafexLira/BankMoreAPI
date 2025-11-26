using System;

namespace Transferencias.Application.Dtos
{
    public class TransferenciaDto
    {
        public Guid Id { get; set; }
        public string ChaveIdempotencia { get; set; } = null!;
        public int NumeroContaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; } = null!;
        public string? CodigoErro { get; set; }
        public string? MensagemErro { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
    }
}
