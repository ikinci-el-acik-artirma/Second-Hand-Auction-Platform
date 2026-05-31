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
            var title = request.Title?.Trim() ?? string.Empty;
            var description = request.Description?.Trim() ?? string.Empty;
            var category = request.Category?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(category) ||
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
                Category = category,
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
                Category = auctionItem.Category,
                StartingPrice = auctionItem.StartingPrice,
                AuctionEndDate = auctionItem.AuctionEndDate,
                SellerId = auctionItem.SellerId
            };
        }

        public async Task<List<AuctionItemResponse>> GetAsync(string? category = null)
        {
            var query = _context.AuctionItems.AsNoTracking();
            var normalizedCategory = category?.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(normalizedCategory))
            {
                query = query.Where(item => item.Category.ToLower() == normalizedCategory);
            }

            return await query
                .OrderBy(item => item.AuctionEndDate)
                .Select(item => new AuctionItemResponse
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Category = item.Category,
                    StartingPrice = item.StartingPrice,
                    AuctionEndDate = item.AuctionEndDate,
                    SellerId = item.SellerId
                })
                .ToListAsync();
        }
    }
}
