namespace MS.Services.Identity.Users.Features.RegisteringUser;

public record RegisterUserRequest(
    long UserManagmentId,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string MobileNumber)
{
    public IEnumerable<string> Roles { get; init; } = new List<string> { Constants.Role.User };
}
