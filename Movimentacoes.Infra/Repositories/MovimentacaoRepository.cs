using System.Data;
using Dapper;
using Movimentacoes.Domain.Entities;
using Movimentacoes.Domain.Entities.Repositories;
using Movimentacoes.Domain.Enums;

namespace Movimentacoes.Infra.Repositories
{
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly IDbConnection _connection;

        public MovimentacaoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AdicionarAsync(Movimentacao movimento)
        {
            var sql = @"
                INSERT INTO Movimentacao (
                    Id,
                    NumeroConta,
                    Valor,
                    Tipo,
                    IdentificacaoRequisicao,
                    DataMovimento
                ) VALUES (
                    @Id,
                    @NumeroConta,
                    @Valor,
                    @Tipo,
                    @IdentificacaoRequisicao,
                    @DataMovimento
                );
            ";

            await _connection.ExecuteAsync(sql, new
            {
                Id = movimento.Id.ToString(),
                movimento.NumeroConta,
                movimento.Valor,
                Tipo = (int)movimento.Tipo,
                movimento.IdentificacaoRequisicao,
                DataMovimento = movimento.DataMovimento.ToString("O")
            });
        }

        public async Task<IEnumerable<Movimentacao>> ObterPorContaAsync(int numeroConta)
        {
                                        var sql = @"
                                                    SELECT 
                                    Id AS Id,
                                    NumeroConta AS NumeroConta,
                                    Valor AS Valor,
                                    Tipo AS Tipo,
                                    IdentificacaoRequisicao AS IdentificacaoRequisicao,
                                    DataMovimento AS DataMovimento
                                FROM Movimentacao
                                WHERE NumeroConta = @NumeroConta
                                ORDER BY DataMovimento;
                            ";

            var result = await _connection.QueryAsync<dynamic>(sql, new { NumeroConta = numeroConta });

            return result.Select(r =>
                new MovimentacaoBuilder().FromDatabase(
                    Guid.Parse((string)r.Id),
                    (int)r.NumeroConta,
                    (decimal)r.Valor,
                    (TipoMovimento)r.Tipo,
                    (string)r.IdentificacaoRequisicao,
                    DateTime.Parse((string)r.DataMovimento)
                )
            );


        }

        public async Task<bool> ExistePorIdentificacaoAsync(string identificacaoRequisicao)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM Movimentacao
                WHERE IdentificacaoRequisicao = @IdentificacaoRequisicao;
            ";

            var count = await _connection.ExecuteScalarAsync<int>(sql,
                new { IdentificacaoRequisicao = identificacaoRequisicao });

            return count > 0;
        }

        public async Task<decimal> ObterSaldoAsync(int numeroConta)
        {
            var sql = @"
                SELECT 
                    SUM(CASE WHEN Tipo = 1 THEN Valor ELSE -Valor END)
                FROM Movimentacao
                WHERE NumeroConta = @NumeroConta;
            ";

            var saldo = await _connection.ExecuteScalarAsync<decimal?>(sql, new { NumeroConta = numeroConta });

            return saldo ?? 0;
        }
              
        private class MovimentacaoBuilder
        {
            public Movimentacao FromDatabase(
                Guid id,
                int numeroConta,
                decimal valor,
                TipoMovimento tipo,
                string identificacaoRequisicao,
                DateTime dataMovimento)
            {
                var m = (Movimentacao?)Activator.CreateInstance(
                    typeof(Movimentacao),
                    nonPublic: true
                )!;

                typeof(Movimentacao).GetProperty("Id")!.SetValue(m, id);
                typeof(Movimentacao).GetProperty("NumeroConta")!.SetValue(m, numeroConta);
                typeof(Movimentacao).GetProperty("Valor")!.SetValue(m, valor);
                typeof(Movimentacao).GetProperty("Tipo")!.SetValue(m, tipo);
                typeof(Movimentacao).GetProperty("IdentificacaoRequisicao")!.SetValue(m, identificacaoRequisicao);
                typeof(Movimentacao).GetProperty("DataMovimento")!.SetValue(m, dataMovimento);

                return m;
            }
        }
    }
}
