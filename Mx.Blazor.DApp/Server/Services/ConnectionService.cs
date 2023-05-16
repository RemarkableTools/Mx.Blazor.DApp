using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.Blazor.DApp.Server.Helpers;

namespace Mx.Blazor.DApp.Server.Services
{
    public interface IConnectionService
    {
        ConnectionToken? Verify(ConnectionRequest request);
    }

    public class ConnectionService : IConnectionService
    {
        private readonly IConfiguration _configuration;

        public ConnectionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ConnectionToken? Verify(ConnectionRequest request)
        {
            if (string.IsNullOrEmpty(request.AuthToken)) return null;
            var authToken = new AuthToken(request.AuthToken);
            if (!authToken.IsValid()) return null;

            var ownership = SignatureVerifier.Verify(request);
            if (!ownership) return null;

            var token = GenerateJwtToken(request.AccountToken.Address, request.AuthToken, authToken.TimeToLive);
            return new ConnectionToken()
            {
                AccountToken = request.AccountToken,
                AccessToken = token
            };
        }

        private string GenerateJwtToken(string address, string authToken, int ttl)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWT:SecurityKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("address", address),
                    new Claim("authToken", authToken)
                }),
                Expires = DateTime.UtcNow.AddSeconds(ttl),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
