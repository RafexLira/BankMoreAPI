using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Transferencias.API;
using Xunit;

namespace Transferencias.Tests.Integration.Transferencias
{
    public class RegistrarTransferenciaTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RegistrarTransferenciaTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegistrarTransferencia_DeveRetornarOk()
        {
            var payload = new
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                NumeroContaOrigem = 123,
                NumeroContaDestino = 987,
                Valor = 50.00m
            };

            var resp = await _client.PostAsJsonAsync("/api/Transferencias", payload);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
