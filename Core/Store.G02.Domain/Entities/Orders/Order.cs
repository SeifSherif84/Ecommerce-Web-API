using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Entities.Orders
{
    // Table: Orders
    public class Order : BaseEntity<Guid>
    {
        public Order(string userEmail)
        {
            UserEmail = userEmail;
        }

        public Order(string userEmail, ShippingAddress shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subtotal, string? paymentIntentId)
        {
            UserEmail = userEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            Subtotal = subtotal;
            PaymentIntentId = paymentIntentId;
        }

        public string UserEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public ShippingAddress ShippingAddress { get; set; } // Owned Entity


        public int DeliveryMethodId { get; set; } // Foreign Key
        public DeliveryMethod DeliveryMethod { get; set; } // Navigation Property


        public ICollection<OrderItem> Items { get; set; } // Navigation Property


        public decimal Subtotal { get; set; } // Total before shipping and taxes

        //[NotMapped]
        //public decimal Total { get; set; } // Total after shipping and taxes


        // NotMapped alternative
        public decimal GetTotal()
        {
            return Subtotal + DeliveryMethod.Price;
        }

        public string? PaymentIntentId { get; set; } // For future use with payment gateway

    }
}
