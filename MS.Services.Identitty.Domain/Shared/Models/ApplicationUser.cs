using Microsoft.AspNetCore.Identity;

namespace MS.Services.Identity.Shared.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public long UserManagmentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? LastLoggedInAt { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public UserState UserState { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MobileNumber { get; set; } = default;
}
