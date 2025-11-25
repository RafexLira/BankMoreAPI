using System;
using System.Threading;
using System.Threading.Tasks;
using Contas.Domain.Entities;

namespace Contas.Domain.Entities.Repositories
{
    public interface IContaCorrenteReadRepository
    {
        Task<ContaCorrente?> GetByNumeroAsync(int numero, CancellationToken ct = default);
        Task<bool> ExistsNumeroAsync(int numero, CancellationToken ct = default);
    }

}
