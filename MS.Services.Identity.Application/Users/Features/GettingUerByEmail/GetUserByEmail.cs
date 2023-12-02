using Ardalis.GuardClauses;
using AutoMapper;
using MS.Services.Identity.Shared.Models;
using MS.Services.Identity.Users.Dtos;
using FluentValidation;
using MsftFramework.Abstractions.CQRS.Query;
using Microsoft.AspNetCore.Identity;
using FluentResults;

namespace MS.Services.Identity.Users.Features.GettingUerByEmail;

public record GetUserByEmail(string Email) : IQuery<Result<GetUserByEmailResult>>;

internal class GetUserByIdValidator : AbstractValidator<GetUserByEmail>
{
    public GetUserByIdValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email address is not valid");
    }
}

public class GetUserByEmailHandler : IQueryHandler<GetUserByEmail, GetUserByEmailResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserByEmailHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _mapper = Guard.Against.Null(mapper, nameof(mapper));
    }

    public async Task<Result<GetUserByEmailResult>> Handle(GetUserByEmail query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var identityUser = await _userManager.FindByEmailAsync(query.Email);

        var userDto = _mapper.Map<IdentityUserDto>(identityUser);
        var res = new Result();
        return res.ToResult(new GetUserByEmailResult(userDto));
    }
}

public record GetUserByEmailResult(IdentityUserDto? UserIdentity);
