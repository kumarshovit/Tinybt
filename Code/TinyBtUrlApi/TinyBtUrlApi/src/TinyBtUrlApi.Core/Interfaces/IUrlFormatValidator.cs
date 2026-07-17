using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IUrlFormatValidator
{
    UrlSecurityResult ValidateFormat(string url);
}
