using System;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IQrCodeService
{
    byte[] GenerateQrCode(string url);
}
