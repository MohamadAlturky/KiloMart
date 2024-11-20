using System.IdentityModel.Tokens.Jwt;
using KiloMart.Domain.Login.Models;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace KiloMart.Presentation.RealTime;

public static class HubUserProvider
{
    public static int GetUserId(HubCallerContext callerContext)
    {
        Microsoft.Extensions.Primitives.StringValues? token =
                (callerContext.GetHttpContext()?.Request.Query["access_token"]) ??
                    throw new UnauthorizedAccessException();
        bool decodingResult = JwtTokenValidator.ValidateToken(token!,
            GuardAttribute.SECRET_KEY,
            GuardAttribute.ISSUER,
            GuardAttribute.AUDIENCE,
            out JwtSecurityToken? decodedToken);

        if (!decodingResult || decodedToken is null)
        {
            throw new UnauthorizedAccessException();
        }

        var userIdClaim = decodedToken?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }
}