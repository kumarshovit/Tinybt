//using Ardalis.SharedKernel;
//using TinyBtUrlApi.Core.Models;
//using TinyBtUrlApi.Core.DTOs;
//using TinyBtUrlApi.Core.Specifications;
//using TinyBtUrlApi.Core.Interfaces;

//namespace TinyBtUrlApi.UseCases.Account.Login;

//public class LoginHandler
//{
//  private readonly IRepository<User> _userRepository;
//  private readonly IRepository<RefreshToken> _refreshTokenRepository;
//  private readonly IRepository<IpLoginAttempt> _ipRepository;
//  private readonly IJwtService _jwtService;

//  public LoginHandler(
//      IRepository<User> userRepository,
//      IRepository<RefreshToken> refreshTokenRepository,
//      IRepository<IpLoginAttempt> ipRepository,
//      IJwtService jwtService)
//  {
//    _userRepository = userRepository;
//    _refreshTokenRepository = refreshTokenRepository;
//    _ipRepository = ipRepository;
//    _jwtService = jwtService;
//  }

//  public async Task<object> Handle(LoginQuery query)
//  {
//    var dto = query.Dto;
//    var ip = query.IpAddress;

//    var ipAttempt = await _ipRepository
//        .FirstOrDefaultAsync(new IpLoginAttemptByIpSpec(ip));

//    // 🔒 Already blocked
//    if (ipAttempt != null &&
//        ipAttempt.BlockedUntil != null &&
//        ipAttempt.BlockedUntil > DateTime.UtcNow)
//    {
//      return new
//      {
//        message = $"Too many attempts. Try again after {ipAttempt.BlockedUntil}",
//        requireCaptcha = true
//      };
//    }

//    var email = dto.Email!.Trim().ToLower();

//    var user = await _userRepository
//        .FirstOrDefaultAsync(new UserByEmailSpec(email));

//    if (user != null && user.IsDeleted)
//    {
//      return new
//      {
//        message = "Your account has been deleted. Please contact support."
//      };
//    }

//    // ❌ Login Failure
//    if (user == null ||
//        !BCrypt.Net.BCrypt.Verify(dto.Password!, user.PasswordHash))
//    {

//      if (ipAttempt == null)
//      {
//        ipAttempt = new IpLoginAttempt
//        {
//          IpAddress = ip,
//          AttemptCount = 1,
//          LastAttemptAt = DateTime.UtcNow
//        };

//        await _ipRepository.AddAsync(ipAttempt);
//      }
//      else
//      {
//        ipAttempt.AttemptCount++;
//        ipAttempt.LastAttemptAt = DateTime.UtcNow;

//        if (ipAttempt.AttemptCount >= 5)
//        {
//          ipAttempt.BlockedUntil = DateTime.UtcNow.AddMinutes(10);

//          await _ipRepository.UpdateAsync(ipAttempt);

//          return new
//          {
//            message = "Too many attempts. You are blocked for 10 minutes.",
//            requireCaptcha = true
//          };
//        }

//        await _ipRepository.UpdateAsync(ipAttempt);
//      }

//      return new
//      {
//        message = "Invalid email or password."
//      };
//    }

//    if (!user.IsActive)
//    {
//      return new
//      {
//        message = "Your account has been disabled by admin"
//      };
//    }
//    // 🔄 Reset IP attempts on success
//    if (ipAttempt != null)
//    {
//      ipAttempt.AttemptCount = 0;
//      ipAttempt.BlockedUntil = null;
//      await _ipRepository.UpdateAsync(ipAttempt);
//    }

//    if (!user.IsEmailVerified)
//    {
//      return new
//      {
//        message = "Please verify your email before logging in."
//      };
//    }

//    // 🔐 Generate tokens
//    var (token, expires) = _jwtService.Generate(user);

//    var refreshTokenValue = Guid.NewGuid().ToString();

//    var refreshToken = new RefreshToken
//    {
//      Token = refreshTokenValue,
//      UserId = user.Id,
//      ExpiresAt = DateTime.UtcNow.AddDays(7),
//      IsRevoked = false
//    };

//    await _refreshTokenRepository.AddAsync(refreshToken);

//    return new
//    {
//      accessToken = token,
//      refreshToken = refreshTokenValue,
//      expires = expires
//    };
//  }
//}


//using Ardalis.SharedKernel;
//using TinyBtUrlApi.Core.Models;
//using TinyBtUrlApi.Core.DTOs;
//using TinyBtUrlApi.Core.Specifications;
//using TinyBtUrlApi.Core.Interfaces;

//namespace TinyBtUrlApi.UseCases.Account.Login;

//public class LoginHandler
//{
//  private readonly IRepository<User> _userRepository;
//  private readonly IRepository<RefreshToken> _refreshTokenRepository;
//  private readonly IRepository<IpLoginAttempt> _ipRepository;
//  private readonly IJwtService _jwtService;

//  public LoginHandler(
//      IRepository<User> userRepository,
//      IRepository<RefreshToken> refreshTokenRepository,
//      IRepository<IpLoginAttempt> ipRepository,
//      IJwtService jwtService)
//  {
//    _userRepository = userRepository;
//    _refreshTokenRepository = refreshTokenRepository;
//    _ipRepository = ipRepository;
//    _jwtService = jwtService;
//  }

//  public async Task<object> Handle(LoginQuery query)
//  {
//    var dto = query.Dto;
//    var ip = query.IpAddress;

//    var ipAttempt = await _ipRepository
//        .FirstOrDefaultAsync(new IpLoginAttemptByIpSpec(ip));

//    var now = DateTime.UtcNow;

//    // ✅ FIX 1: Reset attempts if block expired
//    if (ipAttempt != null &&
//        ipAttempt.BlockedUntil != null &&
//        ipAttempt.BlockedUntil <= now)
//    {
//      ipAttempt.AttemptCount = 0;
//      ipAttempt.BlockedUntil = null;
//      ipAttempt.LastAttemptAt = now;

//      await _ipRepository.UpdateAsync(ipAttempt);
//    }

//    // 🔒 Already blocked
//    if (ipAttempt != null &&
//        ipAttempt.BlockedUntil != null &&
//        ipAttempt.BlockedUntil > now)
//    {
//      return new
//      {
//        message = $"Too many attempts. Try again after {ipAttempt.BlockedUntil}",
//        requireCaptcha = true
//      };
//    }

//    var email = dto.Email!.Trim().ToLower();

//    var user = await _userRepository
//        .FirstOrDefaultAsync(new UserByEmailSpec(email));

//    // 🚫 Deleted user
//    if (user != null && user.IsDeleted)
//    {
//      return new
//      {
//        message = "Your account has been deleted. Please contact support."
//      };
//    }

//    // ❌ Login Failure
//    if (user == null ||
//        user.PasswordHash == null ||
//        !BCrypt.Net.BCrypt.Verify(dto.Password!, user.PasswordHash))
//    {
//      if (ipAttempt == null)
//      {
//        ipAttempt = new IpLoginAttempt
//        {
//          IpAddress = ip,
//          AttemptCount = 1,
//          LastAttemptAt = now
//        };

//        await _ipRepository.AddAsync(ipAttempt);
//      }
//      else
//      {
//        ipAttempt.AttemptCount++;
//        ipAttempt.LastAttemptAt = now;

//        if (ipAttempt.AttemptCount >= 5)
//        {
//          ipAttempt.BlockedUntil = now.AddMinutes(1);

//          await _ipRepository.UpdateAsync(ipAttempt);

//          return new
//          {
//            message = "Too many attempts. You are blocked for 1 minute.",
//            requireCaptcha = true
//          };
//        }

//        await _ipRepository.UpdateAsync(ipAttempt);
//      }

//      return new
//      {
//        message = "Invalid email or password."
//      };
//    }

//    // 🚫 Account disabled
//    if (!user.IsActive)
//    {
//      return new
//      {
//        message = "Your account has been disabled by admin"
//      };
//    }

//    // 🔄 Reset attempts on successful login
//    if (ipAttempt != null)
//    {
//      ipAttempt.AttemptCount = 0;
//      ipAttempt.BlockedUntil = null;
//      ipAttempt.LastAttemptAt = now;

//      await _ipRepository.UpdateAsync(ipAttempt);
//    }

//    // 📧 Email verification check
//    if (!user.IsEmailVerified)
//    {
//      return new
//      {
//        message = "Please verify your email before logging in."
//      };
//    }

//    // 🔐 Generate tokens
//    var (token, expires) = _jwtService.Generate(user);

//    var refreshTokenValue = Guid.NewGuid().ToString();

//    var refreshToken = new RefreshToken
//    {
//      Token = refreshTokenValue,
//      UserId = user.Id,
//      ExpiresAt = now.AddDays(7),
//      IsRevoked = false
//    };

//    await _refreshTokenRepository.AddAsync(refreshToken);

//    return new
//    {
//      accessToken = token,
//      refreshToken = refreshTokenValue,
//      expires = expires
//    };
//  }
//}


using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Specifications;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Account.Login;

public class LoginHandler
{
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<RefreshToken> _refreshTokenRepository;
  private readonly IRepository<IpLoginAttempt> _ipRepository;
  private readonly IJwtService _jwtService;

  public LoginHandler(
      IRepository<User> userRepository,
      IRepository<RefreshToken> refreshTokenRepository,
      IRepository<IpLoginAttempt> ipRepository,
      IJwtService jwtService)
  {
    _userRepository = userRepository;
    _refreshTokenRepository = refreshTokenRepository;
    _ipRepository = ipRepository;
    _jwtService = jwtService;
  }

  public async Task<object> Handle(LoginQuery query)
  {
    var dto = query.Dto;
    var ip = query.IpAddress;
    var now = DateTime.UtcNow;

    var ipAttempt = await _ipRepository
        .FirstOrDefaultAsync(new IpLoginAttemptByIpSpec(ip));

    // ✅ Reset attempts if block expired
    if (ipAttempt != null &&
        ipAttempt.BlockedUntil != null &&
        ipAttempt.BlockedUntil <= now)
    {
      ipAttempt.AttemptCount = 0;
      ipAttempt.BlockedUntil = null;
      ipAttempt.LastAttemptAt = now;

      await _ipRepository.UpdateAsync(ipAttempt);
    }

    // 🔒 Already blocked
    if (ipAttempt != null &&
        ipAttempt.BlockedUntil != null &&
        ipAttempt.BlockedUntil > now)
    {
      return new
      {
        message = $"Too many attempts. Try again after {ipAttempt.BlockedUntil}",
        requireCaptcha = true
      };
    }

    var email = dto.Email!.Trim().ToLower();

    var user = await _userRepository
        .FirstOrDefaultAsync(new UserByEmailSpec(email));

    // ❌ Unregistered email
    if (user == null)
    {
      await HandleFailedAttempt(ipAttempt, ip, now);

      return new
      {
        message = "You are not registered , Please register first."
      };
    }

    // 🚫 Deleted user
    if (user.IsDeleted)
    {
      return new
      {
        message = "Your account has been deleted. Please contact support."
      };
    }

    // ❌ Wrong password
    if (user.PasswordHash == null ||
        !BCrypt.Net.BCrypt.Verify(dto.Password!, user.PasswordHash))
    {
      await HandleFailedAttempt(ipAttempt, ip, now);

      return new
      {
        message = "Invalid Username or password."
      };
    }

    // 🚫 Account disabled
    if (!user.IsActive)
    {
      return new
      {
        message = "Your account has been disabled by admin"
      };
    }

    // 📧 Email verification check
    if (!user.IsEmailVerified)
    {
      return new
      {
        message = "Please verify your email before logging in."
      };
    }

    // 🔄 Reset attempts on successful login
    if (ipAttempt != null)
    {
      ipAttempt.AttemptCount = 0;
      ipAttempt.BlockedUntil = null;
      ipAttempt.LastAttemptAt = now;

      await _ipRepository.UpdateAsync(ipAttempt);
    }

    // 🔐 Generate tokens
    var (token, expires) = _jwtService.Generate(user);

    var refreshTokenValue = Guid.NewGuid().ToString();

    var refreshToken = new RefreshToken
    {
      Token = refreshTokenValue,
      UserId = user.Id,
      ExpiresAt = now.AddDays(7),
      IsRevoked = false
    };

    await _refreshTokenRepository.AddAsync(refreshToken);

    return new
    {
      accessToken = token,
      refreshToken = refreshTokenValue,
      expires = expires
    };
  }

  // 🔧 Helper for failed attempts
  private async Task HandleFailedAttempt(IpLoginAttempt? ipAttempt, string ip, DateTime now)
  {
    if (ipAttempt == null)
    {
      ipAttempt = new IpLoginAttempt
      {
        IpAddress = ip,
        AttemptCount = 1,
        LastAttemptAt = now
      };

      await _ipRepository.AddAsync(ipAttempt);
    }
    else
    {
      ipAttempt.AttemptCount++;
      ipAttempt.LastAttemptAt = now;

      if (ipAttempt.AttemptCount >= 5)
      {
        ipAttempt.BlockedUntil = now.AddMinutes(1);
      }

      await _ipRepository.UpdateAsync(ipAttempt);
    }
  }
}
