using System.Threading;
using System.Threading.Tasks;

namespace Transferencias.Domain.Entities.Repositories
{
    public interface IIdempotenciaRepository
    {
        Task<Idempotencia?> GetByChaveAsync(string chave, CancellationToken ct);
        Task AddAsync(Idempotencia idem, CancellationToken ct);
    }
}
