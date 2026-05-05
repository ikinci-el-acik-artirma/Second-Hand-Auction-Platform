namespace AuctionSystem.API.DTOs.DevicePhoto
{
    public class DevicePhotoResponse
    {
        public int Id { get; set; }

        public string PhotoUrl { get; set; } = string.Empty;

        public string DetectedDeviceType { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}