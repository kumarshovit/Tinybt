using System;
using System.Collections.Generic;
using System.Text;
using Ardalis.SharedKernel;
namespace TinyBtUrlApi.Core.Models;

public class PasswordResetToken : IAggregateRoot
{
  public int Id { get; set; }

  public string Token { get; set; } = string.Empty;

  public int UserId { get; set; }

  public DateTime ExpiresAt { get; set; }

  public bool IsUsed { get; set; } = false;
}
