using Contas.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Contas.Tests.Integration.Contas
{
    public class ObterContaTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ObterContaTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ObterPorNumero_DeveRetornarOk()
        {

            var numeroConta = Random.Shared.Next(100000, 999999);
            var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => Random.Shared.Next(0, 10)));

         
            var novaConta = new
            {
                numero = numeroConta,
                nome = "Conta Para Consulta",
                cpf = cpf,
                senha = "abc123"
            };

            var create = await _client.PostAsJsonAsync("/api/Contas", novaConta);
            var createBody = await create.Content.ReadAsStringAsync();

            Console.WriteLine("STATUS CREATE: " + create.StatusCode);
            Console.WriteLine("BODY CREATE: " + createBody);

            Assert.True(create.IsSuccessStatusCode, "ERRO AO REGISTRAR CONTA: " + createBody);



            Assert.True(create.IsSuccessStatusCode);


   
            var login = await _client.PostAsJsonAsync("/api/Auth/login", new
            {
                numero = numeroConta,
                senha = "abc123",
                cpf = cpf
            });

            Assert.Equal(System.Net.HttpStatusCode.OK, login.StatusCode);

            var tokenObj = await login.Content.ReadFromJsonAsync<TokenResponse>();
            var token = tokenObj!.AccessToken;
            Assert.False(string.IsNullOrEmpty(token));


    
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


        
            var response = await _client.GetAsync("/api/Contas/2001");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Conta Para Consulta", body);
            Assert.Contains("2001", body);
        }

        private class TokenResponse
        {
            public string? AccessToken { get; set; }
        }
    }
}
