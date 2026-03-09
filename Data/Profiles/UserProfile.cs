using AutoMapper;
using community_api.Data.DTO;
using community_api.Data.Entities;

namespace community_api.Data.Profiles
{
    // AutoMapper-profil för mappning mellan AppUser-entiteter och User-DTO:s
    // Registreras automatiskt av AddAutoMapper(typeof(Program)) i Program.cs
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // UpdateUserDto -> AppUser: hoppar över null-fält (partial update)
            // Används när användaren uppdaterar sin profil
            CreateMap<UpdateUserDto, AppUser>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            // RegisterUserDto -> AppUser: direkt mappning for registrering
            CreateMap<RegisterUserDto, AppUser>();

            // AppUser -> UserResponseDto: kombinerar förnamn och efternamn till FullName
            CreateMap<AppUser, UserResponseDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));
        }
    }
}
