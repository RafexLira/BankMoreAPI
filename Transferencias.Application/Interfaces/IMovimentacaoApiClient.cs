using System.Threading.Tasks;

namespace Transferencias.Application.Interfaces
{
    public interface IMovimentacaoApiClient
    {
        Task<bool> DebitarAsync(int numeroConta, decimal valor, string idRequisicao);
        Task<bool> CreditarAsync(int numeroConta, decimal valor, string idRequisicao);
    }
}
