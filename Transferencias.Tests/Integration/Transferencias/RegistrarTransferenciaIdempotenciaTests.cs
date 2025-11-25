using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Transferencias.Application.Security;
using Xunit;

namespace Transferencias.Tests.Integration.Transferencias
{
    public class RegistrarTransferenciaIdempotenciaTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public RegistrarTransferenciaIdempotenciaTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task RegistrarTransferencia_MesmaChave_DeveRetornarMesmoId()
        {
            var config = _factory.Services.GetRequiredService<IConfiguration>();

            var secret = config["JwtInternal:Secret"];
            var issuer = config["JwtInternal:Issuer"];
            var audience = config["JwtInternal:Audience"];

            var tokenService = new MicroserviceTokenService(secret!, issuer!, audience!);
            var token = tokenService.Generate();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var numeroContaOrigem = Random.Shared.Next(100000, 999999);
            var numeroContaDestino = Random.Shared.Next(100000, 999999);

            var payload = new
            {
                numeroContaOrigem,
                numeroContaDestino,
                valor = 50.00m,
                chaveIdempotencia = Guid.NewGuid().ToString()
            };


            // Act 1 - primeira tentativa
            var resp1 = await _client.PostAsJsonAsync("/api/Transferencias", payload);

            Console.WriteLine(resp1.StatusCode);

            Assert.True(resp1.IsSuccessStatusCode);

            var t1 = await resp1.Content.ReadFromJsonAsync<TransferenciaResponse>();
            Assert.NotNull(t1);
            Assert.False(string.IsNullOrWhiteSpace(t1.Id));

            // Act 2 - mesma chave novamente       
            var resp2 = await _client.PostAsJsonAsync("/api/Transferencias", payload);

            Console.WriteLine(resp2.StatusCode);
            Assert.True(resp2.IsSuccessStatusCode);

            var t2 = await resp2.Content.ReadFromJsonAsync<TransferenciaResponse>();
            Assert.NotNull(t2);

            // Assert - IDs devem ser os mesmos
            Assert.Equal(t1.Id, t2.Id);
        }

        private class TransferenciaResponse
        {
            public string Id { get; set; } = "";
        }
    }
}
