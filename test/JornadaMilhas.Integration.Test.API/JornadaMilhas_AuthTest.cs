using JornadaMilhas.API.DTO.Auth;
using System.Net;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;

public class JornadaMilhas_AuthTest : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public JornadaMilhas_AuthTest(JornadaMilhasWebApplicationFactory app)
    {
        _app = app;
    }

    [Fact]
    public async Task POST_Efetua_Login_Com_Sucesso()
    {
        //Arrange
        var user = new UserDTO { Email = "tester@email.com", Password = "Senha123@" };
        using var client = _app.CreateClient();

        //Act
        var resultado = await client.PostAsJsonAsync("/auth-login", user);

        //Assert
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);
    }
}