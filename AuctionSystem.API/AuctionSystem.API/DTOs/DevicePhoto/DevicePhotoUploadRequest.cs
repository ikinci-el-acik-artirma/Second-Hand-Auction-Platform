using Microsoft.AspNetCore.Http;

namespace AuctionSystem.API.DTOs.DevicePhoto
{
    public class DevicePhotoUploadRequest
    {
        public IFormFile Photo { get; set; } = default!;

        public int UserId { get; set; }
    }
}
