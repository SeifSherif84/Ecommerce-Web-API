using AutoMapper;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities.Orders;
using Store.G02.Domain.Entities.Products;
using Store.G02.Domain.Exceptions.BadRequest;
using Store.G02.Domain.Exceptions.NotFound;
using Store.G02.Services.Abstractions.Orders;
using Store.G02.Services.Specifications.Orders;
using Store.G02.Shared.Dtos.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Orders
{
    public class OrderService(IUnitOfWork _unitOfWork, 
                              IMapper _mapper,
                              IBasketRepository _basketRepository) 
        : IOrderService
    {
        public async Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest, string userEmail)
        {
            var shippingAddress = _mapper.Map<ShippingAddress>(orderRequest.ShipToAddress);
            var deliveryMethod = await _unitOfWork.GetRepository<int, DeliveryMethod>().GetByIdAsync(orderRequest.DeliveryMethodId);
            if (deliveryMethod == null) throw new DeliveryMethodNotFoundException(orderRequest.DeliveryMethodId);

            var basket = await _basketRepository.GetBasketAsync(orderRequest.BasketId);
            if (basket == null) throw new BasketNotFoundException(orderRequest.BasketId);

            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.GetRepository<int, Product>().GetByIdAsync(item.Id);
                if (product == null) throw new ProductNotFoundException(item.Id);
                if (product.Price != item.Price) item.Price = product.Price;
                var productInOrder = new ProductInOrder(item.Id, item.ProductName, item.PictureUrl);
                var orderItem = new OrderItem(productInOrder, item.Price, item.Quantity);
                orderItems.Add(orderItem);
            }

            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existsOrder = await _unitOfWork.GetRepository<Guid, Order>().GetByIdAsync(spec);
            if (existsOrder is not null)
                _unitOfWork.GetRepository<Guid, Order>().Delete(existsOrder);

            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            var order = new Order(userEmail, shippingAddress, deliveryMethod, orderItems, subtotal, basket.PaymentIntentId);
            await _unitOfWork.GetRepository<Guid, Order>().AddAsync(order);
            var count = await _unitOfWork.SaveChangesAsync();
            if(count <= 0) throw new CreateOrderBadRequestException();
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<IEnumerable<DeliveryMethodResponse>> GetAllDeliveryMethodAsync()
        {
            var DeliveryMethods = await _unitOfWork.GetRepository<int, DeliveryMethod>().GetAllAsync();
            return _mapper.Map<IEnumerable<DeliveryMethodResponse>>(DeliveryMethods);
        }

        public async Task<OrderResponse> GetOrderByIdForSpecificUserAsync(Guid Id, string userEmail)
        {
            var orderSpec = new OrderSpecifications(Id, userEmail);
            var order = await _unitOfWork.GetRepository<Guid, Order>().GetByIdAsync(orderSpec);
            if (order == null) throw new OrderNotFoundException(Id);
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersForSpecificUserAsync(string userEmail)
        {
            var orderSpec = new OrderSpecifications(userEmail);
            var orders = await _unitOfWork.GetRepository<Guid, Order>().GetAllAsync(orderSpec);
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }
    }
}
