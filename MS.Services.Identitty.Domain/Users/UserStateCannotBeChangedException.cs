using MS.Services.Identity.Shared.Models;
using MsftFramework.Core.Exception.Types;
using MsftFramework.Core.Extensions.Utils;

namespace MS.Services.Identity.Users.Features.UpdatingUserState;

public class UserStateCannotBeChangedException : AppException
{
    public UserState State { get; }
    public Guid UserId { get; }

    public UserStateCannotBeChangedException(UserState state, Guid userId)
        : base($"User state cannot be changed to: '{state.ToName()}' for user with ID: '{userId}'.")
    {
        State = state;
        UserId = userId;
    }
}
