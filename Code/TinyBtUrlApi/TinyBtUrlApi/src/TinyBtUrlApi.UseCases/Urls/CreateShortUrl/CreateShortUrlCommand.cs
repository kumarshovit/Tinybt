using System;
using System.Collections.Generic;
using System.Text;
using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.CreateShortUrl;

public record CreateShortUrlCommand(
    string LongUrl,
    string? CustomAlias,
    DateTime? ExpirationDate
) : IRequest<CreateShortUrlResult>;
