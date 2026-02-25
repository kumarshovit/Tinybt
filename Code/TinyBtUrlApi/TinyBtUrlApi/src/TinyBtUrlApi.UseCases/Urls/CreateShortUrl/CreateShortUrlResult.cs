using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.CreateShortUrl;


public record CreateShortUrlResult(
    int Id,
    string ShortCode,
    string LongUrl,
    DateTime? ExpirationDate,
    DateTime CreatedAt
);

