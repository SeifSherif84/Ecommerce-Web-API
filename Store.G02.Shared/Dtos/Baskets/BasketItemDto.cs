namespace Store.G02.Shared.Dtos.Baskets
{
    public class BasketItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
    }
}