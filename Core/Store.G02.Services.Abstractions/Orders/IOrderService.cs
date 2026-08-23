using Store.G02.Shared.Dtos.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Abstractions.Orders
{
    public interface IOrderService 
    {
        Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest, string userEmail);
        Task<IEnumerable<DeliveryMethodResponse>> GetAllDeliveryMethodAsync();
        Task<OrderResponse> GetOrderByIdForSpecificUserAsync(Guid Id, string userEmail);
        Task<IEnumerable<OrderResponse>> GetOrdersForSpecificUserAsync(string userEmail);
    }
}
