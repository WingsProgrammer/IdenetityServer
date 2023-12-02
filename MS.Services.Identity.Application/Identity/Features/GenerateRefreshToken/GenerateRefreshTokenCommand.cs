using Ardalis.GuardClauses;
using MS.Services.Identity.Identity.Dtos;
using MS.Services.Identity.Identity.Features.RefreshingToken;
using MS.Services.Identity.Shared.Data;
using MsftFramework.Abstractions.CQRS.Command;
using MsftFramework.Core.Extensions.Utils;
using Microsoft.EntityFrameworkCore;
using FluentResults;

namespace MS.Services.Identity.Identity.Features.GenerateRefreshToken;

public record GenerateRefreshTokenCommand : ICommand<Result<GenerateRefreshTokenCommandResult>>
{
    public Guid UserId { get; init; }
    public string Token { get; init; }
}

public class
    GenerateRefreshTokenCommandHandler : ICommandHandler<GenerateRefreshTokenCommand, GenerateRefreshTokenCommandResult>
{
    private readonly IdentityContext _context;

    public GenerateRefreshTokenCommandHandler(IdentityContext context)
    {
        _context = context;
    }

    public async Task<Result<GenerateRefreshTokenCommandResult>> Handle(
        GenerateRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GenerateRefreshTokenCommand));

        var refreshToken = await _context.Set<global::MS.Services.Identity.Shared.Models.RefreshToken>()
            .FirstOrDefaultAsync(
                rt => rt.UserId == request.UserId && rt.Token == request.Token,
                cancellationToken);

        if (refreshToken == null)
        {
            var token = global::MS.Services.Identity.Shared.Models.RefreshToken.GetRefreshToken();

            refreshToken = new global::MS.Services.Identity.Shared.Models.RefreshToken
            {
                UserId = request.UserId,
                Token = token,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(30),
                CreatedByIp = IpHelper.GetIpAddress()
            };

            await _context.Set<global::MS.Services.Identity.Shared.Models.RefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (!refreshToken.IsRefreshTokenValid())
                throw new InvalidRefreshTokenException();

            var token = global::MS.Services.Identity.Shared.Models.RefreshToken.GetRefreshToken();

            refreshToken.Token = token;
            refreshToken.ExpiredAt = DateTime.Now;
            refreshToken.CreatedAt = DateTime.Now.AddDays(10);
            refreshToken.CreatedByIp = IpHelper.GetIpAddress();

            _context.Set<global::MS.Services.Identity.Shared.Models.RefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // remove old refresh tokens from user
        // we could also maintain them on the database with changing their revoke date
        await RemoveOldRefreshTokens(request.UserId);
        var res = new Result();
        return res.ToResult( new GenerateRefreshTokenCommandResult(new RefreshTokenDto
        {
            Token = refreshToken.Token,
            CreatedAt = refreshToken.CreatedAt,
            ExpireAt = refreshToken.ExpiredAt,
            UserId = refreshToken.UserId,
            CreatedByIp = refreshToken.CreatedByIp,
            IsActive = refreshToken.IsActive,
            IsExpired = refreshToken.IsExpired,
            IsRevoked = refreshToken.IsRevoked,
            RevokedAt = refreshToken.RevokedAt
        }));
    }


    private Task RemoveOldRefreshTokens(Guid userId, long? ttlRefreshToken = null)
    {
        var refreshTokens = _context.Set<global::MS.Services.Identity.Shared.Models.RefreshToken>().Where(rt => rt.UserId == userId);

        refreshTokens.ToList().RemoveAll(x => !x.IsRefreshTokenValid(ttlRefreshToken));

        return _context.SaveChangesAsync();
    }
}
