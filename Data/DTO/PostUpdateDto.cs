namespace community_api.Data.DTO
{
    // DTO for uppdatering av ett befintligt inlagg
    // Alla falt ar valfria - bara angivna falt uppdateras (partial update)
    public record PostUpdateDto
    {
        // Valfri ny titel - uppdateras bara om den ar angiven (inte null)
        public string? Title { get; set; }

        // Valfri ny text - uppdateras bara om den ar angiven (inte null)
        public string? Text { get; set; }

        // Valfritt nytt kategori-id - uppdateras bara om det ar angivet
        public int? CategoryId { get; set; }
    }
}
