using MsftFramework.Core.Exception.Types;

namespace MS.Services.Identity.Identity.Features.RefreshingToken;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() : base("access_token is invalid!")
    {
    }
}
