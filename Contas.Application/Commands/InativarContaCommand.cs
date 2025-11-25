using MediatR;

namespace Contas.Application.Commands
{
    public class InativarContaCommand : IRequest<bool>
    {
        public int Numero { get; }
        public string Senha { get; }

        public InativarContaCommand(int numero, string senha)
        {
            Numero = numero;
            Senha = senha;
        }
    }
}
