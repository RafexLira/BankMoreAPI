namespace Transferencias.Application.ReadModels
{
    public class TransferenciaReadModel
    {
        public string Id { get; set; } = "";
        public int NumeroContaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
    }
}
