using MediatR;
using Transferencias.Application.Dtos;
using Transferencias.Application.Queries;
using Transferencias.Domain.Entities.Repositories;

namespace Transferencias.Application.QueryHandlers
{
    public class GetTransferenciaByIdQueryHandler
        : IRequestHandler<GetTransferenciaByIdQuery, TransferenciaDto?>
    {
        private readonly ITransferenciaRepository _repo;

        public GetTransferenciaByIdQueryHandler(ITransferenciaRepository repo)
        {
            _repo = repo;
        }

        public async Task<TransferenciaDto?> Handle(
            GetTransferenciaByIdQuery request,
            CancellationToken cancellationToken)
        {
            var t = await _repo.ObterPorIdAsync(request.Id);

            if (t == null)
                return null;

            return new TransferenciaDto
            {
                Id = t.Id,
                NumeroContaOrigem = t.NumeroContaOrigem,
                NumeroContaDestino = t.NumeroContaDestino,
                Valor = t.Valor,
                ChaveIdempotencia = t.ChaveIdempotencia,
                Status = t.Status.ToString(),
                CodigoErro = t.CodigoErro,
                MensagemErro = t.MensagemErro,
                DataCriacao = t.DataCriacao,
                DataConclusao = t.DataConclusao
            };
        }
    }
}
