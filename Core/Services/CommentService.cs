
using AutoMapper;
using community_api.Core.Interfaces;
using community_api.Data.DTO;
using community_api.Data.Entities;
using community_api.Data.Interfaces;

namespace community_api.Core.Services
{
    // Implementerar affärslogik för kommentarshantering
    // Kontrollerar affärsregler: inlägget måste finnas och användaren kan inte kommentera sitt eget inlägg
    public class CommentService : ICommentService
    {
        // Privata fält för repos och AutoMapper - injiceras via DI
        private readonly ICommentRepo _commentRepo;
        private readonly IPostRepo _postRepo;
        private readonly IMapper _mapper;

        // Konstruktor - tar emot repos och IMapper via Dependency Injection
        public CommentService(ICommentRepo commentRepo, IPostRepo postRepo, IMapper mapper)
        {
            _commentRepo = commentRepo;
            _postRepo = postRepo;
            _mapper = mapper;
        }

        // Lägger till en kommentar på ett inlägg
        // Kontrollerar att inlägget finns och att användaren inte kommenterar sitt eget inlägg
        public async Task<ServiceResult<CommentAddResponseDto>> AddCommentAsync(int postId, CommentAddDto dto, string userId)
        {
            // Hämtar inlägget från databasen för att validera det
            var postEntity = await _postRepo.GetPostByIdAsync(postId);

            // Om inlägget inte hittades - returnera felmeddelande
            if (postEntity is null)
                return ServiceResult<CommentAddResponseDto>.Fail("Inlagget du forsoker kommentera finns inte");

            // Affärsregel: en användare kan inte kommentera sitt eget inlägg
            if (postEntity.UserId == userId)
                return ServiceResult<CommentAddResponseDto>.Fail("Du kan inte kommentera ditt eget inlagg");

            // Skapar kommentarentiteten med data från DTO och inloggad användare
            var commentEntity = new Comment
            {
                UserId = userId,
                CommentText = dto.CommentText,
                PostId = postId
            };

            // Sparar kommentaren i databasen via repot
            await _commentRepo.AddCommentAsync(commentEntity);

            // Mappar den skapade entiteten till response-DTO och returnerar
            var resDto = _mapper.Map<CommentAddResponseDto>(commentEntity);
            return ServiceResult<CommentAddResponseDto>.Ok(resDto);
        }
    }
}
