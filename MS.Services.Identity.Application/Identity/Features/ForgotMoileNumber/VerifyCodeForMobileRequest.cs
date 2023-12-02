using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services.Identity.Application.Identity.Features.ForgotMoileNumber;
    public record VerifyCodeForMobileRequest(string Mobile,string VerifyCode);