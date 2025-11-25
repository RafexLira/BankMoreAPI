using Contas.Application.CommandHandlers;
using Contas.Application.Commands;
using Contas.Application.Security;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;
using Moq;
using System.Security.Cryptography;
using Xunit;

namespace Contas.Tests.Unit.Application
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IContaCorrenteReadRepository> _contaCorrenteReadRepoMock;
        private readonly TokenService _tokenService;
        private readonly LoginCommandHandler _loginCommandHandler;

        public LoginCommandHandlerTests()
        {
            _contaCorrenteReadRepoMock = new Mock<IContaCorrenteReadRepository>();


            var keyBytes = RandomNumberGenerator.GetBytes(32);
            var _secretBase64 = Convert.ToBase64String(keyBytes);

            // TokenService REAL — igual ao que o Program.cs registra
            _tokenService = new TokenService(
                secretBase64: _secretBase64,
                expiryMinutes: 60,
                issuer: "ContasAPI",
                audience: "ContasAPIUsers"
            );


            _loginCommandHandler = new LoginCommandHandler(
                _contaCorrenteReadRepoMock.Object,
                _tokenService
            );
        }

        private static ContaCorrente CriarContaFake(string senha)
        {
            var numeroConta = Random.Shared.Next(100000, 999999);

            var cpf = string.Concat(
                Enumerable.Range(0, 11).Select(_ => Random.Shared.Next(0, 10))
            );

            var (hash, salt) = PasswordHasher.Hash(senha);

            return new ContaCorrente(
                numeroConta,
                "Usuário Teste",
                cpf,
                hash,
                salt
            );
        }

        [Fact]
        public async Task DeveRetornarToken_QuandoDadosCorretos()
        {
            var senha = "abc123";
            var conta = CriarContaFake(senha);

            _contaCorrenteReadRepoMock
                .Setup(r => r.GetByNumeroAsync(conta.Numero, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conta);

            var loginCommand = new LoginCommand(conta.Numero, senha, conta.CPF);

            var result = await _loginCommandHandler.Handle(loginCommand, CancellationToken.None);

            Assert.False(string.IsNullOrWhiteSpace(result)); // token gerado

        }

        [Fact]
        public async Task DeveRetornarNull_QuandoCpfErrado()
        {
            var conta = CriarContaFake("abc123");

            _contaCorrenteReadRepoMock
                .Setup(r => r.GetByNumeroAsync(conta.Numero, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conta);

            var loginCommand = new LoginCommand(conta.Numero, "abc123", "99999999999");

            var result = await _loginCommandHandler.Handle(loginCommand, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeveRetornarNull_QuandoSenhaErrada()
        {
            var contaFake = CriarContaFake("senha_certa");

            _contaCorrenteReadRepoMock
                .Setup(r => r.GetByNumeroAsync(contaFake.Numero, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contaFake);

            var cmd = new LoginCommand(contaFake.Numero, "senha_errada", contaFake.CPF);

            var result = await _loginCommandHandler.Handle(cmd, CancellationToken.None);

            Assert.Null(result);
        }
    }
}
