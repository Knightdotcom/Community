
using AutoMapper;
using community_api.Core.Interfaces;
using community_api.Data.DTO;
using community_api.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace community_api.Core.Services
{
    // Implementerar affärslogik för användarhantering
    // Använder ASP.NET Core Identity (UserManager) för CRUD-operationer på användare
    public class UserService : IUserService
    {
        // Privata fält för Identity UserManager och AutoMapper - injiceras via DI
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        // Konstruktor - tar emot UserManager och IMapper via Dependency Injection
        public UserService(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // Registrerar en ny användare i systemet
        // Mappar DTO till entitet, skapar användaren med hashat lösenord via Identity
        public async Task<ServiceResult<UserResponseDto>> RegisterUserAsync(RegisterUserDto dto)
        {
            // Mappar RegisterUserDto till AppUser-entitet via AutoMapper
            var userEntity = _mapper.Map<AppUser>(dto);

            // Skapar användaren i databasen med Identity (hashar lösenordet automatiskt)
            var result = await _userManager.CreateAsync(userEntity, dto.Password);

            // Om skapandet misslyckades (t.ex. duplicerat användarnamn) - returnera fel
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<UserResponseDto>.Fail(errors);
            }

            // Mappar den skapade entiteten till response-DTO och returnerar den
            var response = _mapper.Map<UserResponseDto>(userEntity);
            return ServiceResult<UserResponseDto>.Ok(response);
        }

        // Tar bort en användare med angivet id från systemet
        public async Task<ServiceResult<string>> DeleteUserAsync(string id)
        {
            // Söker efter användaren via Identity med angivet id
            var user = await _userManager.FindByIdAsync(id);

            // Om användaren inte hittades - returnera felmeddelande
            if (user is null)
                return ServiceResult<string>.Fail("Anvandaren hittades inte");

            // Tar bort användaren via Identity
            var result = await _userManager.DeleteAsync(user);
            var errors = result.Errors.Select(e => e.Description).ToList();

            // Returnerar bekräftelse eller felmeddelanden beroende på resultat
            return result.Succeeded
                ? ServiceResult<string>.Ok("Anvandaren har tagits bort")
                : ServiceResult<string>.Fail(errors);
        }

        // Uppdaterar en befintlig användares uppgifter
        // Mappar DTOns icke-null-fält till entiteten via AutoMapper
        public async Task<ServiceResult<UserResponseDto>> UpdateUserAsync(string id, UpdateUserDto dto)
        {
            // Söker efter användaren via Identity med angivet id
            var userEntity = await _userManager.FindByIdAsync(id);

            // Om användaren inte hittades - returnera felmeddelande
            if (userEntity is null)
                return ServiceResult<UserResponseDto>.Fail("Anvandaren hittades inte");

            // Mappar uppdaterade fält från DTO till befintlig entitet
            _mapper.Map(dto, userEntity);

            // Sparar ändringarna via Identity
            var result = await _userManager.UpdateAsync(userEntity);

            // Om uppdateringen misslyckades - returnera felmeddelanden
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<UserResponseDto>.Fail(errors);
            }

            // Mappar den uppdaterade entiteten till response-DTO och returnerar den
            var response = _mapper.Map<UserResponseDto>(userEntity);
            return ServiceResult<UserResponseDto>.Ok(response);
        }
    }
}
