using System;

namespace TinyBtUrlApi.Core.Models;

/// <summary>
/// Payload stored inside the short-lived access token.
/// All fields are server-set; the client never supplies these values.
/// </summary>
public record AccessTokenPayload(
    string ShortCode,
    string Source,
    long IssuedAtTicks);
