using MediatR;
using Contas.Application.Queries;
using Contas.Application.ReadModels;
using Contas.Domain.Entities.Repositories;

namespace Contas.Application.QueryHandlers
{
    public class GetContaCorrenteByNumeroQueryHandler
     : IRequestHandler<GetContaCorrenteByNumeroQuery, ContaCorrenteReadModel?>
    {
        private readonly IContaCorrenteReadRepository _readRepo;

        public GetContaCorrenteByNumeroQueryHandler(IContaCorrenteReadRepository readRepo)
        {
            _readRepo = readRepo;
        }

        public async Task<ContaCorrenteReadModel?> Handle(GetContaCorrenteByNumeroQuery request, CancellationToken ct)
        {
            var conta = await _readRepo.GetByNumeroAsync(request.Numero, ct);

         
           return new ContaCorrenteReadModel
            {  
               Id = conta.Id.ToString(),           
                Numero = conta.Numero,
                Nome = conta.Nome,
                CPF = conta.CPF,
                Ativo = conta.Ativo
            };           
        }
    }
}
