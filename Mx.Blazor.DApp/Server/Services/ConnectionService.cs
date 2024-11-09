using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.NativeAuthServer;
using Mx.NET.SDK.NativeAuthServer.Entities;

namespace Mx.Blazor.DApp.Services
{
    public interface IConnectionService
    {
        ConnectionToken? Verify(string accessToken);
    }

    public class ConnectionService(IConfiguration configuration) : IConnectionService
    {
        public ConnectionToken? Verify(string accessToken)
        {
            var nativeAuthServer = new NativeAuthServer(new NativeAuthServerConfig());
            try
            {
                var nativeAuthToken = nativeAuthServer.Validate(accessToken);

                var jwtToken = GenerateJwtToken(nativeAuthToken.Address, accessToken, nativeAuthToken.Body.TTL);
                return new ConnectionToken(
                    new AccountToken()
                    {
                        Address = nativeAuthToken.Address,
                        Signature = nativeAuthToken.Signature
                    },
                    jwtToken
                );
            }
            catch
            {
                return null;
            }
        }

        private string GenerateJwtToken(string address, string accessToken, int ttl)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWT:SecurityKey") ?? string.Empty);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim("address", address),
                        new Claim("accessToken", accessToken)
                    }
                ),
                Expires = DateTime.UtcNow.AddSeconds(ttl),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
