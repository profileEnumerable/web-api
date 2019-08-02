using System;
using System.ComponentModel.DataAnnotations;

namespace Business_Logic_Layer.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public DateTime YearOfIssue { get; set; }
    }
}
