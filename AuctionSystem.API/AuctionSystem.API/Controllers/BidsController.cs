using AuctionSystem.API.DTOs.Bid;
using AuctionSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSystem.API.Controllers
{
    [ApiController]
    [Route("api/auctionitems/{auctionItemId:int}/bids")]
    public class BidsController : ControllerBase
    {
        private readonly BidService _bidService;

        public BidsController(BidService bidService)
        {
            _bidService = bidService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int auctionItemId)
        {
            return Ok(await _bidService.GetForAuctionAsync(auctionItemId));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            int auctionItemId,
            CreateBidRequest request)
        {
            var result = await _bidService.PlaceAsync(auctionItemId, request);

            if (result == null)
            {
                return BadRequest("Bid could not be placed. Check buyer, amount, and auction end date.");
            }

            return CreatedAtAction(nameof(Get), new { auctionItemId }, result);
        }
    }
}
