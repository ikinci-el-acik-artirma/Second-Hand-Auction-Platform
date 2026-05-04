namespace AuctionSystem.API.Models
{
    public class DevicePhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string AiAssessmentResult { get; set; } 
        public int UserId { get; set; } 
    }
}