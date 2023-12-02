using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ardalis.GuardClauses;
using MS.Services.Identity.Identity.Exceptions;
using MS.Services.Identity.Identity.Features.GenerateJwtToken;
using MS.Services.Identity.Identity.Features.GenerateRefreshToken;
using MS.Services.Identity.Shared.Exceptions;
using MS.Services.Identity.Shared.Models;
using FluentValidation;
using MsftFramework.Abstractions.CQRS.Command;
using MsftFramework.Security.Jwt;
using Microsoft.AspNetCore.Identity;
using FluentResults;

namespace MS.Services.Identity.Identity.Features.RefreshingToken;

public record RefreshToken(string AccessTokenData, string RefreshTokenData) : ICommand<Result<RefreshTokenResult>>;

internal class RefreshTokenValidator : AbstractValidator<RefreshToken>
{
    public RefreshTokenValidator()
    {
        RuleFor(v => v.AccessTokenData)
            .NotEmpty();

        RuleFor(v => v.RefreshTokenData)
            .NotEmpty();
    }
}

internal class RefreshTokenHandler : ICommandHandler<RefreshToken, RefreshTokenResult>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IJwtHandler _jwtHandler;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenHandler(
        IJwtHandler jwtHandler,
        UserManager<ApplicationUser> userManager,
        ICommandProcessor commandProcessor)
    {
        _jwtHandler = jwtHandler;
        _userManager = userManager;
        _commandProcessor = commandProcessor;
    }

    public async Task<Result<RefreshTokenResult>> Handle(RefreshToken request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(RefreshToken));

        // invalid token/signing key was passed and we can't extract user claims
        var userClaimsPrincipal = _jwtHandler.ValidateToken(request.AccessTokenData);

        if (userClaimsPrincipal is null)
            throw new InvalidTokenException();

        var userId = userClaimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.NameId);

        var identityUser = await _userManager.FindByIdAsync(userId);

        if (identityUser == null)
            throw new UserNotFoundException(userId);

        var refreshToken =
            (await _commandProcessor.SendAsync(
                new GenerateRefreshTokenCommand { UserId = identityUser.Id, Token = request.RefreshTokenData },
                cancellationToken)).Value.RefreshToken;

        var jsonWebToken =
            (await _commandProcessor.SendAsync(
                new GenerateJwtTokenCommand(identityUser, refreshToken.Token), cancellationToken)).Value.JsonWebToken;
        var res = new Result();
        return res.ToResult(new RefreshTokenResult(identityUser, jsonWebToken, refreshToken.Token));
    }
}

public record RefreshTokenResult
{
    public RefreshTokenResult(ApplicationUser user, JsonWebToken jwtToken, string refreshToken)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.UserName;
        JsonWebToken = jwtToken;
        RefreshToken = refreshToken;
    }

    public JsonWebToken JsonWebToken { get; }
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Username { get; }
    public string RefreshToken { get; }
}
