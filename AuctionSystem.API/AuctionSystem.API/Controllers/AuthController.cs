using AuctionSystem.API.DTOs.Auth;
using AuctionSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (result == null)
            {
                return BadRequest("Registration failed. Email may already exist or input may be invalid.");
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(result);
        }
    }
}

