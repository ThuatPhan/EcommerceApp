namespace EcommerceApp.Application.DTO.Responses
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }
}
