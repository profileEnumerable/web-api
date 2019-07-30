using System;
using System.ComponentModel.DataAnnotations;

namespace Business_Logic_Layer.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public DateTime YearOfIssue { get; set; }
    }
}
