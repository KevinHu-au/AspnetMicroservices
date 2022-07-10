using System;

namespace Shopping.Aggregator.Models
{
    public class BasektModel
    {
        public string UserName { get; set; }
        public List<BasketItemExtendedModel> Items { get; set; } = new List<BasketItemExtendedModel>();

        public decimal totalPrice { get; set; }
    }
}
