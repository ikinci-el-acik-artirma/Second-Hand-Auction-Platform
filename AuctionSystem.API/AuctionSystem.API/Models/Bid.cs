namespace AuctionSystem.API.Models
{
    public class Bid
    {
        public int Id { get; set; }

        public int AuctionItemId { get; set; }

        public int BuyerId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
