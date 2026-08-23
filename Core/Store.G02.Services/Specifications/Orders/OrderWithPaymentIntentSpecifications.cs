using Store.G02.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Specifications.Orders
{
    public class OrderWithPaymentIntentSpecifications : BaseSpecifications<Guid, Order>
    {
        public OrderWithPaymentIntentSpecifications(string paymentIntent) : base()
        {
            ApplyCriteria(paymentIntent);
        }

        public void ApplyCriteria(string paymentIntent)
        {
            Criteria = O => O.PaymentIntentId == paymentIntent;
        }

    }
}
