using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MS.Services.Identitty.Domain.Shared.Models;
using MS.Services.Identity.Shared.Data;
using MsftFramework.Abstractions.CQRS.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services.Identity.Application.Identity.Features.ForgotMoileNumber;
  public record VerifyCodeForMobileCommand(string MobileNumber,string VerifyCode):ICommand<Result<Unit>>;


public class VerifyCodeForMobileCommandHandler : ICommandHandler<VerifyCodeForMobileCommand, Unit>
{
    private readonly IdentityContext identityDbContext;

    public VerifyCodeForMobileCommandHandler(IdentityContext identityDbContext)
    {
        this.identityDbContext = identityDbContext;
    }
    public async Task<Result<Unit>> Handle(VerifyCodeForMobileCommand request, CancellationToken cancellationToken)
    {
        var result = new Result();
        var vermob =await identityDbContext.Set<VerifyCode>().AsNoTracking().FirstOrDefaultAsync(x => x.MobileNumber == request.MobileNumber && x.VerifyCodeValue == request.VerifyCode);
        if (vermob==null)
        {
            result.Errors.Add(new Error("Undefined Error.... "));
        }
        return result.ToResult(Unit.Value);
    }}