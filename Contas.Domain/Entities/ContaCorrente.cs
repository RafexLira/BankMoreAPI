using Contas.Domain.Exceptions;

namespace Contas.Domain.Entities
{
    public class ContaCorrente
    {
        public string Id { get; private set; }
        public int Numero { get; private set; }
        public string Nome { get; private set; } = null!;
        public string CPF { get; private set; } = null!;
        public bool Ativo { get; private set; }
        public string SenhaHash { get; private set; } = null!;
        public string Salt { get; private set; } = null!;

        protected ContaCorrente() { }

        public ContaCorrente(int numero, string nome, string cpf, string senhaHash, string salt)
        {
            if (numero <= 0)
                throw new DomainException("Número da conta inválido.");

            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome inválido.");

            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                throw new DomainException("CPF inválido.");

            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new DomainException("Senha inválida.");

            if (string.IsNullOrWhiteSpace(salt))
                throw new DomainException("Salt inválido.");

            Id = Guid.NewGuid().ToString();
            Numero = numero;
            Nome = nome.Trim();
            CPF = cpf;
            SenhaHash = senhaHash;
            Salt = salt;
            Ativo = true;
        }

        public void AlterarNome(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new DomainException("Nome inválido.");

            Nome = novoNome.Trim();
        }

        public void AlterarSenha(string novaSenhaHash, string novoSalt)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
                throw new DomainException("Senha inválida.");

            if (string.IsNullOrWhiteSpace(novoSalt))
                throw new DomainException("Salt inválido.");

            SenhaHash = novaSenhaHash;
            Salt = novoSalt;
        }

        public void Ativar() => Ativo = true;

        public void Desativar() => Ativo = false;
    }
}
