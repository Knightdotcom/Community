
using community_api.Core.Services;
using community_api.Data.DTO;

namespace community_api.Core.Interfaces
{
    // Kontrakt för användartjänsten
    // Implementeras av UserService och möjliggör Dependency Injection och testbarhet
    public interface IUserService
    {
        // Registrerar en ny användare och returnerar den skapade användaren
        Task<ServiceResult<UserResponseDto>> RegisterUserAsync(RegisterUserDto dto);

        // Tar bort en användare med angivet id och returnerar ett bekräftelsemeddelande
        Task<ServiceResult<string>> DeleteUserAsync(string id);

        // Uppdaterar en befintlig användares uppgifter och returnerar den uppdaterade användaren
        Task<ServiceResult<UserResponseDto>> UpdateUserAsync(string id, UpdateUserDto dto);
    }
}
