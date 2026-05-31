namespace AuctionSystem.API.DTOs.Bid
{
    public class BidResponse
    {
        public int Id { get; set; }

        public int AuctionItemId { get; set; }

        public int BuyerId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
