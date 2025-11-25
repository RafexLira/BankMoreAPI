using Contas.Application.CommandHandlers;
using Contas.Application.Commands;
using Contas.Application.ReadModels;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;
using Moq;
using Xunit;

namespace Contas.Tests.Unit.Application
{
    public class RegisterContaCorrenteCommandHandlerTests
    {
        private readonly Mock<IContaCorrenteWriteRepository> _writeRepoMock;
        private readonly Mock<IContaCorrenteReadRepository> _readRepoMock;
        private readonly RegisterContaCorrenteCommandHandler _handler;

        public RegisterContaCorrenteCommandHandlerTests()
        {
            _writeRepoMock = new Mock<IContaCorrenteWriteRepository>();
            _readRepoMock = new Mock<IContaCorrenteReadRepository>();

            _handler = new RegisterContaCorrenteCommandHandler(
                _writeRepoMock.Object,
                _readRepoMock.Object
            );
        }

        private static string GerarCpf()
        {
            return string.Concat(
                Enumerable.Range(0, 11)
                    .Select(_ => Random.Shared.Next(0, 10))
            );
        }

        [Fact]
        public async Task DeveRegistrarConta_ComSucesso()
        {
            // Arrange
            var numeroConta = Random.Shared.Next(100000, 999999);
            var cpf = GerarCpf();

            var command = new RegisterContaCorrenteCommand
            {
                Numero = numeroConta,
                Nome = "Usuário Teste",
                Cpf= cpf,
                Senha = "senha123"            
            };           
          

            _readRepoMock
                .Setup(r => r.ExistsNumeroAsync(numeroConta, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _writeRepoMock
                .Setup(r => r.AddAsync(It.IsAny<ContaCorrente>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(numeroConta, result.Numero);
            Assert.Equal("Usuário Teste", result.Nome);
            Assert.Equal(cpf, result.CPF);

            _writeRepoMock.Verify(
                w => w.AddAsync(It.IsAny<ContaCorrente>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeveFalhar_QuandoNumeroDeContaJaExiste()
        {
            // Arrange
            var numeroConta = Random.Shared.Next(100000, 999999);
            var cpf = GerarCpf();

            var command = new RegisterContaCorrenteCommand
            {
                Numero = numeroConta,
                Nome = "Usuário Teste",
                Cpf = cpf,
                Senha = "senha123"
            };

            _readRepoMock
                .Setup(r => r.ExistsNumeroAsync(numeroConta, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(command, CancellationToken.None)
            );

            _writeRepoMock.Verify(
                w => w.AddAsync(It.IsAny<ContaCorrente>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}
