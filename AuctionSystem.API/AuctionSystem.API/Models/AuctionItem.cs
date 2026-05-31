namespace AuctionSystem.API.Models
{
    public class AuctionItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal StartingPrice { get; set; }

        public DateTime AuctionEndDate { get; set; }

        public int SellerId { get; set; }
    }
}
