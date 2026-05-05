namespace AuctionSystem.API.DTOs.Auth
{
    public class AuthResponse
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
