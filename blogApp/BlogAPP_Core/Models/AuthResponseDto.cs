namespace BlogAPP_Core.Models
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public object? User { get; set; }
        public string? ErrorMessage { get; set; }
    }
}