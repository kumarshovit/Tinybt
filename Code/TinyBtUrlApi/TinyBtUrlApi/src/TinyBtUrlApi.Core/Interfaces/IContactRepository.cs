using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IContactRepository
{
  Task AddAsync(ContactMessage message);
}
