using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Account.ForgotPassword;

public record ForgotPasswordResult(bool Success, string Message);
