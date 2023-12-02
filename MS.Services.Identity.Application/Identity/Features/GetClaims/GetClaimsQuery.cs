using MsftFramework.Abstractions.CQRS.Query;
using Microsoft.AspNetCore.Http;
using FluentResults;

namespace MS.Services.Identity.Identity.Features.GetClaims;

public class GetClaimsQuery : IQuery<Result<GetClaimsQueryResult>>
{
}

public class GetClaimsQueryHandler : IQueryHandler<GetClaimsQuery, GetClaimsQueryResult>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetClaimsQueryHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Result<GetClaimsQueryResult>> Handle(GetClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = _httpContextAccessor.HttpContext?.User.Claims.Select(x => new ClaimDto
        {
            Type = x.Type,
            Value = x.Value
        });
        var res = new Result();

        return Task.FromResult(res.ToResult(new GetClaimsQueryResult(claims)));
    }
}
