namespace AuctionSystem.API.DTOs.DevicePhoto
{
    public class DevicePhotoReportResponse
    {
        public int Id { get; set; }

        public string PhotoUrl { get; set; } = string.Empty;

        public string DetectedDeviceType { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}