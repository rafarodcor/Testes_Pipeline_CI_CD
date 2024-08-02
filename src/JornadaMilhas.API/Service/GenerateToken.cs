using JornadaMilhas.API.DTO.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JornadaMilhas.API.Service;

internal class GenerateToken
{
    private readonly IConfiguration _configuration;
    public GenerateToken(IConfiguration configuration) => _configuration = configuration;

    internal UserTokenDTO GenerateUserToken(UserDTO user)
    {
        //Configurações a serem usadas no Token a ser gerado.
        var myClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName,user.Email!),
            new Claim("alura","c#"),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        //Gerar uma chave simétrica
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:key"]!));

        //Faz uma assinatura da chave
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        //Expiration
        var expiracao = _configuration["JWTTokenConfiguration:ExpireHours"];
        var expiracaoEmHoras = DateTime.UtcNow.AddHours(double.Parse(expiracao!));

        //Gerando Token
        JwtSecurityToken? token = null;
        try
        {
            token = new JwtSecurityToken(
            claims: myClaims,
            expires: expiracaoEmHoras,
            issuer: _configuration["JWTTokenConfiguration:Issuer"],
            audience: _configuration["JWTTokenConfiguration:Audience"],
            signingCredentials: credenciais);
        }
        catch (Exception)
        {
            throw new ArgumentException("Problemas na geração do token JWT.");
        }

        return new UserTokenDTO()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiracaoEmHoras,
        };
    }
}