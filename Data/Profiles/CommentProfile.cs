using AutoMapper;
using community_api.Data.DTO;
using community_api.Data.Entities;

namespace community_api.Data.Profiles
{
    // AutoMapper-profil för mappning mellan Comment-entiteter och Comment-DTO:s
    // Registreras automatiskt av AddAutoMapper(typeof(Program)) i Program.cs
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            // Comment -> CommentGetDto: mappas id och användarnamn från navigeringsegenskapen
            CreateMap<Comment, CommentGetDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CommentId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText));

            // Comment -> CommentAddResponseDto: direkt mappning (fältnamnen matchar)
            CreateMap<Comment, CommentAddResponseDto>();
        }
    }
}
