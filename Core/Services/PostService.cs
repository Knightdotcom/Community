
using AutoMapper;
using community_api.Core.Interfaces;
using community_api.Data.DTO;
using community_api.Data.Entities;
using community_api.Data.Interfaces;

namespace community_api.Core.Services
{
    // Implementerar affärslogik för inläggshantering
    // Delegerar databasoperationer till IPostRepo (repository-lagret)
    public class PostService : IPostService
    {
        // Privata fält för AutoMapper och post-repot - injiceras via DI
        private readonly IMapper _mapper;
        private readonly IPostRepo _repo;

        // Konstruktor - tar emot IMapper och IPostRepo via Dependency Injection
        public PostService(IMapper mapper, IPostRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        // Skapar ett nytt inlägg och kopplar det till den inloggade användaren
        // Hämtar det skapade inlägget igen för att få med kategorinamnet
        public async Task<ServiceResult<PostAddResponseDto>> AddPostAsync(PostAddDto dto, string userId)
        {
            // Mappar PostAddDto till Post-entitet och sätter ägar-id
            var entity = _mapper.Map<Post>(dto);
            entity.UserId = userId;

            // Sparar inlägget i databasen via repot
            var result = await _repo.AddPostAsync(entity);

            // Om sparandet misslyckades - returnera felmeddelande
            if (result is null)
                return ServiceResult<PostAddResponseDto>.Fail("Det gick inte att spara inlagget");

            // Hämtar det skapade inlägget igen för att få med kategorin (Include)
            var createdEntity = await _repo.GetPostByIdAsync(result.PostId);

            // Om återhämtningen misslyckades - returnera felmeddelande
            if (createdEntity is null)
                return ServiceResult<PostAddResponseDto>.Fail("Det gick inte att hamta det skapade inlagget");

            // Mappar till response-DTO och sätter kategorinamnet manuellt
            var responseDto = _mapper.Map<PostAddResponseDto>(createdEntity);
            responseDto = responseDto with { Category = createdEntity.Category?.CategoryName };

            return ServiceResult<PostAddResponseDto>.Ok(responseDto);
        }

        // Hämtar alla inlägg med valfri filtrering på titel och kategorinamn
        public async Task<ServiceResult<List<PostsGetDto>>> GetPostsAsync(PostSearchFilterDto filter)
        {
            // Hämtar filtrerade inlägg från repot och mappar till DTO-lista
            var result = await _repo.GetPostsAsync(filter);
            var resDto = _mapper.Map<List<PostsGetDto>>(result);
            return ServiceResult<List<PostsGetDto>>.Ok(resDto);
        }

        // Hämtar ett specifikt inlägg med kommentarer och författarinformation
        public async Task<ServiceResult<PostsGetDetailsDto>> GetDetailedPostAsync(int id)
        {
            // Hämtar det detaljerade inlägget från repot (med Include för kommentarer och användare)
            var result = await _repo.GetDetailedPostAsync(id);

            // Om inlägget inte hittades - returnera felmeddelande
            if (result is null)
                return ServiceResult<PostsGetDetailsDto>.Fail("Inlagget hittades inte");

            // Mappar entiteten till details-DTO och returnerar
            var resDto = _mapper.Map<PostsGetDetailsDto>(result);
            return ServiceResult<PostsGetDetailsDto>.Ok(resDto);
        }

        // Uppdaterar ett befintligt inlägg
        // Kontrollerar ägarskap och att kategorin finns om den ändras
        public async Task<ServiceResult<PostUpdateResponseDto>> UpdatePostAsync(PostUpdateDto dto, int postId, string userId)
        {
            // Hämtar inlägget från databasen
            var dbEntity = await _repo.GetPostByIdAsync(postId);

            // Om inlägget inte hittades - returnera felmeddelande
            if (dbEntity is null)
                return ServiceResult<PostUpdateResponseDto>.Fail($"Inlagg med id {postId} hittades inte");

            // Kontrollerar att den inloggade användaren äger inlägget
            if (userId != dbEntity.UserId)
                return ServiceResult<PostUpdateResponseDto>.Fail("Du kan inte redigera nagon annans inlagg");

            // Om kategorin ändras - kontrollera att den nya kategorin finns i databasen
            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _repo.CategoryExistsAsync(dto.CategoryId.Value);
                if (!categoryExists)
                    return ServiceResult<PostUpdateResponseDto>.Fail($"Kategori med id {dto.CategoryId} finns inte");
            }

            // Mappar uppdaterade fält från DTO till befintlig entitet
            _mapper.Map(dto, dbEntity);

            // Sparar ändringarna i databasen
            var result = await _repo.SaveChangesAsync();

            // Om inga rader ändrades - returnera felmeddelande
            if (result == 0)
                return ServiceResult<PostUpdateResponseDto>.Fail("Inlagget uppdaterades inte");

            // Hämtar det uppdaterade inlägget och mappar till response-DTO
            var updatedEntity = await _repo.GetPostByIdAsync(dbEntity.PostId);
            var resultDto = _mapper.Map<PostUpdateResponseDto>(updatedEntity);

            return ServiceResult<PostUpdateResponseDto>.Ok(resultDto);
        }

        // Tar bort ett inlägg permanent
        // Kontrollerar att den inloggade användaren äger inlägget
        public async Task<ServiceResult<string>> DeletePostAsync(int postId, string userId)
        {
            // Hämtar inlägget från databasen
            var entity = await _repo.GetPostByIdAsync(postId);

            // Om inlägget inte hittades - returnera felmeddelande
            if (entity is null)
                return ServiceResult<string>.Fail("Inlagget hittades inte");

            // Kontrollerar att den inloggade användaren äger inlägget
            if (entity.UserId != userId)
                return ServiceResult<string>.Fail("Du kan inte ta bort nagon annans inlagg");

            // Tar bort inlägget (repot tar bort kommentarerna först)
            var result = await _repo.DeletePostAsync(entity);

            // Returnerar bekräftelse eller felmeddelande beroende på resultat
            return result
                ? ServiceResult<string>.Ok("Inlagget har tagits bort")
                : ServiceResult<string>.Fail("Nagot gick fel vid borttagning");
        }
    }
}
