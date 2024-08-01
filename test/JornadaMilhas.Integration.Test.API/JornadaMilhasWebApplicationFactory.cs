using JornadaMilhas.API.DTO.Auth;
using JornadaMilhas.Dados;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Testcontainers.MsSql;

namespace JornadaMilhas.Integration.Test.API;

public class JornadaMilhasWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public JornadaMilhasContext Context { get; private set; }

    private IServiceScope _scope;

    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .Build();

    //public JornadaMilhasWebApplicationFactory()
    //{
    //    scope = Services.CreateScope();
    //    Context = scope.ServiceProvider.GetRequiredService<JornadaMilhasContext>();
    //}

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<JornadaMilhasContext>));
            services.AddDbContext<JornadaMilhasContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(_mssqlContainer.GetConnectionString()));
            //.UseSqlServer("Server=localhost,11433;Database=JornadaMilhas;User ID=sa;Password=sqlRRC00!;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));            
        });
        base.ConfigureWebHost(builder);
    }

    public async Task<HttpClient> GetClientWithAccessTokenAsync()
    {
        var client = this.CreateClient();
        var user = new UserDTO { Email = "tester@email.com", Password = "Senha123@" };
        var resultado = await client.PostAsJsonAsync("/auth-login", user);
        resultado.EnsureSuccessStatusCode();

        var result = await resultado.Content.ReadFromJsonAsync<UserTokenDTO>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);

        return client;
    }

    public async Task InitializeAsync()
    {
        await _mssqlContainer.StartAsync();

        _scope = Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<JornadaMilhasContext>();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _mssqlContainer.DisposeAsync();
    }
}