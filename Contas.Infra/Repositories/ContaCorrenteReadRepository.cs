using Contas.Application.ReadModels;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;
using Dapper;
using System.Data;

namespace Contas.Infra.Repositories
{
    public class ContaCorrenteReadRepository : IContaCorrenteReadRepository
    {
        private readonly IDbConnection _connection;

        public ContaCorrenteReadRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ContaCorrente?> GetByNumeroAsync(int numero, CancellationToken ct)
        {
            var sql = @"
        SELECT 
            idcontacorrente AS Id,
            numero AS Numero,
            nome AS Nome,
            cpf AS CPF,
            ativo AS Ativo,
            senha AS SenhaHash,
            salt AS Salt
        FROM contacorrente
        WHERE numero = @numero
    ";

            return await _connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { numero });
        }




        public async Task<bool> ExistsNumeroAsync(int numero, CancellationToken ct = default)
        {
            var sql = "SELECT COUNT(1) FROM contacorrente WHERE numero = @numero;";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new { numero });
            return count > 0;
        }

    
    }
}
