using MsftFramework.Core.Exception.Types;

namespace MS.Services.Identity.Shared.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string emailOrUserName) : base($"User with email or username: '{emailOrUserName}' not found.")
    {
    }

    public UserNotFoundException(Guid id) : base($"User with id: '{id}' not found.")
    {
    }
}
