using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities.Orders;
using Store.G02.Domain.Entities.Products;
using Store.G02.Domain.Exceptions.NotFound;
using Store.G02.Services.Abstractions.Payment;
using Store.G02.Shared.Dtos.Baskets;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services
{
    public class PaymentService(IBasketRepository _basketRepository, IUnitOfWork _unitOfWork, IConfiguration _configuration, IMapper _mapper) : IPaymentService
    {
        public async Task<BasketDto> CreatePaymentIntentAsync(string basketId)
        {
            // Calculate Amount = SubTotal + Delivery Method Cost

            // get basket by id
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket is null) throw new BasketNotFoundException(basketId);


            // check product and its price
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.GetRepository<int, Domain.Entities.Products.Product>().GetByIdAsync(item.Id);
                if (product is null) throw new ProductNotFoundException(item.Id);

                item.Price = product.Price;
            }

            // calculate subTotal
            var subTotal = basket.Items.Sum(I => I.Price * I.Quantity);

            // Get Delivery Method by Id
            if (!basket.DeliveryMethodId.HasValue) throw new DeliveryMethodNotFoundException(-1);
            var deliveryMethod = await _unitOfWork.GetRepository<int, DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
            if (deliveryMethod is null) throw new DeliveryMethodNotFoundException(basket.DeliveryMethodId.Value);

            basket.ShippingCost = deliveryMethod.Price;
            var amount = subTotal + deliveryMethod.Price;

            // Send Amount TO Stripe
            StripeConfiguration.ApiKey = _configuration["StripeOptions:SecretKey"];
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (basket.PaymentIntentId is null)
            {
                // Create
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)amount * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await paymentIntentService.CreateAsync(options);
            }
            else
            {
                // Update
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)amount * 100,
                };
                paymentIntent = await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
            }

            basket.PaymentIntentId = paymentIntent.Id;
            basket.ClientSecret = paymentIntent.ClientSecret;
            await _basketRepository.CreateBasketAsync(basket, TimeSpan.FromDays(1));
            return _mapper.Map<BasketDto>(basket);
        }
    }
}
