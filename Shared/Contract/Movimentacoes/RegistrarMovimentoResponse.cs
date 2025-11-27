using System;

namespace Shared.Contracts.Movimentacoes
{
    public class RegistrarMovimentoResponse
    {
        public Guid IdMovimento { get; set; }
        public int NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = null!;
        public decimal SaldoAtual { get; set; }
    }
}
