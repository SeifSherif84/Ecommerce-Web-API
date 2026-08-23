using Store.G02.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Specifications.Orders
{
    public class OrderSpecifications : BaseSpecifications<Guid,Order>
    {
        public OrderSpecifications(Guid Id, string userEmail) : base() // Get Order by Id for specific user
        {
            ApplyFilteration(Id, userEmail);
            ApplyIncludes();
        }

        public OrderSpecifications(string userEmail) // Get Orders for specific user
        {
            ApplyFilteration(null, userEmail);
            ApplyIncludes();
        }

        public void ApplyFilteration(Guid? Id, string userEmail)
        {
            Criteria = Id is null
                       ? O => O.UserEmail == userEmail
                       : O => O.Id == Id && O.UserEmail.ToLower() == userEmail.ToLower();
        }

        public void ApplyIncludes()
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
        }

    }
}
