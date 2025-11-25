using Contas.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Contas.Tests.Integration.Contas
{
    public class InativarContaTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public InativarContaTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task InativarConta_DeveRetornarNoContent()
        {

            var numeroConta = Random.Shared.Next(100000, 999999);
            var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => Random.Shared.Next(0, 10)));       

            var createResp = await _client.PostAsJsonAsync("/api/Contas", new
            {
                numero = numeroConta,
                nome = "Conta Teste",
                cpf = cpf,
                senha = "abc123"
            });

            Assert.True(createResp.IsSuccessStatusCode);
       
            var loginResp = await _client.PostAsJsonAsync("/api/Auth/login", new
            {
                numero = numeroConta,
                senha = "abc123",
                cpf = cpf
            });

            Assert.True(loginResp.IsSuccessStatusCode);

            var tokenObj = await loginResp.Content.ReadFromJsonAsync<TokenResponse>();
            var token = tokenObj!.AccessToken;

            Assert.False(string.IsNullOrWhiteSpace(token));


            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
      
            var desativarResp = await _client.PostAsJsonAsync("/api/Contas/inativar", new
            {
                numero = numeroConta,
                senha = "abc123"
            });

            Assert.Equal(System.Net.HttpStatusCode.NoContent, desativarResp.StatusCode);
        }

        private class TokenResponse
        {
            public string? AccessToken { get; set; }
        }
    }
}
