using System.Collections.Generic;

namespace Data_Access_Layer.Entities
{
    public class Order
    {
        public Order()
        {
            Products = new List<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}