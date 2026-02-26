using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public record ResetPasswordDto(
    string Email,
    string Token,
    string NewPassword);
