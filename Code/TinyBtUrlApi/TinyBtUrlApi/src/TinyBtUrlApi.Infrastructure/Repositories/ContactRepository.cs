using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
  private readonly AppDbContext _context;

  public ContactRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task AddAsync(ContactMessage message)
  {
    _context.ContactMessages.Add(message);

    await _context.SaveChangesAsync();
  }
}
