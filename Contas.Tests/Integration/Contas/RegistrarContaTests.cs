using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Contas.Tests.Integration.Contas
{
    public class RegistrarContaTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RegistrarContaTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegistrarConta_DeveRetornarCreated()
        {
            var numeroConta = Random.Shared.Next(100000, 999999);
            var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => Random.Shared.Next(0, 10)));

         
            var request = new
            {
                numero = numeroConta,
                nome = "Teste Registro",
                cpf = cpf,
                senha = "1234"
            };

     
            var response = await _client.PostAsJsonAsync("/api/Contas", request);
           
        
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
