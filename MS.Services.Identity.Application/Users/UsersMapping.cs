using AutoMapper;
using MS.Services.Identity.Shared.Models;
using MS.Services.Identity.Users.Dtos;

namespace MS.Services.Identity.Users;

public class UsersMapping : Profile
{
    public UsersMapping()
    {
        CreateMap<ApplicationUser, IdentityUserDto>()
            .ForMember(x => x.RefreshTokens, opt => opt.MapFrom(x => x.RefreshTokens.Select(r => r.Token)));
       
    }
}
