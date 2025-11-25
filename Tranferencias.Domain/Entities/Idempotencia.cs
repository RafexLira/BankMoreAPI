namespace Transferencias.Domain.Entities
{
    public class Idempotencia
    {
        public string Chave { get; private set; }
        public string Requisicao { get; private set; }
        public string Resultado { get; private set; }

        protected Idempotencia() { }

        public Idempotencia(string chave, string requisicao, string resultado)
        {
            Chave = chave;
            Requisicao = requisicao;
            Resultado = resultado;
        }
    }
}
