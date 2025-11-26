using System;
using System.Threading.Tasks;
using Transferencias.Domain.Entities;

namespace Transferencias.Domain.Entities.Repositories
{
    public interface ITransferenciaRepository
    {
        Task<Transferencia?> ObterPorIdAsync(Guid id);
        Task<Transferencia?> ObterPorChaveIdempotenciaAsync(string chave);
        Task AdicionarAsync(Transferencia transferencia);
        Task AtualizarAsync(Transferencia transferencia);
    }
}
