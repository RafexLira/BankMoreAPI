using System;

namespace Movimentacoes.Application.ReadModels
{
    public class MovimentacaoReadModel
    {
        public Guid Id { get; set; }
        public int NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = null!;
        public DateTime DataMovimento { get; set; }
        public string IdentificacaoRequisicao { get; set; } = null!;
    }
}
