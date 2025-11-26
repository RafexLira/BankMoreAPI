using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;
using Transferencias.Domain.Entities;
using Transferencias.Domain.Entities.Repositories;

namespace Transferencias.Infra.Repositories
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly IDbConnection _connection;

        public TransferenciaRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Transferencia?> ObterPorIdAsync(Guid id)
        {
            var sql = @"SELECT * FROM Transferencia WHERE Id = @Id";

            return await _connection.QueryFirstOrDefaultAsync<Transferencia>(sql, new
            {
                Id = id.ToString()
            });
        }

        public async Task<Transferencia?> ObterPorChaveIdempotenciaAsync(string chave)
        {
            var sql = @"SELECT * FROM Transferencia WHERE ChaveIdempotencia = @Chave";

            return await _connection.QueryFirstOrDefaultAsync<Transferencia>(sql, new
            {
                Chave = chave
            });
        }

        public async Task AdicionarAsync(Transferencia t)
        {
            var sql = @"
                INSERT INTO Transferencia (
                    Id,
                    ChaveIdempotencia,
                    NumeroContaOrigem,
                    NumeroContaDestino,
                    Valor,
                    Status,
                    CodigoErro,
                    MensagemErro,
                    DataCriacao,
                    DataConclusao
                )
                VALUES (
                    @Id,
                    @ChaveIdempotencia,
                    @NumeroContaOrigem,
                    @NumeroContaDestino,
                    @Valor,
                    @Status,
                    @CodigoErro,
                    @MensagemErro,
                    @DataCriacao,
                    @DataConclusao
                );
            ";

            await _connection.ExecuteAsync(sql, new
            {
                Id = t.Id.ToString(),
                t.ChaveIdempotencia,
                t.NumeroContaOrigem,
                t.NumeroContaDestino,
                t.Valor,
                Status = (int)t.Status,
                t.CodigoErro,
                t.MensagemErro,
                DataCriacao = t.DataCriacao.ToString("o"),
                DataConclusao = t.DataConclusao?.ToString("o")
            });
        }

        public async Task AtualizarAsync(Transferencia t)
        {
            var sql = @"
                UPDATE Transferencia SET
                    Status = @Status,
                    CodigoErro = @CodigoErro,
                    MensagemErro = @MensagemErro,
                    DataConclusao = @DataConclusao
                WHERE Id = @Id;
            ";

            await _connection.ExecuteAsync(sql, new
            {
                Id = t.Id.ToString(),
                Status = (int)t.Status,
                t.CodigoErro,
                t.MensagemErro,
                DataConclusao = t.DataConclusao?.ToString("o")
            });
        }
    }
}
