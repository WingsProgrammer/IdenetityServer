using MsftFramework.Core.Exception.Types;

namespace MS.Services.Identity.Identity.Exceptions;

public class RefreshTokenNotFoundException : NotFoundException
{
    public RefreshTokenNotFoundException() : base("Refresh token not found.")
    {
    }
}
