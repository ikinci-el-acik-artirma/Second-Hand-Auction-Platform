using System;

namespace SecondHandAuctionPlatform.Models
{
    public class AuctionItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime AuctionEndDate { get; set; }
        public int SellerId { get; set; } // Satıcının ID'si
    }
}
