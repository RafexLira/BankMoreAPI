using Contas.Application.ReadModels;
using MediatR;

namespace Contas.Application.Commands
{
    public class RegisterContaCorrenteCommand : IRequest<ContaCorrenteReadModel>
    {
        public int Numero { get; set; }
        public string Nome { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string Cpf { get; set; } = null!;
    }
}
