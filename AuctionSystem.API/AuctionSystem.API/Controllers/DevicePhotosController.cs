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
                return BadRequest("Photo upload failed.");
            }

            return Ok(result);
        }

        [HttpGet("{id}/report")]
        public async Task<IActionResult> GetReport(int id)
        {
            var result = await _devicePhotoService.GetReportAsync(id);

            if (result == null)
            {
                return NotFound("AI device type report not found.");
            }

            return Ok(result);
        }
    }
}