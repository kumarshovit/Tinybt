using System;
using System.Collections.Generic;
using System.Text;
namespace TinyBtUrlApi.Core.DTOs;

public class ContactMessageDto
{
  public string Name { get; set; } = default!;

  public string Email { get; set; } = default!;

  public string Subject { get; set; } = default!;

  public string Message { get; set; } = default!;
}
