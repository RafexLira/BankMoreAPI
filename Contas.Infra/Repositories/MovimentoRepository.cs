using System.Data;
using Dapper;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;

namespace Contas.Infra.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDbConnection _connection;

        public MovimentoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AddAsync(Movimento movimento, CancellationToken ct = default)
        {
            var sql = @"
                INSERT INTO movimento
                (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                VALUES (@Id, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor);
            ";

            await _connection.ExecuteAsync(sql, movimento);
        }

        public async Task<IEnumerable<Movimento>> GetByContaAsync(string idContaCorrente, CancellationToken ct = default)
        {
            var sql = @"
                SELECT 
                    idmovimento AS Id,
                    idcontacorrente AS IdContaCorrente,
                    datamovimento AS DataMovimento,
                    tipomovimento AS TipoMovimento,
                    valor AS Valor
                FROM movimento
                WHERE idcontacorrente = @id
                ORDER BY datamovimento DESC;
            ";

            return await _connection.QueryAsync<Movimento>(sql, new { id = idContaCorrente });
        }
    }
}
