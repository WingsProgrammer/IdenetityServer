using MS.Services.Identity.Identity.Dtos;

namespace MS.Services.Identity.Identity.Features.GenerateRefreshToken;

public record GenerateRefreshTokenCommandResult(RefreshTokenDto RefreshToken);
