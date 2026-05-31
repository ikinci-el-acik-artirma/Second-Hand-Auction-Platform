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
                Category = "  Laptop  ",
                StartingPrice = 5000,
                AuctionEndDate = auctionEndDate,
                SellerId = seller.Id
            });

            Assert.NotNull(result);
            Assert.Equal("Used laptop", result.Title);
            Assert.Equal("Laptop in working condition.", result.Description);
            Assert.Equal("Laptop", result.Category);
            Assert.Equal(5000, result.StartingPrice);
            Assert.Equal(auctionEndDate, result.AuctionEndDate);
            Assert.Equal(seller.Id, result.SellerId);
            Assert.Single(context.AuctionItems);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_WithInvalidCategory_ShouldReturnNull(string? category)
        {
            using var context = CreateDbContext();
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var service = new AuctionItemService(context);
            var request = CreateValidRequest(seller.Id);
            request.Category = category!;

            var result = await service.CreateAsync(request);

            Assert.Null(result);
            Assert.Empty(context.AuctionItems);
        }

        [Fact]
        public async Task GetAsync_WithCategory_ShouldReturnMatchingItemsIgnoringCase()
        {
            using var context = CreateDbContext();
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var service = new AuctionItemService(context);
            var phoneRequest = CreateValidRequest(seller.Id);
            var laptopRequest = CreateValidRequest(seller.Id);
            phoneRequest.Category = "Phone";
            laptopRequest.Category = "Laptop";

            await service.CreateAsync(phoneRequest);
            await service.CreateAsync(laptopRequest);

            var result = await service.GetAsync(" phone ");

            Assert.Single(result);
            Assert.Equal("Phone", result[0].Category);
        }

        [Fact]
        public async Task GetAsync_WithoutCategory_ShouldReturnAllItemsOrderedByEndDate()
        {
            using var context = CreateDbContext();
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var service = new AuctionItemService(context);
            var laterRequest = CreateValidRequest(seller.Id);
            var earlierRequest = CreateValidRequest(seller.Id);
            laterRequest.Title = "Later auction";
            laterRequest.AuctionEndDate = DateTime.UtcNow.AddDays(5);
            earlierRequest.Title = "Earlier auction";
            earlierRequest.AuctionEndDate = DateTime.UtcNow.AddDays(1);

            await service.CreateAsync(laterRequest);
            await service.CreateAsync(earlierRequest);

            var result = await service.GetAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("Earlier auction", result[0].Title);
            Assert.Equal("Later auction", result[1].Title);
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
                Category = "Phone",
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
