using Contas.Application.CommandHandlers;
using Contas.Application.Commands;
using Contas.Application.Security;
using Contas.Domain.Entities;
using Contas.Domain.Entities.Repositories;
using Moq;
using Xunit;

namespace Contas.Tests.Unit.Application
{
    public class InativarContaCommandHandlerTests
    {
        private readonly Mock<IContaCorrenteReadRepository> _readRepoMock;
        private readonly Mock<IContaCorrenteWriteRepository> _writeRepoMock;
        private readonly InativarContaCommandHandler _handler;

        public InativarContaCommandHandlerTests()
        {
            _readRepoMock = new Mock<IContaCorrenteReadRepository>();
            _writeRepoMock = new Mock<IContaCorrenteWriteRepository>();

            _handler = new InativarContaCommandHandler(
                _writeRepoMock.Object,
                _readRepoMock.Object
            );
        }
 
        private ContaCorrente CriarContaFake(string senha)
        {
            var numeroConta = Random.Shared.Next(100000, 999999);

            var cpf = string.Concat(
                Enumerable.Range(0, 11)
                    .Select(_ => Random.Shared.Next(0, 10))
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
        public async Task DeveInativarConta_ComSenhaCorreta()
        {
            var senha = "abc123";
            var conta = CriarContaFake(senha);

            _readRepoMock
                .Setup(r => r.GetByNumeroAsync(conta.Numero, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conta);

            var command = new InativarContaCommand(conta.Numero, senha);

            await _handler.Handle(command, CancellationToken.None);

            Assert.False(conta.Ativo);
            _writeRepoMock.Verify(w => w.UpdateAsync(conta, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeveRetornarErro_QuandoContaNaoExiste()
        {
            var numeroConta = Random.Shared.Next(100000, 999999);

            _readRepoMock
                .Setup(r => r.GetByNumeroAsync(numeroConta, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ContaCorrente?)null);

            var command = new InativarContaCommand(numeroConta, "qualquer");

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("INVALID_ACCOUNT", ex.Message);

            _writeRepoMock.Verify(w => w.UpdateAsync(It.IsAny<ContaCorrente>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeveRetornarErro_QuandoSenhaIncorreta()
        {
            var conta = CriarContaFake("senha_certa");

            _readRepoMock
                .Setup(r => r.GetByNumeroAsync(conta.Numero, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conta);

            var command = new InativarContaCommand(conta.Numero, "senha_errada");

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("USER_UNAUTHORIZED", ex.Message);

            _writeRepoMock.Verify(w => w.UpdateAsync(It.IsAny<ContaCorrente>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
