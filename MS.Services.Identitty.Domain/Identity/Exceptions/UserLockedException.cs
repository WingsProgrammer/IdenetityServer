using MsftFramework.Core.Exception.Types;

namespace MS.Services.Identity.Identity.Exceptions;

public class UserLockedException : BadRequestException
{
    public UserLockedException(string userId) : base($"userId '{userId}' has been locked.")
    {
    }
}
