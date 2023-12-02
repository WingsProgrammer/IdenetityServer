using Ardalis.GuardClauses;
using AutoMapper;
using MS.Services.Identity.Shared.Models;
using MS.Services.Identity.Users.Dtos;
using FluentValidation;
using MsftFramework.Abstractions.CQRS.Query;
using Microsoft.AspNetCore.Identity;
using FluentResults;

namespace MS.Services.Identity.Users.Features.GettingUserById;

public record GetUserById(Guid Id) : IQuery<Result<UserByIdResponse>>;

internal class GetUserByIdValidator : AbstractValidator<GetUserById>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}

public class GetUserByIdHandler : IQueryHandler<GetUserById, UserByIdResponse>
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _mapper = mapper;
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
    }

    public async Task<Result<UserByIdResponse>> Handle(GetUserById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var identityUser = await _userManager.FindByIdAsync(query.Id.ToString());

        var identityUserDto = _mapper.Map<IdentityUserDto>(identityUser);
        var res = new Result();
        return res.ToResult(new UserByIdResponse(identityUserDto));
    }
}

public record UserByIdResponse(IdentityUserDto IdentityUser);
