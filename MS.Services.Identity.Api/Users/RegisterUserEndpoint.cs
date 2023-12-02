using MsftFramework.Abstractions.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MS.Services.Identity.Users.Features.RegisteringUser;

public static class RegisterUserEndpoint
{
    internal static IEndpointRouteBuilder MapRegisterNewUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{UsersConfigs.UsersPrefixUri}", RegisterUser)
            .AllowAnonymous()
            .WithTags(UsersConfigs.Tag)
            .Produces<RegisterUserResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Register New user.");

        return endpoints;
    }

    private static async Task<IResult> RegisterUser(
        RegisterUserRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUser(
            request.UserManagmentId,
            request.FirstName,
            request.LastName,
            request.UserName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.MobileNumber,
            request.Roles?.ToList());
        
        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Created($"{UsersConfigs.UsersPrefixUri}/{result.Value.UserIdentity?.Id}", result);
    }
}
