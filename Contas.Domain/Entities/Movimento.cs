using Contas.Domain.Exceptions;

namespace Contas.Domain.Entities
{
    public class Movimento
    {
        public string Id { get; private set; }
        public string IdContaCorrente { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public string TipoMovimento { get; private set; } // "DEBITO" ou "CREDITO"
        public decimal Valor { get; private set; }

        protected Movimento() { }

        public Movimento(string idContaCorrente, string tipoMovimento, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(idContaCorrente))
                throw new DomainException("Conta inválida.");

            if (tipoMovimento != "DEBITO" && tipoMovimento != "CREDITO")
                throw new DomainException("Tipo de movimento inválido.");

            if (valor <= 0)
                throw new DomainException("Valor inválido.");

            Id = Guid.NewGuid().ToString();
            IdContaCorrente = idContaCorrente;
            TipoMovimento = tipoMovimento;
            Valor = valor;
            DataMovimento = DateTime.UtcNow;
        }
    }
}
