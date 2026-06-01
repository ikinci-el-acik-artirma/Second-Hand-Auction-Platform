using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs.Bid;
using AuctionSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Services
{
    public class BidService
    {
        private readonly ApplicationDbContext _context;

        public BidService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BidResponse?> PlaceAsync(
            int auctionItemId,
            CreateBidRequest request)
        {
            if (request.Amount <= 0)
            {
                return null;
            }

            var auctionItem = await _context.AuctionItems
                .FirstOrDefaultAsync(item => item.Id == auctionItemId);

            if (auctionItem == null || auctionItem.AuctionEndDate <= DateTime.UtcNow)
            {
                return null;
            }

            var buyerExists = await _context.Users.AnyAsync(user =>
                user.Id == request.BuyerId &&
                user.Role == "Buyer");

            if (!buyerExists)
            {
                return null;
            }

            var highestBid = await _context.Bids
                .Where(bid => bid.AuctionItemId == auctionItemId)
                .MaxAsync(bid => (decimal?)bid.Amount);

            var minimumAmount = highestBid ?? auctionItem.StartingPrice;

            if (request.Amount <= minimumAmount)
            {
                return null;
            }

            var bid = new Bid
            {
                AuctionItemId = auctionItemId,
                BuyerId = request.BuyerId,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            return MapResponse(bid);
        }

        public async Task<List<BidResponse>> GetForAuctionAsync(int auctionItemId)
        {
            return await _context.Bids
                .AsNoTracking()
                .Where(bid => bid.AuctionItemId == auctionItemId)
                .OrderByDescending(bid => bid.Amount)
                .ThenBy(bid => bid.CreatedAt)
                .Select(bid => new BidResponse
                {
                    Id = bid.Id,
                    AuctionItemId = bid.AuctionItemId,
                    BuyerId = bid.BuyerId,
                    Amount = bid.Amount,
                    CreatedAt = bid.CreatedAt
                })
                .ToListAsync();
        }

        private static BidResponse MapResponse(Bid bid)
        {
            return new BidResponse
            {
                Id = bid.Id,
                AuctionItemId = bid.AuctionItemId,
                BuyerId = bid.BuyerId,
                Amount = bid.Amount,
                CreatedAt = bid.CreatedAt
            };
        }
    }
}
