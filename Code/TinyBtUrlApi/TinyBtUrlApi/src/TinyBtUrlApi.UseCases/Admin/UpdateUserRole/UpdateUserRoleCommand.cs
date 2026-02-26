namespace TinyBtUrlApi.UseCases.Admin.UpdateUserRole;

public record UpdateUserRoleCommand(int UserId, string NewRole);
