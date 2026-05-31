using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs.Bid;
using AuctionSystem.API.Models;
using AuctionSystem.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuctionSystem.Tests
{
    public class BidServiceTests
    {
        [Fact]
        public async Task PlaceAsync_WithValidBuyerAndAmount_ShouldCreateBid()
        {
            using var context = CreateDbContext();
            var auctionItem = await AddAuctionItemAsync(context);
            var buyer = await AddUserAsync(context, "buyer@test.com", "Buyer");
            var service = new BidService(context);

            var result = await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = buyer.Id,
                Amount = 5500
            });

            Assert.NotNull(result);
            Assert.Equal(auctionItem.Id, result.AuctionItemId);
            Assert.Equal(buyer.Id, result.BuyerId);
            Assert.Equal(5500, result.Amount);
            Assert.Single(context.Bids);
        }

        [Fact]
        public async Task PlaceAsync_WithAmountEqualToStartingPrice_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var auctionItem = await AddAuctionItemAsync(context);
            var buyer = await AddUserAsync(context, "buyer@test.com", "Buyer");
            var service = new BidService(context);

            var result = await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = buyer.Id,
                Amount = auctionItem.StartingPrice
            });

            Assert.Null(result);
            Assert.Empty(context.Bids);
        }

        [Fact]
        public async Task PlaceAsync_WithAmountBelowHighestBid_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var auctionItem = await AddAuctionItemAsync(context);
            var buyer = await AddUserAsync(context, "buyer@test.com", "Buyer");
            var secondBuyer = await AddUserAsync(context, "second-buyer@test.com", "Buyer");
            var service = new BidService(context);

            await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = buyer.Id,
                Amount = 6000
            });

            var result = await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = secondBuyer.Id,
                Amount = 5900
            });

            Assert.Null(result);
            Assert.Single(context.Bids);
        }

        [Fact]
        public async Task PlaceAsync_WithSellerAccount_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var auctionItem = await AddAuctionItemAsync(context);
            var service = new BidService(context);

            var result = await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = auctionItem.SellerId,
                Amount = 5500
            });

            Assert.Null(result);
            Assert.Empty(context.Bids);
        }

        [Fact]
        public async Task PlaceAsync_WithExpiredAuction_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var auctionItem = await AddAuctionItemAsync(
                context,
                DateTime.UtcNow.AddMinutes(-1));
            var buyer = await AddUserAsync(context, "buyer@test.com", "Buyer");
            var service = new BidService(context);

            var result = await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = buyer.Id,
                Amount = 5500
            });

            Assert.Null(result);
            Assert.Empty(context.Bids);
        }

        [Fact]
        public async Task PlaceAsync_WithMissingAuction_ShouldReturnNull()
        {
            using var context = CreateDbContext();
            var buyer = await AddUserAsync(context, "buyer@test.com", "Buyer");
            var service = new BidService(context);

            var result = await service.PlaceAsync(999, new CreateBidRequest
            {
                BuyerId = buyer.Id,
                Amount = 5500
            });

            Assert.Null(result);
            Assert.Empty(context.Bids);
        }

        [Fact]
        public async Task GetForAuctionAsync_ShouldReturnHighestBidFirst()
        {
            using var context = CreateDbContext();
            var auctionItem = await AddAuctionItemAsync(context);
            var buyer = await AddUserAsync(context, "buyer@test.com", "Buyer");
            var secondBuyer = await AddUserAsync(context, "second-buyer@test.com", "Buyer");
            var service = new BidService(context);

            await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = buyer.Id,
                Amount = 5500
            });

            await service.PlaceAsync(auctionItem.Id, new CreateBidRequest
            {
                BuyerId = secondBuyer.Id,
                Amount = 6500
            });

            var result = await service.GetForAuctionAsync(auctionItem.Id);

            Assert.Equal(2, result.Count);
            Assert.Equal(6500, result[0].Amount);
            Assert.Equal(5500, result[1].Amount);
        }

        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static async Task<AuctionItem> AddAuctionItemAsync(
            ApplicationDbContext context,
            DateTime? auctionEndDate = null)
        {
            var seller = await AddUserAsync(context, "seller@test.com", "Seller");
            var auctionItem = new AuctionItem
            {
                Title = "Used laptop",
                Description = "Laptop in working condition.",
                Category = "Laptop",
                StartingPrice = 5000,
                AuctionEndDate = auctionEndDate ?? DateTime.UtcNow.AddDays(2),
                SellerId = seller.Id
            };

            context.AuctionItems.Add(auctionItem);
            await context.SaveChangesAsync();

            return auctionItem;
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
