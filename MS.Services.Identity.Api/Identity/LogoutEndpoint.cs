using MsftFramework.Security.Jwt;

namespace MS.Services.Identity.Identity.Features.Logout;

public static class LogoutEndpoint
{
    internal static IEndpointRouteBuilder MapLogoutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfigs.IdentityPrefixUri}/logout", async (HttpContext httpContext ,IAccessTokenService tokenService) =>
            {
                
                await tokenService.DeactivateCurrentAsync();
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .WithTags(IdentityConfigs.Tag)
            .RequireAuthorization()
            .WithDisplayName("Logout User.");

        return endpoints;
    }
}
