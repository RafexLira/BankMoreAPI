using System.Data;
using Dapper;
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

        public async Task AddAsync(Transferencia transferencia, CancellationToken ct)
        {
            var sql = @"
                INSERT INTO transferencia
                (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor)
                VALUES (@Id, @IdContaOrigem, @IdContaDestino, @DataMovimento, @Valor);
            ";

            await _connection.ExecuteAsync(sql, transferencia);
        }
    }
}
