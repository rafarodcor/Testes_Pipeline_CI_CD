using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;

[Collection(nameof(ContextCollection))]
public class OfertaViagem_PUT// : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public OfertaViagem_PUT(JornadaMilhasWebApplicationFactory app)
    {
        _app = app;
    }

    [Fact]
    public async Task Atualizar_OfertaViagem()
    {
        //Arrange  
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM OfertasViagem");
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM Rota");

        var ofertaExistente = _app.Context.OfertasViagem.FirstOrDefault();
        if (ofertaExistente is null)
        {
            ofertaExistente = new OfertaViagem()
            {
                Preco = 100,
                Rota = new Rota("Origem", "Destino"),
                Periodo = new Periodo(DateTime.Parse("2024-03-03"), DateTime.Parse("2024-03-06"))
            };
            _app.Context.Add(ofertaExistente);
            _app.Context.SaveChanges();
        }

        ofertaExistente.Rota.Origem = "Origem Atualizada";
        ofertaExistente.Rota.Destino = "Destino Atualizada";

        using var client = await _app.GetClientWithAccessTokenAsync();

        //Act
        var response = await client.PutAsJsonAsync($"/ofertas-viagem/", ofertaExistente);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}