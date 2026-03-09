namespace community_api.Data.DTO
{
    // DTO for sokfiltrering av inlagg
    // Bada falt ar valfria - inga filter returnerar alla inlagg
    public record PostSearchFilterDto(
        // Valfri titel att filtrera pa (delstrangssokning)
        string? Title,
        // Valfritt kategorinamn att filtrera pa (delstrangssokning)
        string? Category
    );
}
