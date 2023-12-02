using Ardalis.GuardClauses;
using MS.Services.Identity.Shared.Exceptions;
using MS.Services.Identity.Shared.Models;
using FluentValidation;
using MsftFramework.Abstractions.CQRS.Command;
using MsftFramework.Abstractions.Messaging.Outbox;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MsftFramework.Core.Exception;
using MediatR;
using FluentResults;

namespace MS.Services.Identity.Users.Features.UpdatingUserState;

public record UpdateUserState(Guid UserId, UserState State) : ITxUpdateCommand<Result<Unit>>;

internal class UpdateUserStateValidator : AbstractValidator<UpdateUserState>
{
    public UpdateUserStateValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(v => v.State)
            .NotEmpty();

        RuleFor(v => v.UserId)
            .NotEmpty();
    }
}

internal class UpdateUserStateHandler : ICommandHandler<UpdateUserState>
{
    private readonly ILogger<UpdateUserStateHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOutboxService _outboxService;

    public UpdateUserStateHandler(
        IOutboxService outboxService,
        UserManager<ApplicationUser> userManager,
        ILogger<UpdateUserStateHandler> logger)
    {
        _logger = logger;
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _outboxService = Guard.Against.Null(outboxService, nameof(outboxService));
    }

    public async Task<Result<Unit>> Handle(UpdateUserState request, CancellationToken cancellationToken)
    {
        var res = new Result();
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.Against.Null(user, new UserNotFoundException(request.UserId));

        var previousState = user.UserState;
        if (previousState == request.State)
        {
            return res.ToResult(Unit.Value);
        }

        if (await _userManager.IsInRoleAsync(user, Constants.Role.Admin))
        {
            throw new UserStateCannotBeChangedException(request.State, request.UserId);
        }

        user.UserState = request.State;

        await _userManager.UpdateAsync(user);

        await _outboxService.SaveAsync(
            new UserStateUpdated(request.UserId, previousState, request.State),
            cancellationToken);

        _logger.LogInformation(
            "Updated state for user with ID: '{UserId}', '{PreviousState}' -> '{UserState}'",
            user.Id,
            previousState,
            user.UserState);

        return res.ToResult(Unit.Value);
    }
}
