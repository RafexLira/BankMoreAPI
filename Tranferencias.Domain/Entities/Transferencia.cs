namespace Transferencias.Domain.Entities
{
    public class Transferencia
    {
        public string Id { get; private set; }
        public string IdContaOrigem { get; private set; }
        public string IdContaDestino { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public decimal Valor { get; private set; }

        protected Transferencia() { }

        public Transferencia(
            string idContaOrigem,
            string idContaDestino,
            decimal valor)
        {
            if (idContaOrigem == idContaDestino)
                throw new Exception("Conta origem e destino não podem ser iguais.");

            if (valor <= 0)
                throw new Exception("Valor da transferência deve ser maior que zero.");

            Id = Guid.NewGuid().ToString();
            IdContaOrigem = idContaOrigem;
            IdContaDestino = idContaDestino;
            Valor = valor;
            DataMovimento = DateTime.UtcNow;
        }
    }
}
