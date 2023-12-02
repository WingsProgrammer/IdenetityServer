using MsftFramework.Core.Domain.Events.External;

namespace MS.Services.Identity.Users.Features.RegisteringUser.Events.Integration;

public record UserRegistered(
    Guid IdentityId,
    long UserManagmentId,
    string Email,
    string UserName,
    string FirstName,
    string LastName,
    List<string>? Roles) : IntegrationEvent;
