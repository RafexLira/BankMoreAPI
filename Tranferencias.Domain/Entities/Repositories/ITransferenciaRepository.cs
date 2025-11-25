using System.Threading;
using System.Threading.Tasks;

namespace Transferencias.Domain.Entities.Repositories
{
    public interface ITransferenciaRepository
    {
        Task AddAsync(Transferencia transferencia, CancellationToken ct);
    }
}
