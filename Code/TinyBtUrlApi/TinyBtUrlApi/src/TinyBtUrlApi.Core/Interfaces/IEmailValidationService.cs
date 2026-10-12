using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IEmailValidationService
{
    Task<EmailValidationResult> ValidateEmailAsync(string email, CancellationToken cancellationToken = default);
}
