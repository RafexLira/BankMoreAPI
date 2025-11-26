using System;
using Movimentacoes.Domain.Enums;
using Movimentacoes.Domain.Exceptions;

namespace Movimentacoes.Domain.Entities
{
    public class Movimentacao
    {
        public Guid Id { get; private set; }
        public int NumeroConta { get; private set; }
        public decimal Valor { get; private set; }
        public TipoMovimento Tipo { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public string IdentificacaoRequisicao { get; private set; } = null!;

        private Movimentacao() { }

        private Movimentacao(
            int numeroConta,
            decimal valor,
            TipoMovimento tipo,
            string identificacaoRequisicao)
        {
            Id = Guid.NewGuid();
            DataMovimento = DateTime.UtcNow;

            DefinirDados(numeroConta, valor, tipo, identificacaoRequisicao);
        }

        public static Movimentacao CriarCredito(
            int numeroConta,
            decimal valor,
            string identificacaoRequisicao)
            => new Movimentacao(numeroConta, valor, TipoMovimento.Credito, identificacaoRequisicao);

        public static Movimentacao CriarDebito(
            int numeroConta,
            decimal valor,
            string identificacaoRequisicao)
            => new Movimentacao(numeroConta, valor, TipoMovimento.Debito, identificacaoRequisicao);

        private void DefinirDados(
            int numeroConta,
            decimal valor,
            TipoMovimento tipo,
            string identificacaoRequisicao)
        {
            if (numeroConta <= 0)
                throw new DomainException("Número da conta inválido.", "INVALID_ACCOUNT");

            if (valor <= 0)
                throw new DomainException("Valor deve ser maior que zero.", "INVALID_VALUE");

            if (string.IsNullOrWhiteSpace(identificacaoRequisicao))
                throw new DomainException("Identificação da requisição é obrigatória.", "INVALID_REQUEST_ID");

            NumeroConta = numeroConta;
            Valor = valor;
            Tipo = tipo;
            IdentificacaoRequisicao = identificacaoRequisicao;
        }
    }
}
