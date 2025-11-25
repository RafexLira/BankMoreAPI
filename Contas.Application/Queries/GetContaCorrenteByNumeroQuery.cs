using MediatR;
using Contas.Application.ReadModels;

namespace Contas.Application.Queries
{
    public class GetContaCorrenteByNumeroQuery : IRequest<ContaCorrenteReadModel?>
    {
        public int Numero { get; }

        public GetContaCorrenteByNumeroQuery(int numero)
        {
            Numero = numero;
        }
    }
}
