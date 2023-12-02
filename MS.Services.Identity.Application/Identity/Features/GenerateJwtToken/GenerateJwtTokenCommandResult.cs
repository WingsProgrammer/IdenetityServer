using MsftFramework.Security.Jwt;

namespace MS.Services.Identity.Identity.Features.GenerateJwtToken;

public record GenerateJwtTokenCommandResult(JsonWebToken JsonWebToken);
