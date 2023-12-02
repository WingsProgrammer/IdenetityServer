using MsftFramework.Core.Exception.Types;


namespace MS.Services.Identity.Identity.Exceptions;

public class InvalidTokenException : BadRequestException
{
    public InvalidTokenException() : base("access_token is invalid!")
    {
    }
}
