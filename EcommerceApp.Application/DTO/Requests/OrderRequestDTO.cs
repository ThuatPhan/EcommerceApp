namespace EcommerceApp.Application.DTO.Requests
{
    public class OrderRequestDTO
    {
        public required string ShipAddress { get; set; }
        public required List<OrderItemRequestDTO> Items { get; set; }
    }
}
