using MS.Services.Identity.Users.Features.GettingUerByEmail;
using MS.Services.Identity.Users.Features.GettingUserById;
using MS.Services.Identity.Users.Features.GettingUsers;
using MS.Services.Identity.Users.Features.RegisteringUser;
using MS.Services.Identity.Users.Features.UpdatingUserState;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MS.Services.Identity.Users;

internal static class UsersConfigs
{
    public const string Tag = "Users";
    public const string UsersPrefixUri = $"{IdentityModuleConfiguration.IdentityModulePrefixUri}/users";

    internal static IServiceCollection AddUsersServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        return services;
    }

    internal static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRegisterNewUserEndpoint();
        endpoints.MapUpdateUserStateEndpoint();
        endpoints.MapGetUserByIdEndpoint();
        //endpoints.MapGetUsersEndpoint();
        endpoints.MapGetUserByEmailEndpoint();

        return endpoints;
    }
}
