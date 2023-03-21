using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Stripe;

namespace API.Services
{
    public class PaymentService
    {
        // we need to get our configuration values because we need access to the stripe secret key inside here
        private readonly IConfiguration _config;
        
        public PaymentService(IConfiguration config)
        { 
            _config = config;
        }

        public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var service = new PaymentIntentService();

            var intent = new PaymentIntent();
            var subtotal = basket.Items.Sum(item => item.Quantity * item.Product.Price);
            var deliveryFee = subtotal > 10000 ? 0 : 500;

            // we'll check to see if we have paymentIntent already inside our basket. If we do, then we know we're updating the paymentIntent. If not, then we know that we're creating a new payment intent and we need to contact Stripe in a slightly different way depending on if we're updating or creating one. So we'll check to see if the string is null or empty for the basket and the paymentIntent ID.
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = subtotal + deliveryFee,  // we use long as type also due to Stripe
                    Currency = "nzd",
                    PaymentMethodTypes = new List<string> {"card"} // there are other ways people can pay for things nowadays. but the interest to keep things simple and actually finishing this course at some points, we just go for the card option
                };
                intent = await service.CreateAsync(options); // this is going to create a new payment intent for us.
                
            }
            else // if we have an PaymentIntentId in our basket, this means that we're updating an already existing payment intent.
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = subtotal + deliveryFee // we need to double check the amount because customers may already delete or add items
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            return intent;
        }


    }
}