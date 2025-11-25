using Contas.Application.Commands;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;
using MediatR;

namespace Contas.Application.CommandHandlers
{
    public class RegistrarMovimentoCommandHandler: IRequestHandler<RegistrarMovimentoCommand, Unit>
    {
        private readonly IMovimentoRepository _repo;

        public RegistrarMovimentoCommandHandler(IMovimentoRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(RegistrarMovimentoCommand request, CancellationToken ct)
        {
            var movimento = new Movimento(
                request.IdContaCorrente,
                request.Tipo,
                request.Valor
            );

            await _repo.AddAsync(movimento, ct);

            return Unit.Value;
        }
    }
}
