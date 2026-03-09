namespace community_api.Data.DTO
{
    // DTO for svar efter skapande av en ny kommentar
    // Returneras med HTTP 200 OK
    public record CommentAddResponseDto
    {
        // Det genererade kommentar-id:t
        public int CommentId { get; set; }

        // Texten i den skapade kommentaren
        public string CommentText { get; set; } = string.Empty;

        // Id:t pa inlagget som kommentaren tillhor
        public int? PostId { get; set; }

        // Id:t pa anvandaren som skapade kommentaren
        public string UserId { get; set; } = string.Empty;
    }
}
