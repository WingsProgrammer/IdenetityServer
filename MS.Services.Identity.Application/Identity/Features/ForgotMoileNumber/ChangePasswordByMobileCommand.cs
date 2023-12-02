using Ardalis.GuardClauses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MS.Services.Identitty.Domain.Shared.Models;
using MS.Services.Identity.Shared.Data;
using MS.Services.Identity.Shared.Models;
using MsftFramework.Abstractions.CQRS.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services.Identity.Application.Identity.Features.ForgotMoileNumber;

public record ChangePasswordByMobileCommand(string MobileNumber, string NewPassword, string ConfirmNewPassword, string VerifyCode) : ICommand<Result<Unit>>;


public class ChangePasswordByMobileCommandHandler : ICommandHandler<ChangePasswordByMobileCommand>
{
    private readonly IdentityContext context;
    private readonly UserManager<ApplicationUser> _um;

    public ChangePasswordByMobileCommandHandler(IdentityContext context, UserManager<ApplicationUser> um)
    {
        this.context = context; _um = um;
    }
    public async Task<Result<Unit>> Handle(ChangePasswordByMobileCommand request, CancellationToken cancellationToken)
    {
        var result = new Result();

        var verify = await context.Set<VerifyCode>().FirstOrDefaultAsync(x => x.MobileNumber == request.MobileNumber && x.VerifyCodeValue == request.VerifyCode);
        
        Guard.Against.Null(verify, nameof(verify), "Code is not valid...");


        Guard.Against.AgainstExpression<bool>(x => x =true, verify.Used, "Invalid code");

        //Guard.Against.AgainstExpression(x => x > DateTime.Now, verify.CreateDate, "invalid Code");

        //Guard.Against.AgainstExpression(x => x < DateTime.Now, verify.ExpireDate, "invalid Code");

        var appUser = await context.Set<ApplicationUser>().FirstOrDefaultAsync(x => x.MobileNumber == request.MobileNumber);

        Guard.Against.Null(appUser, nameof(appUser), "Mobile not Valid...");


        if (request.NewPassword != request.ConfirmNewPassword)
        {
            result.Errors.Add(new Error("Passwords not Same"));
        }

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var hashedPassword = passwordHasher.HashPassword(appUser, request.NewPassword);

        verify.Used = true;
        var token = await _um.GeneratePasswordResetTokenAsync(appUser);
        var res= await _um.ResetPasswordAsync(appUser, token, request.NewPassword);

        await context.SaveChangesAsync();

        return result.ToResult(Unit.Value);
    }
}
