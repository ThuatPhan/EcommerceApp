namespace EcommerceApp.Application.DTO.Responses
{
    public class PagedResult<T>
    {
        public int RemainingItems { get; set; }
        public List<T> Items { get; set; }
    }

}
