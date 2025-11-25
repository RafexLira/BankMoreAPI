using MediatR;

namespace Contas.Application.Commands
{
    public class LoginCommand : IRequest<string?>
    {
        public int Numero { get; }
        public string Senha { get; }
        public string Cpf { get; }

        public LoginCommand(int numero, string senha, string cpf)
        {
            Numero = numero;
            Senha = senha;
            Cpf = cpf;
        }
    }
}
