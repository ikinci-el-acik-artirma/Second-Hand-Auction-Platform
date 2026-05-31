using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs.AuctionItem;
using AuctionSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Services
{
    public class AuctionItemService
    {
        private readonly ApplicationDbContext _context;

        public AuctionItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuctionItemResponse?> CreateAsync(CreateAuctionItemRequest request)
        {
            var title = request.Title.Trim();
            var description = request.Description.Trim();

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(description) ||
                request.StartingPrice <= 0 ||
                request.AuctionEndDate <= DateTime.UtcNow)
            {
                return null;
            }

            var sellerExists = await _context.Users.AnyAsync(user =>
                user.Id == request.SellerId &&
                user.Role == "Seller");

            if (!sellerExists)
            {
                return null;
            }

            var auctionItem = new Models.AuctionItem
            {
                Title = title,
                Description = description,
                StartingPrice = request.StartingPrice,
                AuctionEndDate = request.AuctionEndDate,
                SellerId = request.SellerId
            };

            _context.AuctionItems.Add(auctionItem);
            await _context.SaveChangesAsync();

            return new AuctionItemResponse
            {
                Id = auctionItem.Id,
                Title = auctionItem.Title,
                Description = auctionItem.Description,
                StartingPrice = auctionItem.StartingPrice,
                AuctionEndDate = auctionItem.AuctionEndDate,
                SellerId = auctionItem.SellerId
            };
        }
    }
}
