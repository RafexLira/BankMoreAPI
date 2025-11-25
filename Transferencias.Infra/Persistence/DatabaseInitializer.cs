using Dapper;
using System.Data;

namespace Transferencias.Infra.Persistence
{
    public static class DatabaseInitializer
    {
        public static void EnsureDatabaseCreated(IDbConnection connection)
        {
            Console.WriteLine("Criando tabela transferencia..."); // para teste

            // tabela transferencia
            var sqlTransferencia = @"
                CREATE TABLE IF NOT EXISTS transferencia (
                    idtransferencia TEXT(37) PRIMARY KEY,
                    idcontacorrente_origem TEXT(37) NOT NULL,
                    idcontacorrente_destino TEXT(37) NOT NULL,
                    datamovimento TEXT NOT NULL,
                    valor REAL NOT NULL
                );
            ";

            Console.WriteLine("Criando tabela idempotencia..."); // para teste


            // tabela idempotencia
            var sqlIdempotencia = @"
                CREATE TABLE IF NOT EXISTS idempotencia (
                    chave_idempotencia TEXT(200) PRIMARY KEY,
                    requisicao TEXT NOT NULL,
                    resultado TEXT NOT NULL
                );
            ";

            connection.Execute(sqlTransferencia);
            connection.Execute(sqlIdempotencia);
        }
    }
}
