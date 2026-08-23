namespace Store.G02.Domain.Entities.Orders
{
    // Part of OrderItem Entity
    public class ProductInOrder
    {
        public ProductInOrder(int productId, string productName, string pictureUrl)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
    }
}