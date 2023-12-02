using Ardalis.GuardClauses;
using FluentResults;
using KavehNegarSmsProvider;
using MediatR;
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

public record SendCodeForMobileCommand(string Mobile) : ICommand<Result<Unit>>;


public class SendCodeForMobileCommandHandler : ICommandHandler<SendCodeForMobileCommand, Unit>
{
    public ISMSProvider _smsProvider { get; }
    private readonly IdentityContext identityContext;

    public SendCodeForMobileCommandHandler(IdentityContext identityContext, ISMSProvider smsProvider)
    {
        this.identityContext = identityContext;
        _smsProvider = smsProvider;
    }


    public async Task<Result<Unit>> Handle(SendCodeForMobileCommand request, CancellationToken cancellationToken)
    {
        var result = new Result();

        var appUser = await identityContext.Set<ApplicationUser>().AsNoTracking().FirstOrDefaultAsync(x => x.MobileNumber == request.Mobile);

        Guard.Against.Null(appUser, nameof(appUser), "Mobile Number NotFound.");

        var verifyCode = new VerifyCode
        {
            CreateDate = DateTime.Now,
            ExpireDate = DateTime.Now.AddMinutes(5),
            MobileNumber = request.Mobile.StartsWith('0') ? request.Mobile : "0" + request.Mobile,
            VerifyCodeValue = VerifyCode.GenerateVerifyCode()
        };

        var sendResult = await _smsProvider.SendVerifyLookup(verifyCode.VerifyCodeValue, verifyCode.MobileNumber, "ResetPassword");

        verifyCode.SendDate = sendResult.Date.ToString();
        verifyCode.SendStatus = sendResult.Status.ToString();

        identityContext.Set<VerifyCode>().Add(verifyCode);
        await identityContext.SaveChangesAsync();

        if (sendResult.Status != 200)
        {
            result.Errors.Add(new Error("sms send result is not success"));
        }

        return result.ToResult(Unit.Value);
    }
}