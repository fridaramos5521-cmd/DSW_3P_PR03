using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DSW_3P_PR03.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DSW_3P_PR03.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        string usuarioEsperado = _configuration["DemoUser:Usuario"]
            ?? throw new InvalidOperationException(
                "Falta configurar DemoUser:Usuario");

        string passwordEsperado = _configuration["DemoUser:Password"]
            ?? throw new InvalidOperationException(
                "Falta configurar DemoUser:Password");

        if (login.Usuario != usuarioEsperado ||
            login.Password != passwordEsperado)
        {
            return Unauthorized(new
            {
                mensaje = "Credenciales incorrectas."
            });
        }

        var jwt = _configuration.GetSection("Jwt");

        byte[] key = Convert.FromBase64String(
            jwt["Key"] ?? throw new InvalidOperationException(
                "Falta configurar Jwt:Key"));

        int minutos = int.Parse(jwt["ExpirationMinutes"] ?? "30");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Usuario),
            new Claim(ClaimTypes.Role, "Administrador")
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutos),
            signingCredentials: credentials);

        string tokenString =
            new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            access_token = tokenString,
            token_type = "Bearer",
            expires_in = minutos * 60
        });
    }
}