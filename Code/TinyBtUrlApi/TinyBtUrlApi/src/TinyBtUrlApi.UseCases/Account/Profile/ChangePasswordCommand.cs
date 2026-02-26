using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Account.Profile;

public record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword);
