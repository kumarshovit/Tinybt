using System;
using System.Collections.Generic;
using System.Text;
using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.UpdateDestination;

public record UpdateDestinationCommand(int Id, string NewLongUrl) : IRequest<bool>;
