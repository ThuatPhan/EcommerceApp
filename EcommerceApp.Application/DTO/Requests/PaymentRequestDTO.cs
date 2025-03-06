namespace EcommerceApp.Application.DTO.Requests
{
    public class PaymentRequestDTO
    {
        public required List<OrderItemRequestDTO> Items { get; set; } = new List<OrderItemRequestDTO>();
    }
}
