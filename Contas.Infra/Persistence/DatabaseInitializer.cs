using Dapper;
using System.Data;

namespace Contas.Infra.Persistence
{
    public static class DatabaseInitializer
    {
        public static void EnsureDatabaseCreated(IDbConnection connection)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS contacorrente (
                    idcontacorrente TEXT(37) PRIMARY KEY,
                    numero INTEGER(10) NOT NULL UNIQUE,
                    nome TEXT(100) NOT NULL,
                    cpf TEXT(11) NOT NULL,
                    ativo INTEGER(1) NOT NULL DEFAULT 1,
                    senha TEXT(100) NOT NULL,
                    salt TEXT(100) NOT NULL,
                    CHECK (ativo IN (0, 1))
                );
                   CREATE TABLE IF NOT EXISTS movimento (
                   idmovimento TEXT(37) PRIMARY KEY,
                   idcontacorrente TEXT(37) NOT NULL,
                   datamovimento TEXT NOT NULL,
                   tipomovimento TEXT NOT NULL,
                   valor REAL NOT NULL
    );
            ";

            connection.Execute(sql);
        }
    }
}
