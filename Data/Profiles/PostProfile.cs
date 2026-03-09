using AutoMapper;
using community_api.Data.DTO;
using community_api.Data.Entities;

namespace community_api.Data.Profiles
{
    // AutoMapper-profil för mappning mellan Post-entiteter och Post-DTO:s
    // Registreras automatiskt av AddAutoMapper(typeof(Program)) i Program.cs
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            // PostAddDto -> Post: mappas direkt (titeln och texten matchar)
            CreateMap<PostAddDto, Post>();

            // Post -> PostAddResponseDto: mappas kategorinamnet från navigeringsegenskapen
            CreateMap<Post, PostAddResponseDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.CategoryName));

            // Post -> PostsGetDto: mappas id, användarnamn och kategorinamn från navigeringsegenskaper
            CreateMap<Post, PostsGetDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.CategoryName));

            // PostUpdateDto -> Post: ignorerar id, ägare och navigeringsegenskaper
            // ForAllMembers: hoppar över null-fält (partial update)
            CreateMap<PostUpdateDto, Post>()
                .ForMember(d => d.PostId, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.User, opt => opt.Ignore())
                .ForMember(d => d.Category, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Post -> PostUpdateResponseDto: mappas id, kategorinamn och ägar-id
            CreateMap<Post, PostUpdateResponseDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PostId))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId));

            // Post -> PostsGetDetailsDto: mappas id, kategorinamn och användarnamn
            CreateMap<Post, PostsGetDetailsDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PostId))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => src.User.UserName));
        }
    }
}
