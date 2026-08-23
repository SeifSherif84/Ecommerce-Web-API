using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Shared.Dtos.Orders
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public OrderAddressDto OrderAddressDto { get; set; } 
        public string DeliveryMethodName { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } 
        public string? PaymentIntentId { get; set; }
        public decimal Subtotal { get; set; } 
        public decimal Total { get; set; }

    }
}
