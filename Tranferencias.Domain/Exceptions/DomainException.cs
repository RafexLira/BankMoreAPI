using System;

namespace Transferencias.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string? Codigo { get; }

        public DomainException(string mensagem, string? codigo = null)
            : base(mensagem)
        {
            Codigo = codigo;
        }
    }
}
