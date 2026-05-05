using AuctionSystem.API.DTOs.DevicePhoto;
using AuctionSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicePhotosController : ControllerBase
    {
        private readonly DevicePhotoService _devicePhotoService;

        public DevicePhotosController(DevicePhotoService devicePhotoService)
        {
            _devicePhotoService = devicePhotoService;
        }

        [HttpPost("detect")]
        public async Task<IActionResult> UploadAndDetect([FromForm] DevicePhotoUploadRequest request)
        {
            var result = await _devicePhotoService.UploadAndDetectAsync(request);

            if (result == null)
            {
                return BadRequest("Photo upload failed. Please upload a valid image file.");
            }

            return Ok(result);
        }
    }
}
