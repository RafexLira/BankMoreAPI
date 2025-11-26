using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Transferencias.API;
using Xunit;

namespace Transferencias.Tests.Integration.Transferencias
{
    public class RegistrarTransferenciaIdempotenciaTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RegistrarTransferenciaIdempotenciaTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegistrarTransferencia_MesmaChave_DeveRetornarMesmoId()
        {
            var chave = Guid.NewGuid().ToString();

            var payload = new
            {
                ChaveIdempotencia = chave,
                NumeroContaOrigem = 111,
                NumeroContaDestino = 222,
                Valor = 10.00m
            };

            var resp1 = await _client.PostAsJsonAsync("/api/Transferencias", payload);
            var t1 = await resp1.Content.ReadFromJsonAsync<TransferenciaResponse>();

            Assert.True(resp1.IsSuccessStatusCode);
            Assert.NotNull(t1);
            Assert.False(string.IsNullOrWhiteSpace(t1.Id));

            var resp2 = await _client.PostAsJsonAsync("/api/Transferencias", payload);
            var t2 = await resp2.Content.ReadFromJsonAsync<TransferenciaResponse>();

            Assert.True(resp2.IsSuccessStatusCode);
            Assert.NotNull(t2);

            Assert.Equal(t1.Id, t2.Id);
        }

        private class TransferenciaResponse
        {
            public string Id { get; set; } = "";
        }
    }
}
