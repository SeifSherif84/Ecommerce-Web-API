namespace Store.G02.Domain.Entities.Orders
{
    // Part of Order Entity
    public class ShippingAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int BuildingNumber { get; set; }
    }
}