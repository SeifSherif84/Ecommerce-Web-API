namespace Store.G02.Domain.Entities.Orders
{
    // Table: OrderItems
    public class OrderItem : BaseEntity<int>
    {
        public OrderItem()
        {
            
        }

        public OrderItem(ProductInOrder product, decimal price, int quantity)
        {
            Product = product;
            Price = price;
            Quantity = quantity;
        }

        public ProductInOrder Product { get; set; } // Owned Entity
        public decimal Price { get; set; }
        public int Quantity { get; set; }


        public Guid OrderId { get; set; } // Foreign Key
        public Order Order { get; set; } // Navigation Property
    }
}