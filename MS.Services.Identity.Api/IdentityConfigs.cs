using MS.Services.Identity.Identity.Features.GetClaims;
using MS.Services.Identity.Identity.Features.Login;
using MS.Services.Identity.Identity.Features.Logout;
using MS.Services.Identity.Identity.Features.RefreshingToken;
using MS.Services.Identity.Identity.Features.RevokeRefreshToken;
using MS.Services.Identity.Identity.Features.SendEmailVerificationCode;
using MS.Services.Identity.Identity.Features.VerifyEmail;
using MS.Services.Identity.Shared.Data;
using MS.Services.Identity.Shared.Extensions.ServiceCollectionExtensions;
using MsftFramework.Abstractions.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MS.Services.Identity.Api.Identity;

namespace MS.Services.Identity.Identity;

internal static class IdentityConfigs
{
    public const string Tag = "Identity";
    public const string IdentityPrefixUri = $"{IdentityModuleConfiguration.IdentityModulePrefixUri}";

    internal static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        services.AddCustomIdentity(configuration);
        services.AddCustomIdentityServer();

        return services;
    }

    internal static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            $"{IdentityPrefixUri}/user-role",
            [Authorize(
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                Roles = Constants.Role.User)]
            () => new { Role = Constants.Role.User }).WithTags("Identity");

        endpoints.MapGet(
            $"{IdentityPrefixUri}/admin-role",
            [Authorize(
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                Roles = Constants.Role.Admin)]
            () => new { Role = Constants.Role.Admin }).WithTags("Identity");

        endpoints.MapLoginUserEndpoint();
        endpoints.MapLogoutEndpoint();
        endpoints.MapSendEmailVerificationCodeEndpoint();
        endpoints.MapSendVerifyEmailEndpoint();
        endpoints.MapRefreshTokenEndpoint();
        endpoints.MapRevokeTokenEndpoint();
        endpoints.MapGetClaimsEndpoint();
        endpoints.MapForgotePasswordSendCodeEndpoint();
        endpoints.MapVerifyCodeForMobileEndpoint();
        endpoints.MapChangePasswordByMobileEndpoint();

        return endpoints;
    }
}
