namespace Shared.Contracts.Movimentacoes
{
    public class RegistrarMovimentoRequest
    {
        public int NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = null!;
        public string IdentificacaoRequisicao { get; set; } = null!;
    }
}
