﻿namespace EcommerceApp.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<CartItem> Items { get; set; }
    }
}
