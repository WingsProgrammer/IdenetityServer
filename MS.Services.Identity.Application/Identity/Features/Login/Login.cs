using Ardalis.GuardClauses;
using MS.Services.Identity.Identity.Exceptions;
using MS.Services.Identity.Identity.Features.GenerateJwtToken;
using MS.Services.Identity.Identity.Features.GenerateRefreshToken;
using MS.Services.Identity.Shared.Exceptions;
using MS.Services.Identity.Shared.Models;
using FluentValidation;
using MsftFramework.Abstractions.CQRS.Command;
using MsftFramework.Abstractions.CQRS.Query;
using MsftFramework.Abstractions.Persistence;
using MsftFramework.Security.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MsftFramework.Core.Exception;
using FluentResults;

namespace MS.Services.Identity.Identity.Features.Login;

public record Login(string UserNameOrEmail, string Password, bool Remember) :
    ICommand<Result<LoginResponse>>, ITxRequest;

internal class LoginValidator : AbstractValidator<Login>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("UserNameOrEmail cannot be empty");
        RuleFor(x => x.Password).NotEmpty().WithMessage("password cannot be empty");
    }
}

internal class LoginHandler : ICommandHandler<Login, LoginResponse>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IJwtHandler _jwtHandler;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<LoginHandler> _logger;
    private readonly IQueryProcessor _queryProcessor;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        ICommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IJwtHandler jwtHandler,
        IOptions<JwtOptions> jwtOptions,
        SignInManager<ApplicationUser> signInManager,
        ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _jwtHandler = jwtHandler;
        _signInManager = signInManager;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(Login request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(Login));

        var identityUser = await _userManager.FindByNameAsync(request.UserNameOrEmail) ??
                           await _userManager.FindByEmailAsync(request.UserNameOrEmail);

        Guard.Against.Null(identityUser, new UserNotFoundException(request.UserNameOrEmail));

        var signinResult = await _signInManager.PasswordSignInAsync(
            request.UserNameOrEmail,
            request.Password,
            request.Remember,
            false);

        if (signinResult.IsNotAllowed)
        {
            if (!await _userManager.IsEmailConfirmedAsync(identityUser))
                throw new EmailNotConfirmedException(identityUser.Email);

            if (!await _userManager.IsPhoneNumberConfirmedAsync(identityUser))
                throw new PhoneNumberNotConfirmedException(identityUser.PhoneNumber);
        }
        else if (signinResult.IsLockedOut)
        {
            throw new UserLockedException(identityUser.Id.ToString());
        }
        else if (signinResult.RequiresTwoFactor)
        {
            throw new RequiresTwoFactorException("Require two factor authentication.");
        }
        else if (signinResult.Succeeded == false)
        {
            throw new PasswordIsInvalidException("Password is invalid.");
        }

        var refreshToken =
            (await _commandProcessor.SendAsync(
                new GenerateRefreshTokenCommand { UserId = identityUser.Id },
                cancellationToken)).Value.RefreshToken;

        var jsonWebToken =
            (await _commandProcessor.SendAsync(
                new GenerateJwtTokenCommand(identityUser, refreshToken.Token),
                cancellationToken)).Value.JsonWebToken;

        _logger.LogInformation("User with ID: {ID} has been authenticated", identityUser.Id);

        // we can don't return value from command and get token from a short term session in our request with `TokenStorageService`

        var res = new Result();
        return res.ToResult(new LoginResponse(identityUser, jsonWebToken, refreshToken.Token));
    }
}

public class LoginResponse
{
    public LoginResponse(ApplicationUser user, JsonWebToken jwtToken, string refreshToken)
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
