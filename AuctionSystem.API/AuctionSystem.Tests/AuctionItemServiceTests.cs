using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs.AuctionItem;
using AuctionSystem.API.Models;
using AuctionSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuctionSystem.Tests
{
    public class AuctionItemServiceTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_WithValidSellerAndProductDetails_ShouldCreateAuctionItem()
        {
            using var context = CreateDbContext();
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var service = new AuctionItemService(context);
            var auctionEndDate = DateTime.UtcNow.AddDays(3);

            var result = await service.CreateAsync(new CreateAuctionItemRequest
            {
                Title = "  Used laptop  ",
                Description = "  Laptop in working condition.  ",
                StartingPrice = 5000,
                AuctionEndDate = auctionEndDate,
                SellerId = seller.Id
            });

            Assert.NotNull(result);
            Assert.Equal("Used laptop", result.Title);
            Assert.Equal("Laptop in working condition.", result.Description);
            Assert.Equal(5000, result.StartingPrice);
            Assert.Equal(auctionEndDate, result.AuctionEndDate);
            Assert.Equal(seller.Id, result.SellerId);
            Assert.Single(context.AuctionItems);
        }

        [Fact]
        public async Task CreateAsync_WithBuyerAccount_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var buyer = await AddUserAsync(context, "buyer@test.com", "Buyer");
            var service = new AuctionItemService(context);

            var result = await service.CreateAsync(CreateValidRequest(buyer.Id));

            Assert.Null(result);
            Assert.Empty(context.AuctionItems);
        }

        [Fact]
        public async Task CreateAsync_WithMissingSeller_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var service = new AuctionItemService(context);

            var result = await service.CreateAsync(CreateValidRequest(999));

            Assert.Null(result);
            Assert.Empty(context.AuctionItems);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_WithInvalidTitle_ShouldReturnNull(string title)
        {
            using var context = CreateDbContext();
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var service = new AuctionItemService(context);
            var request = CreateValidRequest(seller.Id);
            request.Title = title;

            var result = await service.CreateAsync(request);

            Assert.Null(result);
            Assert.Empty(context.AuctionItems);
        }

        [Fact]
        public async Task CreateAsync_WithNonPositiveStartingPrice_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var service = new AuctionItemService(context);
            var request = CreateValidRequest(seller.Id);
            request.StartingPrice = 0;

            var result = await service.CreateAsync(request);

            Assert.Null(result);
            Assert.Empty(context.AuctionItems);
        }

        [Fact]
        public async Task CreateAsync_WithPastAuctionEndDate_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var service = new AuctionItemService(context);
            var request = CreateValidRequest(seller.Id);
            request.AuctionEndDate = DateTime.UtcNow.AddMinutes(-1);

            var result = await service.CreateAsync(request);

            Assert.Null(result);
            Assert.Empty(context.AuctionItems);
        }

        private static CreateAuctionItemRequest CreateValidRequest(int sellerId)
        {
            return new CreateAuctionItemRequest
            {
                Title = "Used phone",
                Description = "Phone with minor scratches.",
                StartingPrice = 2500,
                AuctionEndDate = DateTime.UtcNow.AddDays(2),
                SellerId = sellerId
            };
        }

        private static async Task<User> AddUserAsync(
            ApplicationDbContext context,
            string email,
            string role)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = "not-used-in-this-test",
                Role = role
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }
    }
}
