using Moq;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Transferencias.Application.CommandHandlers;
using Transferencias.Application.Commands;
using Transferencias.Domain.Entities;
using Transferencias.Domain.Entities.Repositories;
using Xunit;

namespace Transferencias.Tests.Unit.Application
{
    public class RegistrarTransferenciaCommandHandlerTests
    {
        private readonly Mock<ITransferenciaRepository> _repo;
        private readonly Mock<ILogger<RegistrarTransferenciaCommandHandler>> _logger;

        public RegistrarTransferenciaCommandHandlerTests()
        {
            _repo = new Mock<ITransferenciaRepository>();
            _logger = new Mock<ILogger<RegistrarTransferenciaCommandHandler>>();
        }

        [Fact]
        public async Task Handler_DeveCriarTransferencia_QuandoValido()
        {
            // Arrange
            var cmd = new RegistrarTransferenciaCommand
            {
                ChaveIdempotencia = "abc123",
                NumeroContaOrigem = 100,
                NumeroContaDestino = 200,
                Valor = 50
            };
           
            _repo
                .Setup(r => r.ObterPorChaveIdempotenciaAsync(It.IsAny<string>()))
                .ReturnsAsync((Transferencia?)null);

            var handler = new RegistrarTransferenciaCommandHandler(
                _repo.Object,
                _logger.Object
            );

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Concluida", result.Status);

            _repo.Verify(r => r.AdicionarAsync(It.IsAny<Transferencia>()), Times.Once);
            _repo.Verify(r => r.AtualizarAsync(It.IsAny<Transferencia>()), Times.Once);
        }

        [Fact]
        public async Task Handler_DeveRetornarTransferenciaExistente_QuandoIdempotente()
        {
            // Arrange
            var existente = Transferencia.Criar("abc123", 100, 200, 50);

            _repo
                .Setup(r => r.ObterPorChaveIdempotenciaAsync("abc123"))
                .ReturnsAsync(existente);

            var handler = new RegistrarTransferenciaCommandHandler(
                _repo.Object,
                _logger.Object
            );

            var cmd = new RegistrarTransferenciaCommand
            {
                ChaveIdempotencia = "abc123",
                NumeroContaOrigem = 100,
                NumeroContaDestino = 200,
                Valor = 50
            };

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existente.Id, result.Id);
            Assert.Equal("Pendente", result.Status);

            _repo.Verify(r => r.AdicionarAsync(It.IsAny<Transferencia>()), Times.Never);
            _repo.Verify(r => r.AtualizarAsync(It.IsAny<Transferencia>()), Times.Never);
        }

        [Fact]
        public async Task Handler_DeveRetornarFalha_QuandoDominioInvalidar()
        {
            // Arrange
            var cmd = new RegistrarTransferenciaCommand
            {
                ChaveIdempotencia = "",
                NumeroContaOrigem = 0,
                NumeroContaDestino = 0,
                Valor = -10
            };

            var handler = new RegistrarTransferenciaCommandHandler(
                _repo.Object,
                _logger.Object
            );

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Falha", result.Status);

            _repo.Verify(r => r.AdicionarAsync(It.IsAny<Transferencia>()), Times.Never);
        }
    }
}
