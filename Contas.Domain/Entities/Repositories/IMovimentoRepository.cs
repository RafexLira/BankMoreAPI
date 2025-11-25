namespace Contas.Domain.Entities.Repositories
{
    public interface IMovimentoRepository
    {
        Task AddAsync(Movimento movimento, CancellationToken ct = default);
        Task<IEnumerable<Movimento>> GetByContaAsync(string idContaCorrente, CancellationToken ct = default);
    }
}
