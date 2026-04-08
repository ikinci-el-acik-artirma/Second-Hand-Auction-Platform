using System;

namespace SecondHandAuctionPlatform.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
