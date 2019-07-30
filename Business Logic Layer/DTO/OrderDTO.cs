using System.Collections.Generic;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
