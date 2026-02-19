using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public record RedirectUrlQuery(string ShortCode) : IRequest<UrlMapping?>;
