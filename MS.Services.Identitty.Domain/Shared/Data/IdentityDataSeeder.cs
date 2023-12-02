using MS.Services.Identity.Shared.Models;
using MsftFramework.Abstractions.Persistence;
using Microsoft.AspNetCore.Identity;
using MsftFramework.Core.IdsGenerator;

namespace MS.Services.Identity.Shared.Data;

public class IdentityDataSeeder : IDataSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityDataSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        if (await _roleManager.RoleExistsAsync(ApplicationRole.Admin.Name) == false)
            await _roleManager.CreateAsync(ApplicationRole.Admin);

        if (await _roleManager.RoleExistsAsync(ApplicationRole.User.Name) == false)
            await _roleManager.CreateAsync(ApplicationRole.User);
    }

    private async Task SeedUsers()
    {

        var user = new ApplicationUser
        {
            UserManagmentId = SnowFlakIdGenerator.NewId(),
            UserName = "user1",
            FirstName = "bill",
            LastName = "andeson",
            PhoneNumber = "*",
            MobileNumber = "*",
            Email = "user@test.com",
        };

        var result = await _userManager.CreateAsync(user, "123456");

        if (result.Succeeded) await _userManager.AddToRoleAsync(user, ApplicationRole.Admin.Name);


    }
}
