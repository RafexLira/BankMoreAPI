using System.Data;
using Dapper;

namespace Movimentacoes.Infra.Persistence
{
    public static class DatabaseInitializer
    {
        public static void Initialize(IDbConnection connection)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS Movimentacao (
                    Id TEXT PRIMARY KEY,
                    NumeroConta INTEGER NOT NULL,
                    Valor REAL NOT NULL,
                    Tipo INTEGER NOT NULL,
                    IdentificacaoRequisicao TEXT NOT NULL,
                    DataMovimento TEXT NOT NULL
                );
            ";

            connection.Execute(sql);
        }
    }
}
