using System;
using System.Collections.Generic;
using System.Text;
using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.DeleteUrl;

public record DeleteUrlCommand(int Id) : IRequest<bool>;
