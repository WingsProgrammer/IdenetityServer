using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace MS.Services.Identity;

// Ref: https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/api_resources/
// https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/identity/
// https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/api_scopes/
public static class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
            new IdentityResources.Address(),
            new("roles", "User Roles", new List<string> { "role" })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> {
            new("UserManagement.Read", "MS.Services.UserManagement Web API Read Role"),
            new("UserManagement.Write", "MS.Services.UserManagement Web API Write Role"),

            new("TaskCataloge.Read", "MS.Services.TaskCataloge Web API Read Role"),
            new("TaskCataloge.Write", "MS.Services.TaskCataloge Web API Write Role"),
            };

    public static IList<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("UserManagement", "MS.Services.UserManagement Web API")
            {
                Scopes = {
                    "UserManagement.Read",
                    "UserManagement.Write"
                },
                UserClaims = {
                    JwtClaimTypes.Role,
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Id
                }
            },
            new ApiResource("TaskCataloge", "MS.Services.TaskCataloge Web API")
            {
                 Scopes = {
                    "TaskCataloge.Read",
                    "TaskCataloge.Write"
                },
                UserClaims = {
                    JwtClaimTypes.Role,
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Id
                }
            }

        };


    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new()
            {
                ClientId = "Frontend-client",
                ClientName = "Frontend Client",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets={new Secret("123456".ToSha256())},
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "shop-api"
                }
            },
            new()
            {
                ClientId = "oauthClient",
                ClientName = "Example client application using client credentials",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> { new("SuperSecretPassword".Sha256()) }, // change me!
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "shop-api"
                }
            }
        };
}
