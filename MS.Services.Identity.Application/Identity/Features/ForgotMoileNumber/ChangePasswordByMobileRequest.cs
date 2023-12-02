using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services.Identity.Application.Identity.Features.ForgotMoileNumber;

public record ChangePasswordByMobileRequest
(string MobileNumber, string NewPassword, string ConfirmNewPassword, string VerifyCode);

