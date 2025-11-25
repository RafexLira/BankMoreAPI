using Moq;
using Transferencias.Application.CommandHandlers;
using Transferencias.Application.Commands;
using Transferencias.Domain.Entities;
using Transferencias.Domain.Entities.Repositories;
using Xunit;

namespace Transferencias.Tests.Unit.Application
{
    public class RegisterTransferenciaCommandHandlerTests
    {
        private readonly Mock<ITransferenciaRepository> _transferRepoMock;
        private readonly Mock<IIdempotenciaRepository> _idempotenciaRepoMock;
        private readonly RegisterTransferenciaCommandHandler _handler;

        public RegisterTransferenciaCommandHandlerTests()
        {
            _transferRepoMock = new Mock<ITransferenciaRepository>();
            _idempotenciaRepoMock = new Mock<IIdempotenciaRepository>();

            _handler = new RegisterTransferenciaCommandHandler(
                _transferRepoMock.Object,
                _idempotenciaRepoMock.Object
            );
        }

        [Fact]
        public async Task DeveRegistrarTransferencia_ComSucesso()
        {
            var cmd = new RegisterTransferenciaCommand
            {
                NumeroContaOrigem = 1001,
                NumeroContaDestino = 2002,
                Valor = 150.75m,
                ChaveIdempotencia = Guid.NewGuid().ToString()
            };

            _idempotenciaRepoMock
                .Setup(r => r.GetByChaveAsync(cmd.ChaveIdempotencia, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Idempotencia?)null);

            _transferRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Transferencia>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _idempotenciaRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Idempotencia>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(cmd, CancellationToken.None);

            Assert.False(string.IsNullOrWhiteSpace(result));
            _transferRepoMock.Verify(r => r.AddAsync(It.IsAny<Transferencia>(), It.IsAny<CancellationToken>()), Times.Once);
            _idempotenciaRepoMock.Verify(r => r.AddAsync(It.IsAny<Idempotencia>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeveRetornarMesmoResultado_QuandoIdempotenciaExistir()
        {
            var chave = Guid.NewGuid().ToString();

            var cmd = new RegisterTransferenciaCommand
            {
                NumeroContaOrigem = 1111,
                NumeroContaDestino = 2222,
                Valor = 10,
                ChaveIdempotencia = chave
            };

            var idem = new Idempotencia(chave, "req", "resultado_antigo");

            _idempotenciaRepoMock
                .Setup(r => r.GetByChaveAsync(chave, It.IsAny<CancellationToken>()))
                .ReturnsAsync(idem);

            var result = await _handler.Handle(cmd, CancellationToken.None);

            Assert.Equal("resultado_antigo", result);
            _transferRepoMock.Verify(r => r.AddAsync(It.IsAny<Transferencia>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
