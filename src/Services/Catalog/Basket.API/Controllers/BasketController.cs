using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        // injecting the IBasketRepository (its an interface of the basketRepository)
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcServices _discountGrpcService;

        public BasketController(IBasketRepository basketRepository, DiscountGrpcServices discountGrpcService)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(_discountGrpcService));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var shoppingCart = await _basketRepository.GetBasket(userName);

            // creates a new shopping cart if the value is null
            return Ok(shoppingCart ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // Communicate with Discount.Grpc
            // and calculate latest prices of product into the shopping cart
            // consume Discount Grpc

            if (basket == null)
            {
                return BadRequest(new { Message = "Basket is null" });
            }

            foreach (var item in basket.Items)
            {
                if (string.IsNullOrWhiteSpace(item.ProductName))
                {
                    return BadRequest(new { Message = $"The item {item.ProductId} is missing a name" });
                }

                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await _basketRepository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _basketRepository.DeleteBasket(userName);
            return Ok();
        }
    }
}
