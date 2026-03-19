using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Admin.AdminAnalysis;

public class GetUsersByBrowserQuery
    : IRequest<List<UsersByBrowserDto>>
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
}
