using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Entities;


namespace TinyBtUrlApi.UseCases.Urls.GetAllUrls;

public record GetAllUrlsQuery() : IRequest<List<UrlMapping>>;

