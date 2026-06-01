namespace AuctionSystem.API.DTOs.Bid
{
    public class CreateBidRequest
    {
        public int BuyerId { get; set; }

        public decimal Amount { get; set; }
    }
}
