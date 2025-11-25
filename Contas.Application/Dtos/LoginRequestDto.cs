using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contas.Application.Dtos
{
    public class LoginRequestDto
    {
        public int Numero { get; set; }
        public string Senha { get; set; } = null!;
        public string Cpf { get; set; } = null!;
    }
}
