using MsftFramework.Core.Exception.Types;

namespace MS.Services.Identity.Users.Features.RegisteringUser;

public class RegisterIdentityUserException : BadRequestException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}
