using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Transferencias.Application.Security;
using Xunit;
using Transferencias.Api;

namespace Transferencias.Tests.Integration.Transferencias
{
    public class RegistrarTransferenciaTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public RegistrarTransferenciaTests(WebApplicationFactory<Program> factory )
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task DeveRegistrarTransferencia_ComSucesso()
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
  
            var resp = await _client.PostAsJsonAsync("/api/Transferencias", payload);
            Console.WriteLine(resp.StatusCode);

            Assert.True(resp.IsSuccessStatusCode);

            var json = await resp.Content.ReadFromJsonAsync<TransferenciaResponse>();
            Assert.NotNull(json);
            Assert.False(string.IsNullOrWhiteSpace(json.Id));
        }
        private class TransferenciaResponse
        {
            public string Id { get; set; } = "";
        }
    }
}
