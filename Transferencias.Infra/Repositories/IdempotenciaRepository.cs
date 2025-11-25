using System.Data;
using Dapper;
using Transferencias.Domain.Entities;
using Transferencias.Domain.Entities.Repositories;

namespace Transferencias.Infra.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IDbConnection _connection;

        public IdempotenciaRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Idempotencia?> GetByChaveAsync(string chave, CancellationToken ct)
        {
            var sql = @"
                SELECT 
                    chave_idempotencia AS Chave,
                    requisicao AS Requisicao,
                    resultado AS Resultado
                FROM idempotencia
                WHERE chave_idempotencia = @chave";

            return await _connection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { chave });
        }

        public async Task AddAsync(Idempotencia idem, CancellationToken ct)
        {
            var sql = @"
                INSERT INTO idempotencia
                (chave_idempotencia, requisicao, resultado)
                VALUES (@Chave, @Requisicao, @Resultado);
            ";

            await _connection.ExecuteAsync(sql, idem);
        }
    }
}
