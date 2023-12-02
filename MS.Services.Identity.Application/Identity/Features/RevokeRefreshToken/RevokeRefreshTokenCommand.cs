using Ardalis.GuardClauses;
using MS.Services.Identity.Identity.Exceptions;
using MS.Services.Identity.Identity.Features.RefreshingToken;
using MS.Services.Identity.Shared.Data;
using MsftFramework.Abstractions.CQRS.Command;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentResults;

namespace MS.Services.Identity.Identity.Features.RevokeRefreshToken;

public record RevokeRefreshTokenCommand(string RefreshToken) : ICommand<Result<Unit>>;

internal class RevokeRefreshTokenCommandHandler : ICommandHandler<RevokeRefreshTokenCommand>
{
    private readonly IdentityContext _context;

    public RevokeRefreshTokenCommandHandler(IdentityContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(RevokeRefreshTokenCommand));

        var refreshToken = await _context.Set<global::MS.Services.Identity.Shared.Models.RefreshToken>()
            .SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken: cancellationToken);

        if (refreshToken == null)
            throw new RefreshTokenNotFoundException();

        if (refreshToken.IsRefreshTokenValid() == false)
            throw new InvalidRefreshTokenException();

        // revoke token and save
        refreshToken.RevokedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        var res = new Result();
        return res.ToResult(Unit.Value);
    }
}
