using System.Data;
using Dapper;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;

namespace Contas.Infra.Repositories
{
    public class ContaCorrenteWriteRepository : IContaCorrenteWriteRepository
    {
        private readonly IDbConnection _connection;

        public ContaCorrenteWriteRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AddAsync(ContaCorrente conta, CancellationToken ct = default)
        {
            var sql = @"
                INSERT INTO contacorrente
                (idcontacorrente, numero, nome, cpf, ativo, senha, salt)
                VALUES (@Id, @Numero, @Nome, @CPF, @Ativo, @SenhaHash, @Salt);
            ";

            var parameters = new
            {
                Id = conta.Id,
                Numero = conta.Numero,
                Nome = conta.Nome,
                CPF = conta.CPF,
                Ativo = conta.Ativo ? 1 : 0,
                SenhaHash = conta.SenhaHash,
                Salt = conta.Salt
            };

            // breakpoint aqui se for preciso depurar: verificar 'parameters'
            await _connection.ExecuteAsync(sql, parameters);
        }

        public async Task UpdateAsync(ContaCorrente conta, CancellationToken ct = default)
        {
            var sql = @"
                UPDATE contacorrente
                SET 
                    nome = @Nome,
                    cpf = @CPF,
                    ativo = @Ativo,
                    senha = @SenhaHash,
                    salt = @Salt
                WHERE idcontacorrente = @Id;
            ";

            var parameters = new
            {
                Nome = conta.Nome,
                CPF = conta.CPF,
                Ativo = conta.Ativo ? 1 : 0,
                SenhaHash = conta.SenhaHash,
                Salt = conta.Salt,
                Id = conta.Id
            };

            await _connection.ExecuteAsync(sql, parameters);
        }
    }
}
