using AutoMapper;
using MS.Services.Identity.Shared.Models;
using MS.Services.Identity.Users.Dtos;
using FluentValidation;
using MsftFramework.Abstractions.CQRS.Query;
using MsftFramework.Core.Persistence.EfCore;
using MsftFramework.Core.Types;
using MsftFramework.CQRS.Query;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentResults;

namespace MS.Services.Identity.Users.Features.GettingUsers;

public record GetUsers : ListQuery<Result<GetUsersResult>>;


public class GetUsersValidator : AbstractValidator<GetUsers>
{
    public GetUsersValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetUsersHandler : IQueryHandler<GetUsers, GetUsersResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUsersHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result<GetUsersResult>> Handle(GetUsers request, CancellationToken cancellationToken)
    {
        
        var customer = await _userManager.Users
            .OrderByDescending(x => x.CreatedAt)
            .ApplyIncludeList(request.Includes)
            .ApplyFilterList(request.Filters)
            .AsNoTracking()
            .PaginateAsync<ApplicationUser, IdentityUserDto>(
                _mapper.ConfigurationProvider,
                request.Page,
                request.PageSize,
                cancellationToken: cancellationToken)
            ;
        var res = new Result();
        return res.ToResult(new GetUsersResult(customer));
    }
}

public record GetUsersResult(ListResultModel<IdentityUserDto> IdentityUsers);
