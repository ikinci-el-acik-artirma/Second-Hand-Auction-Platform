namespace AuctionSystem.API.Models
{
    public class DevicePhoto
    {
        public int Id { get; set; }

        public string PhotoUrl { get; set; } = string.Empty;

        public string DetectedDeviceType { get; set; } = "Unknown";

        public int UserId { get; set; }
    }
}