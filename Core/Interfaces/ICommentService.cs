using community_api.Core.Services;
using community_api.Data.DTO;

namespace community_api.Core.Interfaces
{
    public interface ICommentService
    {
        Task<ServiceResult<CommentAddResponseDto>> AddCommentAsync(int postId, CommentAddDto dto, string userId);
    }
}
