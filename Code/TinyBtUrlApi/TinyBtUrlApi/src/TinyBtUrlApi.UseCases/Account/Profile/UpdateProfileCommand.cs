using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Account.Profile;

public record UpdateProfileCommand(int UserId, string FullName);
