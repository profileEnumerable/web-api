using System;
using System.Collections.Generic;

namespace Data_Access_Layer.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public DateTime YearOfIssue { get; set; }
    }
}
