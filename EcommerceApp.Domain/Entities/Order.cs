namespace EcommerceApp.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }
}
