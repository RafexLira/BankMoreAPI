using Contas.Application.Commands;
using Contas.Application.ReadModels;
using Contas.Application.Security;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;
using MediatR;

namespace Contas.Application.CommandHandlers;

public class RegisterContaCorrenteCommandHandler
    : IRequestHandler<RegisterContaCorrenteCommand, ContaCorrenteReadModel>
{
    private readonly IContaCorrenteWriteRepository _writeRepo;
    private readonly IContaCorrenteReadRepository _readRepo;

    public RegisterContaCorrenteCommandHandler(
        IContaCorrenteWriteRepository writeRepo,
        IContaCorrenteReadRepository readRepo)
    {
        _writeRepo = writeRepo;
        _readRepo = readRepo;
    }

    public async Task<ContaCorrenteReadModel> Handle(RegisterContaCorrenteCommand request,CancellationToken ct)
    {
        if (await _readRepo.ExistsNumeroAsync(request.Numero, ct)) throw new Exception("Este número de conta já existe.");

        var (hash, salt) = PasswordHasher.Hash(request.Senha);

        var conta = new ContaCorrente(
            request.Numero,
            request.Nome,
            request.Cpf,
            hash,
            salt
        );

        await _writeRepo.AddAsync(conta, ct);

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
