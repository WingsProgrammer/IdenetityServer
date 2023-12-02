namespace MS.Services.Identity.Identity.Features.RefreshingToken;

public record RefreshTokenRequest(string AccessToken, string RefreshToken);
