namespace EcommerceApp.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Icon { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
