using System.IdentityModel.Tokens.Jwt;

namespace GerberBackend.Utils;

public class SessionToken
{
    public string GetSessionIdFromToken(string header)
    {
        if (header != null && header.StartsWith("Bearer "))
        {
            var token = header.Substring("Bearer ".Length).Trim();

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var sessionIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "SessionId");
            return sessionIdClaim?.Value;
        }

        return null;
    }
}
