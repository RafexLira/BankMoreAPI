using System;
using Transferencias.Domain.Enums;
using Transferencias.Domain.Exceptions;

namespace Transferencias.Domain.Entities
{
    public class Transferencia
    {
        public Guid Id { get; private set; }
        public string ChaveIdempotencia { get; private set; }
        public int NumeroContaOrigem { get; private set; }
        public int NumeroContaDestino { get; private set; }
        public decimal Valor { get; private set; }
        public TransferenciaStatus Status { get; private set; }
        public string? CodigoErro { get; private set; }
        public string? MensagemErro { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataConclusao { get; private set; }

        private Transferencia() { }

        private Transferencia(string chave, int origem, int destino, decimal valor)
        {
            Id = Guid.NewGuid();
            DataCriacao = DateTime.UtcNow;
            Status = TransferenciaStatus.Pendente;

            Validar(chave, origem, destino, valor);

            ChaveIdempotencia = chave;
            NumeroContaOrigem = origem;
            NumeroContaDestino = destino;
            Valor = valor;
        }

        public static Transferencia Criar(string chave, int origem, int destino, decimal valor)
            => new Transferencia(chave, origem, destino, valor);

        private void Validar(string chave, int origem, int destino, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(chave))
                throw new DomainException("Chave inválida.", "INVALID_IDEMPOTENCY");

            if (origem <= 0)
                throw new DomainException("Conta origem inválida.", "INVALID_ORIGIN");

            if (destino <= 0)
                throw new DomainException("Conta destino inválida.", "INVALID_DESTINATION");

            if (origem == destino)
                throw new DomainException("Contas iguais.", "SAME_ACCOUNT");

            if (valor <= 0)
                throw new DomainException("Valor inválido.", "INVALID_VALUE");
        }

        public void Concluir()
        {
            Status = TransferenciaStatus.Concluida;
            DataConclusao = DateTime.UtcNow;
            CodigoErro = null;
            MensagemErro = null;
        }

        public void Falhar(string codigo, string mensagem)
        {
            Status = TransferenciaStatus.Falha;
            DataConclusao = DateTime.UtcNow;
            CodigoErro = codigo;
            MensagemErro = mensagem;
        }
    }
}
