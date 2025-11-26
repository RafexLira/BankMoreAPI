using Dapper;
using System.Data;

namespace Transferencias.Infra.Persistence
{
    public static class DatabaseInitializer
    {
        public static void Initialize(IDbConnection connection)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS Transferencia (
                    Id TEXT PRIMARY KEY,
                    ChaveIdempotencia TEXT NOT NULL,
                    NumeroContaOrigem INTEGER NOT NULL,
                    NumeroContaDestino INTEGER NOT NULL,
                    Valor REAL NOT NULL,
                    Status INTEGER NOT NULL,
                    CodigoErro TEXT,
                    MensagemErro TEXT,
                    DataCriacao TEXT NOT NULL,
                    DataConclusao TEXT
                );
            ";

            connection.Execute(sql);
        }
    }
}
