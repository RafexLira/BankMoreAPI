using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movimentacoes.Domain.Entities.Repositories
{
    public interface IMovimentacaoRepository
    {
        Task AdicionarAsync(Movimentacao movimento);
        Task<IEnumerable<Movimentacao>> ObterPorContaAsync(int numeroConta);
        Task<bool> ExistePorIdentificacaoAsync(string identificacaoRequisicao);
        Task<decimal> ObterSaldoAsync(int numeroConta);
    }
}
