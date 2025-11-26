using System;

namespace Transferencias.Application.ReadModels
{
    public class TransferenciaReadModel
    {
        public Guid Id { get; set; }
        public string ChaveIdempotencia { get; set; } = null!;
        public int NumeroContaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; } = null!;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
    }
}
