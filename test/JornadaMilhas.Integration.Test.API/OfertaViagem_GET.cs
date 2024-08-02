using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using JornadaMilhas.Integration.Test.API.DataBuilders;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;

[Collection(nameof(ContextCollection))]
public class OfertaViagem_GET// : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public OfertaViagem_GET(JornadaMilhasWebApplicationFactory app)
    {
        _app = app;
    }

    [Fact]
    public async Task Recuperar_OfertaViagem_PorId()
    {
        //Arrange  
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

        using var client = await _app.GetClientWithAccessTokenAsync();

        //Act
        var response = await client.GetFromJsonAsync<OfertaViagem>("/ofertas-viagem/" + ofertaExistente.Id);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(ofertaExistente.Preco, response.Preco, 0.001);
        Assert.Equal(ofertaExistente.Rota.Origem, response.Rota.Origem);
        Assert.Equal(ofertaExistente.Rota.Destino, response.Rota.Destino);
    }

    [Fact]
    public async Task Recuperar_OfertasViagens_Na_Consulta_Paginada()
    {
        //Arrange
        OfertaViagemDataBuilder databuilder = new OfertaViagemDataBuilder();
        var listaOfertas = databuilder.Generate(80);
        _app.Context.OfertasViagem.AddRange(listaOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = 1;
        int tamanhoPorPagina = 80;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem/?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.True(response != null);
        Assert.Equal(tamanhoPorPagina, response.Count());
    }

    [Fact]
    public async Task Recuperar_OfertasViagens_Na_Ultima_Pagina()
    {
        //Arrange
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM OfertasViagem");
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM Rota");

        OfertaViagemDataBuilder databuilder = new OfertaViagemDataBuilder();
        var listaOfertas = databuilder.Generate(80);
        _app.Context.OfertasViagem.AddRange(listaOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = 4;
        int tamanhoPorPagina = 25;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem/?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.True(response != null);
        // a expectativa de qtde retornada é a diferença entre tamanho da página e o resto de registros
        // tamanhoPorPagina = 25 - [(pagina * tamanhoPorPagina = 100) - (total de registros = 80)] == 5
        Assert.Equal(5, response.Count());
    }

    [Fact]
    public async Task Recuperar_OfertasViagens_Quando_Pagina_Nao_Existente()
    {
        //Arrange
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM OfertasViagem");
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM Rota");

        OfertaViagemDataBuilder databuilder = new OfertaViagemDataBuilder();

        var listaOfertas = databuilder.Generate(80);
        _app.Context.OfertasViagem.AddRange(listaOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = 5;
        int tamanhoPorPagina = 25;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.True(response != null);
        Assert.Equal(0, response.Count);
    }

    [Fact]
    public async Task Recuperar_OfertasViagens_Quando_Pagina_Negativa()
    {
        //Arrange
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM OfertasViagem");
        _app.Context.Database.ExecuteSqlRaw("DELETE FROM Rota");

        OfertaViagemDataBuilder databuilder = new OfertaViagemDataBuilder();

        var listaOfertas = databuilder.Generate(80);
        _app.Context.OfertasViagem.AddRange(listaOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = -5;
        int tamanhoPorPagina = 25;

        //Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");
        });
    }

    [Fact]
    public async Task Recuperar_OfertaViagem_Com_Maior_Desconto()
    {
        //Arrange  
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

        using var client = await _app.GetClientWithAccessTokenAsync();

        //Act
        var response = await client.GetFromJsonAsync<OfertaViagem>("/ofertas-viagem/maior-desconto");

        //Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task Recuperar_Ultima_OfertaViagem_Cadastrada()
    {
        //Arrange  
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

        using var client = await _app.GetClientWithAccessTokenAsync();

        //Act
        var response = await client.GetFromJsonAsync<OfertaViagem>("/ofertas-viagem/ultima-oferta");

        //Assert
        Assert.NotNull(response);
    }
}