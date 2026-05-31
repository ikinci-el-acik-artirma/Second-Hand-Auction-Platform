namespace AuctionSystem.API.DTOs.AuctionItem
{
    public class CreateAuctionItemRequest
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal StartingPrice { get; set; }

        public DateTime AuctionEndDate { get; set; }

        public int SellerId { get; set; }
    }
}
