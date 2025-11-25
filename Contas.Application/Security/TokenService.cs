using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class TokenService
{
    private readonly string _secretBase64;
    private readonly int _expiryMinutes;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(string secretBase64, int expiryMinutes, string issuer, string audience)
    {
        _secretBase64 = secretBase64;
        _expiryMinutes = expiryMinutes;
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateToken(Guid contaId)
    {
        var keyBytes = Convert.FromBase64String(_secretBase64);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("contaId", contaId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
