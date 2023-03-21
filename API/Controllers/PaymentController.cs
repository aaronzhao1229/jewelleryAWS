using API.Data;
using API.DTOs;
using API.Entities.OrderAggregate;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace API.Controllers
{
  public class PaymentsController : BaseApiController
    {
        private readonly PaymentService _paymentService;
        private readonly StoreContext _context;
        private readonly IConfiguration _config;
        public PaymentsController(PaymentService paymentService, StoreContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BasketDto>>  CreateOrUpdatePaymentIntent()
        // We return BasketDto because we need to store information on the client and the information we are keen on passing back to the clients is like client secret. We'll keep the paymentIntentId as a property that we pass back as our DTO anyway, although the only value that we really need to send back to the client is the client secret, but we will add the paymentIntentId as well. But you will find that the client secret is actually a combination of a random string and the paymentIntentId, but we'll pass that info back in our basketDto, and we'll also update our state on the client. 
        {
            var basket = await _context.Baskets.RetrieveBasketWithItems(User.Identity.Name).FirstOrDefaultAsync();

            if (basket == null) return NotFound();

            var intent = await _paymentService.CreateOrUpdatePaymentIntent(basket);

            if (intent == null) return BadRequest(new ProblemDetails{Title = "Problem creating payment intent"}); // on the live website, you might want to use a more vague message than this

            basket.PaymentIntentId = basket.PaymentIntentId ?? intent.Id;
            basket.ClientSecret = basket.ClientSecret ?? intent.ClientSecret;

            _context.Update(basket);

            var result = await _context.SaveChangesAsync() > 0;
            
            if (!result) return BadRequest(new ProblemDetails{Title = "Problem updating basket with intent"});

            return basket.MapBasketToDto();
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        // We are going to read the request that stripe will send us after a payment has successfully been received. 
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(); // get a json response out of the request

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _config["StripeSettings:WhSecret"]); // get access to the strip events that we're interested in. Get the Stripe-Signature and compare it to the WhSecret in the config

            var charge = (Charge)stripeEvent.Data.Object; // access the charge as we will listen for the charge events. (Charge) to cast this charge to a Charge object from Stripe.
            // from this charge, we will want to get access to the paymentIntentId and get hold of the order from our database that matches the PaymentIntentId.
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.PaymentIntentId == charge.PaymentIntentId);

            if (charge.Status == "succeeded") order.OrderStatus = OrderStatus.PaymentReceived;

            await _context.SaveChangesAsync();

            return new EmptyResult(); // if we don't do this, Stripe will continue to send events to this end point because it thinks there's a problemm with us receiving its request, and they will keep trying for a number of days even to keep trying to access this web. So it is important we send this back to Stripe to let Stripe know we've received this.

        }
    }
}