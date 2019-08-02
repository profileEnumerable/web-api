using Data_Access_Layer.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business_Logic_Layer.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public ICollection<Product> Products { get; set; }
    }
}
