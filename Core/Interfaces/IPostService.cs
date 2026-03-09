
using community_api.Core.Services;
using community_api.Data.DTO;

namespace community_api.Core.Interfaces
{
    // Kontrakt för inläggstjänsten
    // Implementeras av PostService och möjliggör Dependency Injection och testbarhet
    public interface IPostService
    {
        // Skapar ett nytt inlägg kopplat till angiven användare
        Task<ServiceResult<PostAddResponseDto>> AddPostAsync(PostAddDto dto, string userId);

        // Hämtar alla inlägg med valfri filtrering på titel och kategori
        Task<ServiceResult<List<PostsGetDto>>> GetPostsAsync(PostSearchFilterDto filter);

        // Hämtar ett specifikt inlägg med kommentarer och författarinfo
        Task<ServiceResult<PostsGetDetailsDto>> GetDetailedPostAsync(int id);

        // Uppdaterar ett befintligt inlägg - kontrollerar att användaren är ägare
        Task<ServiceResult<PostUpdateResponseDto>> UpdatePostAsync(PostUpdateDto dto, int postId, string userId);

        // Tar bort ett inlägg permanent - kontrollerar att användaren är ägare
        Task<ServiceResult<string>> DeletePostAsync(int postId, string userId);
    }
}
