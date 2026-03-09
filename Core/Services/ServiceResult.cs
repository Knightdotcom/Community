namespace community_api.Core.Services
{
    // Generisk resultatklass för alla tjänsteoperationer
    // Används för att returnera antingen data eller felmeddelanden från tjänsterna
    // T är typen av data som returneras vid lyckad operation
    public class ServiceResult<T>
    {
        // Indikerar om operationen lyckades (true) eller misslyckades (false)
        public bool Success { get; init; }

        // Lista med felmeddelanden vid misslyckad operation - tom lista vid lyckad
        public IReadOnlyList<string>? ErrorMessages { get; init; } = Array.Empty<string>();

        // Returdata vid lyckad operation - null vid misslyckad
        public T? Data { get; init; }

        // Fabriksmetod för lyckad operation - returnerar data
        public static ServiceResult<T> Ok(T data) => new()
        {
            Success = true,
            Data = data
        };

        // Fabriksmetod för misslyckad operation - tar en lista med felmeddelanden
        public static ServiceResult<T> Fail(IEnumerable<string> errors) => new()
        {
            Success = false,
            ErrorMessages = errors.ToList()
        };

        // Fabriksmetod för misslyckad operation - tar ett enskilt felmeddelande
        public static ServiceResult<T> Fail(string error) => new()
        {
            Success = false,
            ErrorMessages = new[] { error }
        };
    }
}
