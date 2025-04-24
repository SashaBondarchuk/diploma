using Employee.Performance.Evaluator.Application.Abstractions.Auth;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee.Performance.Evaluator.Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUsersRepository _usersRepository;

    public JwtProvider(IOptions<JwtOptions> jwtOptions, IUsersRepository usersRepository)
    {
        _jwtOptions = jwtOptions.Value;
        _usersRepository = usersRepository;
    }

    public async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Email),
        };

        var userPermissions = await _usersRepository.GetPermissionsAsync(user.Id, cancellationToken);
        foreach (var permission in userPermissions)
        {
            claims.Add(new Claim(CustomClaims.PermissionsClaim, permission.ToString()));
        }

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var expirationTimeInHours = _jwtOptions.ExpirationTimeInHours;
        var expires = DateTime.UtcNow.AddHours(expirationTimeInHours);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expires,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
