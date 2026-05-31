using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs.Auth;
using AuctionSystem.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLower();
            var role = request.Role.Trim();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(role))
            {
                return null;
            }

            if (role != "Buyer" && role != "Seller")
            {
                return null;
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                return null;
            }

            var user = new User
            {
                Email = email,
                Role = role
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Message = "Registration successful."
            };
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Message = "Login successful."
            };
        }

        public async Task<ForgotPasswordResponse?> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var email = request.Email.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            var resetToken = Guid.NewGuid().ToString();

            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(30);

            await _context.SaveChangesAsync();

            return new ForgotPasswordResponse
            {
                Email = user.Email,
                ResetToken = resetToken,
                Message = "Password reset token created successfully."
            };
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var email = request.Email.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(request.ResetToken) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return false;
            }

            if (user.PasswordResetToken != request.ResetToken)
            {
                return false;
            }

            if (user.PasswordResetTokenExpiresAt == null ||
                user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiresAt = null;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}