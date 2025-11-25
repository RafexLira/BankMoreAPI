namespace Contas.Domain.Entities.Repositories
{
    public interface IContaCorrenteWriteRepository
    {
        Task AddAsync(ContaCorrente conta, CancellationToken ct = default);
        Task UpdateAsync(ContaCorrente conta, CancellationToken ct = default);
    }

}
