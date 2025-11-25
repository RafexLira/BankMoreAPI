namespace Contas.Application.ReadModels
{
    public class ContaCorrenteReadModel
    {
        public string Id { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; } = null!;
        public string CPF { get; set; } = null!;
        public bool Ativo { get; set; }
        public string SenhaHash { get; set; } = null!;
        public string Salt { get; set; } = null!;
    }

}
