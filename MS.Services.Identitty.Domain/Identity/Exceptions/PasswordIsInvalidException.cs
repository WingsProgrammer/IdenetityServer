using MsftFramework.Core.Exception.Types;

namespace MS.Services.Identity.Identity.Exceptions;

public class PasswordIsInvalidException : AppException
{
    public PasswordIsInvalidException(string message) : base(message)
    {
    }
}
