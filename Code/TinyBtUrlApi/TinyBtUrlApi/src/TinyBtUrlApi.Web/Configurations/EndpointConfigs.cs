using TinyBtUrlApi.Web.Auth.Create;
using TinyBtUrlApi.Web.Profile.Delete;
using TinyBtUrlApi.Web.Profile.Read;
using TinyBtUrlApi.Web.Profile.Update;

namespace TinyBtUrlApi.Web.Configurations;

public static class EndpointConfigs
{
  public static WebApplication UseEndpointConfigs(this WebApplication app)
  {
    app.UseFastEndpoints();

    app.MapForgotPasswordEndpoint();
    app.MapLogoutEndpoint();
    app.MapGoogleLoginEndpoint();
    app.MapResetPasswordEndpoint();
    app.MapGetProfileEndpoint();
    app.MapUpdateNameEndpoint();
    app.MapChangePasswordEndpoint();
    app.MapDeleteAccountEndpoint();

    app.UseSwaggerGen();

    return app;
  }
}
