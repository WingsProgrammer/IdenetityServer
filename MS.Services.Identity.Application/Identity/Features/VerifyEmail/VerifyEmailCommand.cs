using Ardalis.GuardClauses;
using MS.Services.Identity.Identity.Features.VerifyEmail.Exceptions;
using MS.Services.Identity.Shared.Data;
using MS.Services.Identity.Shared.Exceptions;
using MS.Services.Identity.Shared.Models;
using MsftFramework.Abstractions.CQRS.Command;
using MsftFramework.Core.Exception.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using FluentResults;

namespace MS.Services.Identity.Identity.Features.VerifyEmail;

public record VerifyEmailCommand(string Email, string Code) : ICommand<Result<Unit>>;

internal class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityContext _dbContext;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        IdentityContext dbContext,
        ILogger<VerifyEmailCommandHandler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<Result<Unit>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(VerifyEmailCommand));

        try
        {
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var user = await _userManager.FindByIdAsync(request.Email);
            if (user == null)
            {
                throw new UserNotFoundException(request.Email);
            }

            if (user.EmailConfirmed)
            {
                throw new EmailAlreadyVerifiedException(user.Email);
            }

            var emailVerificationCode = await _dbContext.Set<EmailVerificationCode>()
                .Where(x => x.Email == request.Email && x.Code == request.Code && x.UsedAt == null)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (emailVerificationCode == null)
            {
                throw new BadRequestException("Either email or code is incorrect.");
            }

            if (DateTime.Now > emailVerificationCode.SentAt.AddMinutes(5))
            {
                throw new BadRequestException("The code is expired.");
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            emailVerificationCode.UsedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.Database.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Email verified successfully for userId:{UserId}", user.Id);
            var res = new Result();
            return res.ToResult(Unit.Value);
        }
        catch (Exception e)
        {
            await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
