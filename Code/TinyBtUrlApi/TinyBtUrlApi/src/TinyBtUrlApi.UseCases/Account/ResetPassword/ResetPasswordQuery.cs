using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Account.ResetPassword;

public record ResetPasswordQuery(
    string Email,
    string Token,
    string NewPassword);
