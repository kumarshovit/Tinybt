using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.UseCases.Urls.SearchByTag;

public record SearchByTagQuery(string Tag) : IRequest<List<UrlMapping>>;
