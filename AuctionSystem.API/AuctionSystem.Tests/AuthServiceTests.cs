using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs.Auth;
using AuctionSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuctionSystem.Tests
{
    public class AuthServiceTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task RegisterAsync_WithValidRequest_ShouldCreateUser()
        {
            using var context = CreateDbContext();
            var service = new AuthService(context);

            var request = new RegisterRequest
            {
                Email = "buyer@test.com",
                Password = "123456",
                Role = "Buyer"
            };

            var result = await service.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.Equal("buyer@test.com", result.Email);
            Assert.Equal("Buyer", result.Role);
            Assert.Equal("Registration successful.", result.Message);
            Assert.Equal(1, context.Users.Count());
        }

        [Fact]
        public async Task RegisterAsync_WithDuplicateEmail_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var service = new AuthService(context);

            var request = new RegisterRequest
            {
                Email = "seller@test.com",
                Password = "123456",
                Role = "Seller"
            };

            await service.RegisterAsync(request);
            var result = await service.RegisterAsync(request);

            Assert.Null(result);
            Assert.Equal(1, context.Users.Count());
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnUser()
        {
            using var context = CreateDbContext();
            var service = new AuthService(context);

            await service.RegisterAsync(new RegisterRequest
            {
                Email = "seller@test.com",
                Password = "123456",
                Role = "Seller"
            });

            var result = await service.LoginAsync(new LoginRequest
            {
                Email = "seller@test.com",
                Password = "123456"
            });

            Assert.NotNull(result);
            Assert.Equal("seller@test.com", result.Email);
            Assert.Equal("Seller", result.Role);
            Assert.Equal("Login successful.", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var service = new AuthService(context);

            await service.RegisterAsync(new RegisterRequest
            {
                Email = "seller@test.com",
                Password = "123456",
                Role = "Seller"
            });

            var result = await service.LoginAsync(new LoginRequest
            {
                Email = "seller@test.com",
                Password = "wrong-password"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidRole_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var service = new AuthService(context);

            var result = await service.RegisterAsync(new RegisterRequest
            {
                Email = "user@test.com",
                Password = "123456",
                Role = "Admin"
            });

            Assert.Null(result);
            Assert.Empty(context.Users);
        }
    }
}

