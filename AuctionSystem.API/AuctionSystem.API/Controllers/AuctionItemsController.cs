using AuctionSystem.API.DTOs.AuctionItem;
using AuctionSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionItemsController : ControllerBase
    {
        private readonly AuctionItemService _auctionItemService;

        public AuctionItemsController(AuctionItemService auctionItemService)
        {
            _auctionItemService = auctionItemService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAuctionItemRequest request)
        {
            var result = await _auctionItemService.CreateAsync(request);

            if (result == null)
            {
                return BadRequest("Auction listing could not be created. Check seller and product details.");
            }

            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }
    }
}
