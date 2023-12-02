using MS.Services.Identity.Shared.Models;

namespace MS.Services.Identity.Users.Features.UpdatingUserState;

public record UpdateUserStateRequest
{
    public UserState UserState { get; init; }
}
